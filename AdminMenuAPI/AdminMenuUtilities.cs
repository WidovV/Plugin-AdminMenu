using System.Reflection;
using System.Text.Json;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;

namespace AdminMenuAPI;

public static class AdminMenuUtilities
{
    public static async Task<bool> AddCategory(string modulePath, CategoryNameAttribute category, params Command[] commands)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        if (categoryPages == null)
        {
            return false;
        }

        if (categoryPages.Any(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            return await AddFlagToCategory(modulePath, category) && await AddCommand(modulePath, category, commands);
        }

        if (category.CategoryFlags == null)
        {
            category.CategoryFlags = Array.Empty<string>();
        }

        categoryPages.Add(new Menu
        {
            Category = category.CategoryName,
            Commands = commands,
            Flag = category.CategoryFlags // Consider adding a flags parameter to this method if you want to set flags when adding a category
        });

        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCommand(string modulePath, CategoryNameAttribute category, params Command[] commands)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        Menu? page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));
        if (page == null)
        {
            return false;
        }

        // Check for existing commands with the same name and avoid adding duplicates
        bool commandAdded = false;
        foreach (Command command in commands)
        {
            if (!page.Commands.Any(c => c.CommandName.Equals(command.CommandName, StringComparison.OrdinalIgnoreCase)))
            {
                page.Commands = page.Commands.Concat(new[] { command }).ToArray();
                commandAdded = true;
            }
        }

        if (commandAdded)
        {
            categoryPages = categoryPages.Select(x => x.Category == category.CategoryName ? page : x).ToList();
            await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        }

        return commandAdded;
    }

    public static async Task<bool> RemoveCategory(string modulePath, CategoryNameAttribute category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;
        if (categoryPages == null)
        {
            return false;
        }

        if (!categoryPages.Any(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        categoryPages.RemoveAll(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));

        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> RemoveCommand(string modulePath, CategoryNameAttribute category, string command, string[] flags)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        var page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));
        if (page == null)
        {
            return false;
        }

        // Identify the exact command to remove based on its name and flags
        page.Commands = page.Commands.Where(c => !(c.CommandName.Equals(command, StringComparison.OrdinalIgnoreCase) && c.Flag.SequenceEqual(flags))).ToArray();

        categoryPages = categoryPages.Select(x => x.Category == category.CategoryName ? page : x).ToList();
        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddFlagToCategory(string modulePath, CategoryNameAttribute category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        if (categoryPages.Any(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Menu page = categoryPages.First(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));

        bool flagAdded = false;
        foreach (var f in category.CategoryFlags)
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
            categoryPages = categoryPages.Select(x => x.Category == category.CategoryName ? page : x).ToList();
            await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        }

        return flagAdded;
    }

    public static async Task<bool> AddFlagToCommand(string modulePath, CategoryNameAttribute category, string commandName, params string[] flag)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        var page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));
        if (page == null)
        {
            return false;
        }

        var command = page.Commands.FirstOrDefault(c => c.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase));
        if (command == null)
        {
            return false;
        }

        bool flagAdded = false;
        foreach (var f in flag)
        {
            if (command.Flag.Contains(f, StringComparer.OrdinalIgnoreCase) || string.IsNullOrEmpty(f))
            {
                continue;
            }

            command.Flag = command.Flag.Append(f).ToArray();
            flagAdded = true;
        }

        if (flagAdded)
        {
            await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        }

        return flagAdded;
    }

    public static async Task<bool> RemoveFlagFromCategory(string modulePath, string category, string flag)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        if (!categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Menu page = categoryPages.First(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));

        if (!page.Flag.Contains(flag, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Flag = page.Flag.Where(x => !string.Equals(x, flag, StringComparison.OrdinalIgnoreCase)).ToArray();

        categoryPages = categoryPages.Select(x => x.Category == category ? page : x).ToList();
        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> RemoveFlagFromCategory(string modulePath, CategoryNameAttribute category, string flag)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulePath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        if (!categoryPages.Any(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        Menu page = categoryPages.First(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));

        if (!page.Flag.Contains(flag, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        page.Flag = page.Flag.Where(x => !string.Equals(x, flag, StringComparison.OrdinalIgnoreCase)).ToArray();
        categoryPages = categoryPages.Select(x => x.Category == category.CategoryName ? page : x).ToList();

        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> RemoveFlagFromCommand(string modulepath, CategoryNameAttribute category, string commandName, string flag)
    {
        if (string.IsNullOrEmpty(modulepath))
        {
            return false;
        }

        var (menuItem, configPath) = await GetConfig(modulepath);

        menuItem ??= new MenuConfig
        {
            MenuItems = new List<Menu>()
        };

        List<Menu> categoryPages = menuItem.MenuItems;

        var page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase));
        if (page == null)
        {
            return false;
        }

        var command = page.Commands.FirstOrDefault(c => c.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase));
        if (command == null || !command.Flag.Contains(flag, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        command.Flag = command.Flag.Where(x => !string.Equals(x, flag, StringComparison.OrdinalIgnoreCase)).ToArray();

        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> RegisterAdminCategories(string modulePath, params Type[] classtype)
    {
        if (classtype.Length == 0)
        {
            return false;
        }

        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        try
        {
            // Get the type of the class
            foreach (Type type in classtype)
            {
                
                // Foreach each method in the class
                foreach (MethodInfo method in type.GetMethods())
                {
                    // Only add first attribute from the method that is a ConsoleCommandAttribute
                    string commandName = AdminMenuHelper.GetCommandName(method);

                    // If the command name is null or empty, skip the method
                    if (string.IsNullOrEmpty(commandName))
                    {
                        continue;
                    }

                    CategoryNameAttribute categoryName = AdminMenuHelper.GetCategoryName(method) ?? new CategoryNameAttribute("Other");

                    HashSet<string> permissions = AdminMenuHelper.GetPermissions(method);

                    Command command = new() { CommandName = commandName, Flag = permissions?.ToArray() };

                    if (permissions != null && permissions.Count > 0)
                    {
                        Console.WriteLine($"Adding command {commandName} to category {categoryName} with permissions {string.Join(", ", permissions)}");
                        bool added = await AddCategory(modulePath, categoryName, command);
                        Console.WriteLine($"Added: {added}");
                        continue;
                    }

                    Console.WriteLine($"Adding command {commandName} to category {categoryName}");
                    bool value = await AddCategory(modulePath, categoryName, command);
                    Console.WriteLine($"Added: {value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        return true;
    }

    private static async Task<(MenuConfig menu, string path)> GetConfig(string modulePath)
    {
        // Get the path to the AdminMenu.json file
        string configPath = AdminMenuHelper.GetConfigPath(modulePath);

        try
        {
            return (JsonSerializer.Deserialize<MenuConfig>(await File.ReadAllTextAsync(configPath)), configPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}
