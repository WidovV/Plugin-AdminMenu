using CounterStrikeSharp.API.Core;

namespace AdminMenu;

public class MenuConfig : BasePluginConfig
{
    public Menu[] MenuItems { get; set; } = new[]
    {
        new Menu
        {
            Flag = "css/generic",
            Category = "Player commands",
            Commands = new[]
            {
                "css_kick"
            }
        },
        new Menu
        {
            Flag = "css/generic",
            Category = "Fun commands",
            Commands = new[]
            {
                "css_slap",
            }
        }
    };

    public string[] AdminMenuCommands { get; set; } = new[]
    {
        "css_admin",
        "css_adminmenu"
    };

    public string AdminMenuFlag { get; set; } = "css/generic";
    public string AdminMenuTitle { get; set; } = "Admin menu";
    public string CommandsMenuTitle { get; set; } = "Commands";
}

public class Menu
{
    public string Category { get; set; }
    public string Flag { get; set; }
    public string[] Commands { get; set; }
}