using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;

namespace AdminMenu;

internal class MenuConfig : BasePluginConfig
{
    public Menu[] MenuItems { get; set; } = new[]
    {
        new Menu
        {
            Title = "Player commands",
            Commands = new[]
            {
                "css_kick",

            }
        }
    };
}

public class Menu
{
    public string Title { get; set; }
    public string[] Commands { get; set; }
}