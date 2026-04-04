using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ZincirApp.Models;

namespace ZincirApp.Services;
//cd ZincirApp/
//dotnet ef migrations add Version_Init --context ZincirDbContext

public class ZincirDbContextFactory : IDesignTimeDbContextFactory<ZincirDbContext>
{
    public ZincirDbContext CreateDbContext(string[] args)
    {
        var mockDbPath = Path.Combine(Path.GetTempPath(), "dummy_migration.db");
        const string mockSalt = "dummy_salt";
            
        return new ZincirDbContext(mockDbPath, mockSalt);
    }
}
public class ZincirDbContext : DbContext
{
    private readonly string _dbFilePath;
    private readonly string _salt;
    private readonly string? _pin;

    public DbSet<HabitModel> Habits { get; set; } = null!;
    public DbSet<HabitLogModel> HabitLogs { get; set; } = null!;
    public DbSet<TodoModel> Todos { get; set; } = null!;
    public DbSet<PomodoroModel> Pomodoros { get; set; } = null!;
    
    public ZincirDbContext(string dbFilePath, string salt, string? pin = null)
    {
        _dbFilePath = dbFilePath;
        _salt = salt;
        _pin = pin;
        try
        {
            if (string.IsNullOrWhiteSpace(_dbFilePath)) return;
            var directory = Path.GetDirectoryName(_dbFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        catch
        {
            //ignore
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        string source = string.IsNullOrWhiteSpace(_dbFilePath) ? ":memory:" : _dbFilePath;
        string finalKey = string.IsNullOrEmpty(_pin) ? _salt : _pin + _salt;
            
        string connectionString = $"Data Source={source};Password={finalKey};";
            
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<HabitModel>(entity =>
        {
            entity.HasKey(e => e.Uuid);
            
            entity.HasMany(e => e.HabitLogs)
                  .WithOne(e => e.Habit)
                  .HasForeignKey(e => e.HabitUuid)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Pomodoros)
                  .WithOne(e => e.Habit)
                  .HasForeignKey(e => e.HabitUuid)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        modelBuilder.Entity<HabitLogModel>(entity =>
        {
            entity.HasKey(e => e.Uuid);
        });

        modelBuilder.Entity<TodoModel>(entity =>
        {
            entity.HasKey(e => e.Uuid);

            entity.HasOne(e => e.ParentTodo)
                  .WithMany(e => e.SubTodos)
                  .HasForeignKey(e => e.ParentTodoUuid)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Pomodoros)
                  .WithOne(e => e.Todo)
                  .HasForeignKey(e => e.TodoUuid)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PomodoroModel>(entity =>
        {
            entity.HasKey(e => e.Uuid);
        });
    }
}

public enum DbState
{
    Insert,
    Update,
    Delete,
    Get
}


public class ZincirDbService
{
    private string? _dbFilePath;
    private string? _salt;
    private string? _pin;

    private Task? _initTask;
    private readonly Lock _initLock = new Lock();

    public bool IsInitialized => _initTask?.IsCompleted == true;
    
    public void SetPath(string path) => _dbFilePath = path;
    public void SetSalt(string salt) => _salt = salt;
    public void SetPin(string? pin) => _pin = pin;

    private ZincirDbContext? GetDbContext()
    {
        if (string.IsNullOrWhiteSpace(_dbFilePath) || string.IsNullOrWhiteSpace(_salt))
            return null;
        return new ZincirDbContext(_dbFilePath, _salt, _pin);
    }
    
    public Task EnsureInitializedAsync()
    {
        lock (_initLock)
        {
            if (_initTask == null)
            {
                _initTask = InitializeDatabaseInternalAsync();
            }
            return _initTask;
        }
    }

    private async Task InitializeDatabaseInternalAsync()
    {
        await Task.Run(async () =>
        {
            await using var context = GetDbContext();
            if (context != null)
            {
                await context.Database.MigrateAsync();
            }
        });
    }

    public async Task InsertAsync<T>(T entity) where T : class
    {
        await EnsureInitializedAsync();
        await using var context = GetDbContext();
        if (context != null)
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        await EnsureInitializedAsync();
        await using var context = GetDbContext();
        if (context != null)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync<T>(Guid uuid) where T : class
    {
        await EnsureInitializedAsync();
        await using var context = GetDbContext();
        if (context != null)
        {
            var entity = await context.Set<T>().FindAsync(uuid);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        await EnsureInitializedAsync();
        await using var context = GetDbContext();
        if (context != null) return await context.Set<T>().ToListAsync();
        return [];
    }
    
    public async Task<List<HabitModel>> GetHabitsWithLogsAsync()
    {
        await EnsureInitializedAsync();
        await using var context = GetDbContext();
        if (context != null)
            return await context.Habits
                .Include(h => h.HabitLogs)
                .ToListAsync();
        return [];
    }
    
    public async Task<List<TodoModel>> GetSubTodosAsync(Guid parentTodoUuid)
    {
        await EnsureInitializedAsync();
        await using var context = GetDbContext();
        if (context != null)
            return await context.Todos
                .Where(t => t.ParentTodoUuid == parentTodoUuid)
                .ToListAsync();
        return [];
    }
}
