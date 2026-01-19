namespace ZincirApp.Settings;

public class AppSettings
{
    public string Language { get; init; } = "tr-TR"; // Default language (tr-TR or en-US)
    public string Theme { get; init; } = "Dark"; // Default theme (Light or Dark)
    public string PrimaryColor { get; init; } = "Amber";
    public string SecondaryColor { get; init; } = "Pink";
    /*  Red, Pink, Purple, DeepPurple, Indigo, Blue, LightBlue
        Cyan, Teal, Green, LightGreen, Lime, Yellow, Amber
        Orange, DeepOrange, Brown, Grey, BlueGrey */
    public string? HashSignature { get; set; }
}