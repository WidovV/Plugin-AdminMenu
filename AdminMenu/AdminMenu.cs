using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace AdminMenu;

public class AdminMenu : BasePlugin
{
    public override string ModuleName => "Admin menu";
    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "WidovV";


    [ConsoleCommand("css_adminmenu", "Open the admin menu")]
    [ConsoleCommand("css_admin")]
    [RequiresPermissions("css/generic")]
    public void AdminMenuCommand(CCSPlayerController player, CommandInfo info)
    {

    }
}
