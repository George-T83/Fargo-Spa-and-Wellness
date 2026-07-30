namespace Family_and_Spa_Wellness.Models;

public static class ServiceCategoryIcon
{
    public static string GetIcon(string category)
    {
        if (category.Contains("Massage", StringComparison.OrdinalIgnoreCase)) return "\U0001F486";
        if (category.Contains("Facial", StringComparison.OrdinalIgnoreCase)) return "✨";
        if (category.Contains("Body", StringComparison.OrdinalIgnoreCase)) return "\U0001F9D6";
        if (category.Contains("Wellness", StringComparison.OrdinalIgnoreCase)) return "\U0001F33F";
        return "\U0001F31F";
    }
}
