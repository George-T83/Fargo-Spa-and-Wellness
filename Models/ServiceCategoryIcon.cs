namespace Family_and_Spa_Wellness.Models;

public static class ServiceCategoryIcon
{
    private static readonly Dictionary<string, string> Icons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Massage"] = "\U0001F486",
        ["Facial & Skincare"] = "✨",
        ["Body"] = "\U0001F9D6",
        ["Wellness"] = "\U0001F33F",
    };

    public static string GetIcon(string category)
        => Icons.TryGetValue(category, out var icon) ? icon : "\U0001F31F";
}
