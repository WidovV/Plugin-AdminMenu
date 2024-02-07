using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;

namespace AdminMenu;

public class AdminMenu : BasePlugin, IPluginConfig<MenuConfig>
{
    public override string ModuleName => "Admin menu";
    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "WidovV";
    public MenuConfig Config { get; set; }

    public void OnConfigParsed(MenuConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {

        // RegisterListener<Listeners.OnMapStart>(Listener_OnMapStart); // Commented out because i can't remember if i should use a list or dictionary for deserialize
        foreach (string command in Config.AdminMenuCommands)
        {
            AddCommand(command, "Open admin menu", AdminMenuCommand);
        }
    }

    [RequiresPermissions($"css/generic")]
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

        ShowCategoriesMenu(player);
    }

    private void ShowCategoriesMenu(CCSPlayerController player)
    {
        CenterHtmlMenu menu = new("Admin menu");
        foreach (Menu reason in Config.MenuItems)
        {
            menu.AddMenuOption(reason.Category, (_, _) => ShowCommandsMenu(player, reason));
        }
    }

    private void ShowCommandsMenu(CCSPlayerController player, Menu menu)
    {
        CenterHtmlMenu commandsMenu = new(menu.Category);
        foreach (string command in menu.Commands)
        {
            commandsMenu.AddMenuOption(command, (_,_) => player.ExecuteClientCommand(command));
        }
    }
}
