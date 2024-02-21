using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMenuAPI;

public partial class AdminMenuUtilities
{
    public static async Task<bool> RemoveCategory(string modulePath, CategoryNameAttribute category)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await AdminMenuHelper.GetConfig(modulePath);

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

        var (menuItem, configPath) = await AdminMenuHelper.GetConfig(modulePath);

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

    public static async Task<bool> RemoveFlagFromCategory(string modulePath, string category, string flag)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        var (menuItem, configPath) = await AdminMenuHelper.GetConfig(modulePath);

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

        var (menuItem, configPath) = await AdminMenuHelper.GetConfig(modulePath);

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

        var (menuItem, configPath) = await AdminMenuHelper.GetConfig(modulepath);

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
}
