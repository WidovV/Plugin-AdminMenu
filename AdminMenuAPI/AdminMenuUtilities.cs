using System.Linq;
using System.Text.Json;
using CounterStrikeSharp.API.Modules.Menu;

namespace AdminMenuAPI;

public static class AdminMenuUtilities
{
    public static async Task<bool> AddCategory(string modulePath, string category)
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

        if (reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        reasonPages.Add(new Menu
        {
            Category = category,
            Commands = new string[0],
            Flag = new string[0]
        });

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddCategory(string modulePath, string category, params string[] commands)
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

        if (reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return await AddCommand(modulePath, category, commands);
        }

        reasonPages.Add(new Menu
        {
            Category = category,
            Commands = commands,
            Flag = new string[0]
        });

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddCategory(string modulepath, string category, string[] command, params string[] flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulepath);
        if (reasonPages == null)
        {
            return false;
        }

        if (reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return await AddCommand(modulepath, category, command) && await AddFlag(modulepath, category, flag);
        }

        reasonPages.Add(new Menu
        {
            Category = category,
            Commands = command,
            Flag = flag
        });

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }


    public static async Task<bool> RemoveCategory(string modulePath, string category)
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

        if (!reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        reasonPages.RemoveAll(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> RemoveCommand(string modulePath, string category, string command)
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

        if (!reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = reasonPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (!page.Commands.Contains(command, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Commands = page.Commands.Where(x => !string.Equals(x, command, StringComparison.OrdinalIgnoreCase)).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddCommand(string modulePath, string category, string command)
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

        if (!reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = reasonPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (page.Commands.Contains(command, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Commands = page.Commands.Append(command).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddFlag(string modulepath, string category, params string[] flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulepath);
        if (reasonPages == null)
        {
            return false;
        }

        if (!reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = reasonPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        bool flagAdded = false;
        foreach (var f in flag)
        {
            if (page.Flag.Contains(f, StringComparer.OrdinalIgnoreCase) || string.IsNullOrEmpty(f))
            {
                continue;
            }

            page.Flag = page.Flag.Append(f).ToArray();
            flagAdded = true;
        }

        if (flagAdded)
        {
            await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));

        }

        return flagAdded;
    }

    public static async Task<bool> RemoveFlag(string modulepath, string category, string flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (reasonPages, configPath) = await GetConfig(modulepath);
        if (reasonPages == null)
        {
            return false;
        }

        if (!reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = reasonPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (!page.Flag.Contains(flag, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Flag = page.Flag.Where(x => !string.Equals(x, flag, StringComparison.OrdinalIgnoreCase)).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    public static async Task<bool> AddCommand(string modulePath, string category, params string[] commands)
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

        if (!reasonPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Menu? page = reasonPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        page.Commands = page.Commands.Concat(commands).ToArray();

        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(reasonPages, new JsonSerializerOptions { WriteIndented = true }));
        return true;
    }

    private static async Task<(List<Menu> list, string path)> GetConfig(string modulePath)
    {
        // Get the path to the AdminMenu.json file
        string configPath = Path.Combine(Directory.GetParent(Directory.GetParent(modulePath).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");

        // Deserialize the AdminMenu.json file
        return (JsonSerializer.Deserialize<MenuConfig>(await File.ReadAllTextAsync(configPath)).MenuItems.ToList(), configPath);
    }
}
