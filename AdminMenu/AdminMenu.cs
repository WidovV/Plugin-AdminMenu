using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace AdminMenu;

public class AdminMenu : BasePlugin, IPluginConfig<MenuConfig>
{
    public MenuConfig Config { get; set; }

    public void OnConfigParsed(MenuConfig config)
    {
        Config = config;
    }

    public override string ModuleName => "Admin menu";
    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "WidovV";

    public override void Load(bool hotReload)
    {
        foreach (string command in Config.AdminMenuCommands)
        {
            AddCommand(command, "Open admin menu", AdminMenuCommand);
            CommandHelperAttribute attr = 
        }
    }


    [RequiresPermissions($"{Config.AdminMenuFlag}")]
    public void AdminMenuCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }

        if (info.ArgCount > 1)
        {
            player.PrintToChat("Invalid usage of the command. Usage: css_adminmenu");
        }


    }
}
