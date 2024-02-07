using System.Text.Json;
using CounterStrikeSharp.API.Modules.Menu;

namespace AdminMenuAPI;

public static class AdminMenuUtilities
{
    public static async Task<bool> InsertCategory(this CenterHtmlMenu menu, string modulePath, string category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulePath);
        if (reasonPages == null)
        {
            return false;
        }

        if (reasonPages.Any(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        reasonPages.Add(new AdminMenuPage
        {
            Title = category,
            Reasons = new string[0]
        });

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> InsertCategory(this CenterHtmlMenu menu, string modulePath, string category, params string[] options)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulePath);
        if (reasonPages == null)
        {
            return false;
        }

        if (reasonPages.Any(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        reasonPages.Add(new AdminMenuPage
        {
            Title = category,
            Reasons = options
        });

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> RemoveCategory(this CenterHtmlMenu menu, string modulePath, string category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulePath);
        if (reasonPages == null)
        {
            return false;
        }

        if (!reasonPages.Any(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        reasonPages.RemoveAll(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase));

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> RemoveReason(this CenterHtmlMenu menu, string modulePath, string category, string reason)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulePath);
        if (reasonPages == null)
        {
            return false;
        }

        if (!reasonPages.Any(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = reasonPages.First(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase));
        if (!page.Reasons.Contains(reason, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Reasons = page.Reasons.Where(x => !string.Equals(x, reason, StringComparison.OrdinalIgnoreCase)).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddReason(this CenterHtmlMenu menu, string modulePath, string category, string reason)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulePath);
        if (reasonPages == null)
        {
            return false;
        }

        if (!reasonPages.Any(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = reasonPages.First(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase));
        if (page.Reasons.Contains(reason, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Reasons = page.Reasons.Append(reason).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddReasons(this CenterHtmlMenu menu, string modulePath, string category, params string[] reasons)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulePath);
        if (reasonPages == null)
        {
            return false;
        }

        if (!reasonPages.Any(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        AdminMenuPage? page = reasonPages.First(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase));
        page.Reasons = page.Reasons.Concat(reasons).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    private static async Task<(List<AdminMenuPage> list, string path)> GetConfig(string modulePath)
    {
        // Get the path to the AdminMenu.json file
        string configPath = Path.Combine(Directory.GetParent(Directory.GetParent(modulePath).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");

        // Deserialize the AdminMenu.json file
        using FileStream fs = new FileStream(configPath, FileMode.Open);
        var document = await JsonDocument.ParseAsync(fs);
        var root = document.RootElement;
        var menuItemsJson = root.GetProperty("MenuItems").GetRawText();
        return (JsonSerializer.Deserialize<List<AdminMenuPage>>(menuItemsJson), configPath);
    }
}

public class AdminMenuPage
{
    public string Category { get; set; }
    public string[] Reasons { get; set; }
}
