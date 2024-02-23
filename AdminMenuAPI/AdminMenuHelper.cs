using System.Reflection;
using System.Text.Json;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;

namespace AdminMenuAPI;

public partial class AdminMenuHelper
{
    internal static async Task<(MenuConfig menu, string path)> GetConfig(string modulePath)
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
    internal static async Task UpdateConfig(MenuConfig menuItem, string configPath, List<Menu> categoryPages)
    {
        menuItem.MenuItems = categoryPages;
        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(menuItem, new JsonSerializerOptions { WriteIndented = true }));
    }
    internal static string GetConfigPath(string modulePath) => Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(modulePath).FullName).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");

    public static string GetCommandName(MethodInfo method)
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

    public static CategoryNameAttribute GetCategoryName(MethodInfo method)
    {
        object? attribute = method.GetCustomAttribute(typeof(CategoryNameAttribute), false);

        if (attribute == null)
        {
            return null;
        }

        CategoryNameAttribute? categoryName = attribute as CategoryNameAttribute;

        return categoryName;
    }

    public static HashSet<string> GetPermissions(MethodInfo method)
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
