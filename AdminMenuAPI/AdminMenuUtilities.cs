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

        reasonPages.Add(new ReasonPage
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

        reasonPages.Add(new ReasonPage
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

        ReasonPage? page = reasonPages.First(x => string.Equals(x.Title, category, StringComparison.OrdinalIgnoreCase));
        page.Reasons = page.Reasons.Concat(reasons).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    private static async Task<(List<ReasonPage> list, string path)> GetConfig(string modulePath)
    {
        // Get the path to the AdminMenu.json file
        string configPath = Path.Combine(Directory.GetParent(Directory.GetParent(modulePath).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");

        // Deserialize the AdminMenu.json file
        return (JsonSerializer.Deserialize<List<ReasonPage>>(await File.ReadAllTextAsync(configPath)), configPath);
    }
}

public class ReasonPage
{
    public string Title { get; set; }
    public string[] Reasons { get; set; }
}
