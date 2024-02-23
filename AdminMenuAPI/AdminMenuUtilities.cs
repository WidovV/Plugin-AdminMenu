using System.Reflection;
using System.Text.Json;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;

namespace AdminMenuAPI;

public partial class AdminMenuUtilities
{
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
        MenuConfig config = new()
        {
            MenuItems = new List<Menu>()
        };
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

                    CategoryNameAttribute category = AdminMenuHelper.GetCategoryName(method) ?? new CategoryNameAttribute("Other");

                    HashSet<string> permissions = AdminMenuHelper.GetPermissions(method);

                    Command command = new() { CommandName = commandName, Flag = permissions?.ToArray() };

                    config.MenuItems.Add(new Menu
                    {
                        Category = category.CategoryName,
                        Commands = new Command[] { command },
                        Flag = category.CategoryFlags ?? Array.Empty<string>() 
                    });

                }
            }

            bool added = await AddCategory(modulePath, config);
            if (!added)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        return true;
    }
}
