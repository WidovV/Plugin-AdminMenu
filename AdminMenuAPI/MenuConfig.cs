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
}

public class Menu
{
    public string Category { get; set; }
    public string[] Flag { get; set; }
    public Command[] Commands { get; set; }
}

public class Command
{
    public string CommandName { get; set; }
    public string[] Flag { get; set; }
}