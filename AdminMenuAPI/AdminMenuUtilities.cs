using System.Text.Json;

namespace AdminMenuAPI;

public static class AdminMenuUtilities
{
    public static async Task<bool> AddCategory(string modulePath, string category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();

        if (categoryPages == null)
        {
            return false;
        }

        if (categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        categoryPages.Add(new Menu
        {
            Category = category,
            Commands = new string[0],
            Flag = new string[0]
        });

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCategory(string modulePath, string category, params string[] commands)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();
        if (categoryPages == null)
        {
            return false;
        }


        if (categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return await AddCommand(modulePath, category, commands);
        }

        categoryPages.Add(new Menu
        {
            Category = category,
            Commands = commands,
            Flag = new string[0]
        });

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCategory(string modulepath, string category, string[] command, params string[] flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulepath);
        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();

        if (categoryPages == null)
        {
            return false;
        }

        if (categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return await AddCommand(modulepath, category, command) && await AddFlag(modulepath, category, flag);
        }

        categoryPages.Add(new Menu
        {
            Category = category,
            Commands = command,
            Flag = flag
        });

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }


    public static async Task<bool> RemoveCategory(string modulePath, string category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();
        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        categoryPages.RemoveAll(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> RemoveCommand(string modulePath, string category, string command)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();
        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = categoryPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (!page.Commands.Contains(command, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Commands = page.Commands.Where(x => !string.Equals(x, command, StringComparison.OrdinalIgnoreCase)).ToArray();

        categoryPages[categoryPages.FindIndex(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase))] = page;

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCommand(string modulePath, string category, string command)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);
        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();
        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = categoryPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (page.Commands.Contains(command, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Commands = page.Commands.Append(command).ToArray();

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddFlag(string modulepath, string category, params string[] flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulepath);
        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();
        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = categoryPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
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
            await UpdateConfig(menuItem, configPath, categoryPages);
        }

        return flagAdded;
    }

    public static async Task<bool> RemoveFlag(string modulepath, string category, string flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulepath);
        if (menuItem == null)
        {
            return false;
        }

        List<Menu> categoryPages = menuItem.MenuItems.ToList();
        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var page = categoryPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (!page.Flag.Contains(flag, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Flag = page.Flag.Where(x => !string.Equals(x, flag, StringComparison.OrdinalIgnoreCase)).ToArray();

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCommand(string modulePath, string category, params string[] commands)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);
        if (menuItem == null)
        {
            return false;
        }
        List<Menu> categoryPages = menuItem.MenuItems.ToList();

        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Menu? page = categoryPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        page.Commands = page.Commands.Concat(commands).ToArray();

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    private static async Task UpdateConfig(MenuConfig menuItem, string configPath, List<Menu> categoryPages)
    {
        menuItem.MenuItems = categoryPages.ToArray();
        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(menuItem, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static async Task<(MenuConfig menu, string path)> GetConfig(string modulePath)
    {
        // Get the path to the AdminMenu.json file
        string configPath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(modulePath).FullName).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");

        // Deserialize the AdminMenu.json file
        return (JsonSerializer.Deserialize<MenuConfig>(await File.ReadAllTextAsync(configPath)), configPath);
    }
}
