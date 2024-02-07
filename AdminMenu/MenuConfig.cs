using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;

namespace AdminMenu;

public class MenuConfig : BasePluginConfig
{
    public Menu[] MenuItems { get; set; } = new[]
    {
        new Menu
        {
            Title = "Player commands",
            Commands = new[]
            {
                "css_kick"
            }
        }
    };

    public string[] AdminMenuCommands { get; set; } = new[]
    {
        "css_admin",
        "css_adminmenu"
    };

    public string AdminMenuFlag { get; set; } = "css/generic";
}

public class Menu
{
    public string Title { get; set; }
    public string[] Commands { get; set; }
}