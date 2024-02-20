using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
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
        foreach (string command in Config.AdminMenuCommands)
        {
            AddCommand(command, "Open admin menu", AdminMenuCommand);
        }

        RegisterListener<Listeners.OnMapStart>(Listener_OnMapStart);
    }

    private void Listener_OnMapStart(string mapName)
    {
        GetConfig();
    }

    private void GetConfig()
    {
        Task.Run(() =>
        {
            using Stream stream = File.OpenRead(Path.Combine(Directory.GetParent(Directory.GetParent(ModulePath).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json"));
            Config = JsonSerializer.Deserialize<MenuConfig>(stream);
        });
    }

    [ConsoleCommand("css_reloadadminmenu")]
    [RequiresPermissions("css/root")]
    public void ReloadAdminMenuCommand(CCSPlayerController player, CommandInfo info)
    {
        GetConfig();
        if (player == null || !player.IsValid)
        {
            Console.WriteLine("Admin menu reloaded");
            return;
        }

        player.PrintToChat("Admin menu reloaded");
    }

    [RequiresPermissions($"css/generic")]
    public void AdminMenuCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }

        ShowCategoriesMenu(player);
    }

    private void ShowCategoriesMenu(CCSPlayerController player)
    {
        CenterHtmlMenu menu = new("Admin menu");
        foreach (Menu category in Config.MenuItems)
        {
            if (!AdminManager.PlayerHasPermissions(player, category.Flag))
            {
                continue;
            }

            menu.AddMenuOption(category.Category, (_, _) => ShowCommandsMenu(player, category));
        }
    }

    private void ShowCommandsMenu(CCSPlayerController player, Menu menu)
    {
        CenterHtmlMenu commandsMenu = new(menu.Category);
        foreach (string command in menu.Commands)
        {
            if (!AdminManager.PlayerHasPermissions(player, menu.Flag))
            {
                continue;
            }

            commandsMenu.AddMenuOption(command, (_,_) => player.ExecuteClientCommand(command));
        }
    }
}
