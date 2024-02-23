namespace AdminMenuAPI;

public partial class AdminMenuUtilities
{
    public static async Task<bool> AddCategory(string modulePath, CategoryNameAttribute category, params Command[] commands)
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

        if (categoryPages.Any(x => string.Equals(x.Category, category.CategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            bool commandAdded = await AddCommandToCategory(modulePath, category, menuItem, commands);
            bool flagAdded = await AddFlagToCategory(modulePath, category, menuItem);
            return commandAdded || flagAdded;
        }

        category.CategoryFlags ??= Array.Empty<string>();

        categoryPages.Add(new Menu
        {
            Category = category.CategoryName,
            Commands = commands,
            Flag = category.CategoryFlags // Consider adding a flags parameter to this method if you want to set flags when adding a category
        });

        await AdminMenuHelper.UpdateConfig(menuItem, configPath, categoryPages);
        return true;
    }

    public static async Task<bool> AddCategory(string modulePath, MenuConfig config)
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

        // First time actually using operator overloading in a project, lol
        menuItem += config;

        await AdminMenuHelper.UpdateConfig(menuItem, configPath);
        return true;
    }

    public static async Task<bool> AddCommandToCategory(string modulePath, CategoryNameAttribute category, MenuConfig menuItem, params Command[] commands)
    {

        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        if (!modulePath.EndsWith("AdminMenu.json", StringComparison.OrdinalIgnoreCase))
        {
            modulePath = AdminMenuHelper.GetConfigPath(modulePath);
        }

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
            await AdminMenuHelper.UpdateConfig(menuItem, modulePath, categoryPages);
        }

        return commandAdded;
    }

    public static async Task<bool> AddCommandToCategory(string modulePath, CategoryNameAttribute category, params Command[] commands)
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
    public static async Task<bool> AddFlagToCategory(string modulePath, CategoryNameAttribute category)
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

    public static async Task<bool> AddFlagToCategory(string modulePath, CategoryNameAttribute category, MenuConfig menuItem)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        if (!modulePath.EndsWith("AdminMenu.json", StringComparison.OrdinalIgnoreCase))
        {
            modulePath = AdminMenuHelper.GetConfigPath(modulePath);
        }
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
            await AdminMenuHelper.UpdateConfig(menuItem, modulePath, categoryPages);
        }

        return flagAdded;
    }

    public static async Task<bool> AddFlagToCommand(string modulePath, CategoryNameAttribute category, string commandName, params string[] flag)
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

    public static async Task<bool> AddFlagToCommand(string modulePath, CategoryNameAttribute category, string commandName, MenuConfig menuItem, params string[] flag)
    {
        if (string.IsNullOrEmpty(modulePath))
        {
            return false;
        }

        if (!modulePath.EndsWith("AdminMenu.json", StringComparison.OrdinalIgnoreCase))
        {
            modulePath = AdminMenuHelper.GetConfigPath(modulePath);
        }

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
            await AdminMenuHelper.UpdateConfig(menuItem, modulePath, categoryPages);
        }

        return flagAdded;
    }
}
