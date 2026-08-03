using System.Collections.Concurrent;

namespace Family_and_Spa_Wellness.Models;

// Backed by the ServiceCategory table but cached in memory so every page that
// renders a service icon doesn't need its own DB round trip - Configure()
// loads the cache at startup and SetIcon() keeps it in sync when an admin
// edits a category from the Service Catalog page.
public static class ServiceCategoryIcon
{
    private const string DefaultIcon = "\U0001F31F";

    private static readonly ConcurrentDictionary<string, string> Icons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Massage"] = "\U0001F486",
        ["Facial & Skincare"] = "✨",
        ["Body Treatments"] = "\U0001F9D6",
        ["Nail Care"] = "\U0001F485",
        ["Wellness & Add-Ons"] = "\U0001F33F",
    };

    public static void Configure(IEnumerable<ServiceCategory> categories)
    {
        foreach (var category in categories)
        {
            Icons[category.Name] = string.IsNullOrWhiteSpace(category.Icon) ? DefaultIcon : category.Icon;
        }
    }

    public static void SetIcon(string category, string icon)
        => Icons[category] = string.IsNullOrWhiteSpace(icon) ? DefaultIcon : icon;

    public static string GetIcon(string category)
        => Icons.TryGetValue(category, out var icon) ? icon : DefaultIcon;
}
