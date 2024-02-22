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
                        Console.WriteLine($"Adding command {commandName} to category {categoryName.CategoryName} with permissions {string.Join(", ", permissions)}");
                        bool added = await AddCategory(modulePath, categoryName, command);
                        Console.WriteLine($"Added: {added}");
                        continue;
                    }

                    Console.WriteLine($"Adding command {commandName} to category {categoryName.CategoryName}");
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
}
