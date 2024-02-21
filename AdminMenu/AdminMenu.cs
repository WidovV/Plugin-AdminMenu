using System.Text;
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
    private string _configPath;

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
        _configPath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(ModulePath).FullName).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json");
        Task.Run(async () => await FixConfig());
    }

    private async Task FixConfig()
    {
        string[] lines = await File.ReadAllLinesAsync(_configPath);
        if (!lines[0].Contains('/'))
        {
            return;
        }

        lines = lines[1..];
        MenuConfig config = JsonSerializer.Deserialize<MenuConfig>(string.Join('\n', lines));
        
        await File.WriteAllTextAsync(_configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }

    private void Listener_OnMapStart(string mapName)
    {
        GetConfig();
    }

    private void GetConfig()
    {
        Task.Run(() =>
        {
            using Stream stream = File.OpenRead(Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(ModulePath).FullName).FullName).FullName, "configs", "plugins", "AdminMenu", "AdminMenu.json"));
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
            if (category.Flag != null && category.Flag.Length > 0 && !AdminManager.PlayerHasPermissions(player, category.Flag))
            {
                continue;
            }

            menu.AddMenuOption(category.Category, (_, _) => ShowCommandsMenu(player, category));
        }

        MenuManager.OpenCenterHtmlMenu(this, player, menu);
    }

    private void ShowCommandsMenu(CCSPlayerController player, Menu menu)
    {
        CenterHtmlMenu commandsMenu = new(menu.Category);
        foreach (Command command in menu.Commands)
        {
            if (command.Flag != null && command.Flag.Length > 0 && !AdminManager.PlayerHasPermissions(player, command.Flag))
            {
                continue;
            }

            commandsMenu.AddMenuOption(command.CommandName.Replace("css_", string.Empty), (_,_) => player.ExecuteClientCommand(command.CommandName));
        }

        MenuManager.OpenCenterHtmlMenu(this, player, commandsMenu);
    }
}
