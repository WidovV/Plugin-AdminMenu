using System.Reflection;
using System.Text.Json;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;

namespace AdminMenuAPI;

public static class AdminMenuUtilities
{
    public static async Task<bool> AddCategory(string modulePath, string category, params Command[] commands)
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

        List<Menu> categoryPages = menuItem.MenuItems;

        if (categoryPages.Any(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase)))
        {
            return await AddCommand(modulePath, category, commands);
        }

        categoryPages.Add(new Menu
        {
            Category = category,
            Commands = commands,
            Flag = new string[0] // Consider adding a flags parameter to this method if you want to set flags when adding a category
        });

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCommand(string modulePath, string category, params Command[] commands)
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
        List<Menu> categoryPages = menuItem.MenuItems;

        Menu? page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (page == null)
        {
            return false;
        }

        // Check for existing commands with the same name and avoid adding duplicates
        foreach (Command command in commands)
        {
            if (!page.Commands.Any(c => c.CommandName.Equals(command.CommandName, StringComparison.OrdinalIgnoreCase)))
            {
                page.Commands = page.Commands.Concat(new[] { command }).ToArray();
            }
        }

        categoryPages = categoryPages.Select(x => x.Category == category ? page : x).ToList();
        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    private static async Task UpdateConfig(MenuConfig menuItem, string configPath, List<Menu> categoryPages)
    {
        menuItem.MenuItems = categoryPages;
        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(menuItem, new JsonSerializerOptions { WriteIndented = true }));
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

        List<Menu> categoryPages = menuItem.MenuItems;
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

    public static async Task<bool> RemoveCommand(string modulePath, string category, string command, string[] flags)
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

        List<Menu> categoryPages = menuItem.MenuItems;

        var page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        if (page == null)
        {
            return false;
        }

        // Identify the exact command to remove based on its name and flags
        page.Commands = page.Commands.Where(c => !(c.CommandName.Equals(command, StringComparison.OrdinalIgnoreCase) && c.Flag.SequenceEqual(flags))).ToArray();

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddFlag(string modulepath, string category, string commandName, params string[] flag)
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

        List<Menu> categoryPages = menuItem.MenuItems;

        var page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
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
            await UpdateConfig(menuItem, configPath, categoryPages);
        }

        return flagAdded;
    }

    public static async Task<bool> RemoveFlag(string modulepath, string category, string commandName, string flag)
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

        List<Menu> categoryPages = menuItem.MenuItems;

        var page = categoryPages.FirstOrDefault(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
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

        await UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    private static async Task<(MenuConfig menu, string path)> GetConfig(string modulePath)
    {
        // Get the path to the AdminMenu.json file
        string configPath = GetConfigPath(modulePath);

        // Deserialize the AdminMenu.json file
        return (JsonSerializer.Deserialize<MenuConfig>(await File.ReadAllTextAsync(configPath)), configPath);
    }

    private static string GetConfigPath(string modulePath) => Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(modulePath).FullName).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");

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

        modulePath = GetConfigPath(modulePath);

        // Get the type of the class
        foreach (Type type in classtype)
        {
            // Foreach each method in the class
            foreach (MethodInfo method in type.GetMethods())
            {
                // Only add first attribute from the method that is a ConsoleCommandAttribute
                string commandName = GetCommandName(method);

                // If the command name is null or empty, skip the method
                if (string.IsNullOrEmpty(commandName))
                {
                    continue;
                }

                string categoryName = GetCategoryName(method);

                if (string.IsNullOrEmpty(categoryName))
                {
                    categoryName = "Other";
                }

                HashSet<string> permissions = GetPermissions(method);

                Command command = new() { CommandName = commandName, Flag = permissions.ToArray() };

                if (permissions != null && permissions.Count > 0)
                {
                    Console.WriteLine($"Adding command {commandName} to category {categoryName} with permissions {string.Join(", ", permissions)}");
                    bool added = await AddCategory(modulePath, category: categoryName, command);
                    Console.WriteLine($"Added: {added}");
                    continue;
                }

                Console.WriteLine($"Adding command {commandName} to category {categoryName}");
                bool value = await AddCategory(modulePath, categoryName, command);
                Console.WriteLine($"Added: {value}");
            }
        }

        return true;
    }

    private static string GetCommandName(MethodInfo method)
    {
        // Get the first attribute from the method that is a ConsoleCommandAttribute
        object? attribute = method.GetCustomAttributes(typeof(ConsoleCommandAttribute), false).FirstOrDefault();

        // If the attribute is null, return null
        if (attribute == null)
        {
            return null;
        }

        // Get the command name from the attribute
        string commandName = attribute switch
        {
            ConsoleCommandAttribute cca => cca.Command,
            _ => null
        };

        // Return the command name or null
        return commandName;
    }

    private static string GetCategoryName(MethodInfo method)
    {
        object? attribute = method.GetCustomAttribute(typeof(CategoryNameAttribute), false);

        string categoryName = attribute switch
        {
            CategoryNameAttribute cna => cna.CategoryName,
            _ => null
        };

        return categoryName;
    }

    private static HashSet<string> GetPermissions(MethodInfo method)
    {
        // Get the first attribute from the method that is a RequiresPermissions or RequiresPermissionsOr attribute
        object? permissionAttribute = method.GetCustomAttribute(typeof(RequiresPermissions), false) ?? method.GetCustomAttribute(typeof(RequiresPermissionsOr), false);

        HashSet<string> permissions = permissionAttribute switch
        {
            RequiresPermissions rp => rp.Permissions,
            RequiresPermissionsOr rpo => rpo.Permissions,
            _ => null
        };

        return permissions;
    }
}
