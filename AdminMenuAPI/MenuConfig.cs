﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;

namespace AdminMenuAPI;

public class MenuConfig
{
    public Menu[] MenuItems { get; set; }
    public string[] AdminMenuCommands { get; set; }

    public string AdminMenuFlag { get; set; }
    public string AdminMenuTitle { get; set; }
    public string CommandsMenuTitle { get; set; }
}

public class Menu
{
    public string Category { get; set; }
    public string[] Commands { get; set; }
}