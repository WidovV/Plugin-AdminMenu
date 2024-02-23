using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;

namespace AdminMenuAPI;

public class MenuConfig
{
    public List<Menu> MenuItems { get; set; } = new List<Menu>();

    public string[] AdminMenuCommands { get; set; } = new[]
    {
        "css_admin",
        "css_adminmenu"
    };

    public string AdminMenuFlag { get; set; } = "css/generic";
    public string AdminMenuTitle { get; set; } = "Admin menu";
    public string CommandsMenuTitle { get; set; } = "Commands";

    public static MenuConfig operator +(MenuConfig menuItem, MenuConfig config)
    {
        menuItem.MenuItems.AddRange(config.MenuItems);

        // Check if there's any duplicate items in menuItem.MenuItems
        menuItem.MenuItems = menuItem.MenuItems.GroupBy(x => x.Category, StringComparer.OrdinalIgnoreCase).Select(x => x.First()).ToList();
        // Check if there's any duplicate items in menuItem.MenuItems.Commands
        menuItem.MenuItems.ForEach(x => x.Commands = x.Commands.GroupBy(y => y.CommandName, StringComparer.OrdinalIgnoreCase).Select(y => y.First()).ToArray());
        // Check if there's any duplicate items in menuItem.MenuItems.Flag
        menuItem.MenuItems.ForEach(x => x.Flag = x.Flag.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
        return menuItem;
    }

    public static MenuConfig operator -(MenuConfig menuItem, MenuConfig config)
    {
        foreach (Menu menu in config.MenuItems)
        {
            menuItem.MenuItems.RemoveAll(x => x.Category.Equals(menu.Category, StringComparison.OrdinalIgnoreCase));
        }
        return menuItem;
    }
}

public class Menu
{
    public string Category { get; set; }
    public string[] Flag { get; set; }
    public Command[] Commands { get; set; }
}

public class Command
{
    public string? CommandName { get; set; }
    public string?[]? Flag { get; set; }
}
