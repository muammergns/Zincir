namespace ZincirApp.Models;

public class AppSettings
{
    public string Language { get; set; } = "tr-TR"; // Default language (tr-TR or en-US)
    public string Theme { get; set; } = "Dark"; // Default theme (Light or Dark)
    public string PrimaryColor { get; set; } = "Amber";
    public string SecondaryColor { get; set; } = "Pink";
    /*  Red, Pink, Purple, DeepPurple, Indigo, Blue, LightBlue
        Cyan, Teal, Green, LightGreen, Lime, Yellow, Amber
        Orange, DeepOrange, Brown, Grey, BlueGrey */
    public string? HashSignature { get; set; }
}