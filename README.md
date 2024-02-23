## For [developers](https://github.com/WidovV/Plugin-AdminMenu/tree/main/AdminMenuAPI):
This "api" (Actually just a local dll file that shall be referred to through your .csproj) is containing a small amount of methods to insert and remove items from the adminmenu.json file.
Example usage:
Creating a !ban plugin and want the command to be available in the !admin menu.
Xml that your .csproj *should* contain (<itemgroup> can be removed):
```xml
<ItemGroup>
    <Reference Include="AdminMenuAPI">
        <HintPath>AdminMenuAPI.dll</HintPath>
    </Reference>
</ItemGroup>
```

## For [server owners](https://github.com/WidovV/Plugin-AdminMenu):
This is a base-plugin, so it does nothing except showing whatever is inside the json config.
The config can be edited manually, so removing/adding content *should* be straight forward, that is at least the goal of this project.
The plugin only executes a command without parameters, so commands that requires parameters will not work atm.
The config will look like this as default:
```json
{
  "MenuItems": [],
  "AdminMenuCommands": [
    "css_admin",
    "css_adminmenu"
  ],
  "AdminMenuFlag": "css/generic",
  "AdminMenuTitle": "Admin menu",
  "CommandsMenuTitle": "Commands",
  "ConfigVersion": 1
}
```
* The AdminMenu.json can manually be configured and edited, as an example:
```json
{
    "MenuItems": [
        {
          "Category": "Bans",
          "Flag": [
            "@css/ban",
            "@css/unban"
          ],
          "Commands": [
            {
              "CommandName": "css_unban",
              "Flag": [
                "@css/unban"
              ]
            },
            {
              "CommandName": "css_ban",
              "Flag": [
                "@css/ban"
              ]
            }
          ]
        }
    ],
  "AdminMenuCommands": [
    "css_admin",
    "css_adminmenu"
  ],
  "AdminMenuFlag": "css/generic",
  "AdminMenuTitle": "Admin menu",
  "CommandsMenuTitle": "Commands",
  "ConfigVersion": 1
}
```

### Usage of version 0.1.0:
#### Automatisation:
It's possible to automatically insert commands to the admin menu.
* Registering your class to the AdminMenuAPI (It's not needed to add a timer nor Handle it in a Task):
```cs
public class MyClass : BasePlugin
{
    public override string ModuleName => "Hello command";

    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "WidovV";
    public override void Load(bool hotReload)
    {
        AddTimer(5f, () =>
        {
            Task.Run(async () =>
            {
                await AdminMenuUtilities.RegisterAdminCategories(ModulePath, typeof(MyClass)); // Parsing in type of the class with typeof, it is not needed to run it in a task nor AddTimer  
            });
        });
    }
```
* **[CategoryName("Player")]**: The name of the category the command will be in.
* **[ConsoleCommand("css_hello")]**: The command will be trimmed and only show "hello" in the admin menu.
```cs
    [CategoryName("Player")] // The name of the category the command will be in
    [ConsoleCommand("css_hello")] // The command will be trimmed and only show "hello" in the admin menu
    public void SayHelloCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }
        if (AdminManager.PlayerHasPermissions(player, "@css/generic"))
        {
            player.PrintToChat("Hello admin!");
            return;
        }
        
        player.PrintChat("Hello!");
    }
```
* **[CategoryName("Player", "css/generic")]**: Adding a flag to the category, so admin(s) without the required permission will not be able to see the category in the admin menu.
* **[ConsoleCommand("css_example")]**: The command will be trimmed and only show "hello" in the admin menu.
* **[RequiresPermissions("@css/generic")]**: The command itself will only be shown in the category if the admin has the permission.

```cs
    [CategoryName("Player", "css/generic")] // Adding a flag to the category, so admin(s) without the required permission will not be able to see the category in the admin menu
    [ConsoleCommand("css_example")] // The command will be trimmed and only show "hello" in the admin menu
    [RequiresPermissions("@css/generic")] // The command itself will only be shown in the category if the admin has the permission
    public void SayHelloCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }
        player.PrintChat("You have @css/generic flag!");
    }

```
* **[CategoryName("Player", "css/rcon")]**: Adding a flag to the category, so admin(s) without the required permission will not be able to see the category in the admin menu.
* **[ConsoleCommand("css_perms")]**: The command will be trimmed and only show "hello" in the admin menu.
* **[RequiresPermissionsOr("@css/rcon", "@css/root")]**: The command itself will only be shown in the category if the admin has one of the permissions.
```cs
    [CategoryName("Player", "@css/rcon", "@css/root")] // Adding a flag to the category, so admin(s) without the required permission will not be able to see the category in the admin menu
    [ConsoleCommand("css_perms")] // The command will be trimmed and only show "hello" in the admin menu
    [RequiresPermissionsOr("@css/rcon", "@css/root")] // The command itself will only be shown in the category if the admin has one of the permissions
    public void SayHelloCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }
        player.PrintChat("You have @css/generic flag!");
    }
}
```

#### Full example:
```cs
public class MyClass : BasePlugin
{
    public override string ModuleName => "Hello command";

    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "WidovV";
    public override void Load(bool hotReload)
    {
        AddTimer(5f, () =>
        {
            Task.Run(async () =>
            {
                await AdminMenuUtilities.RegisterAdminCategories(ModulePath, typeof(MyClass)); // Parsing in type of the class with typeof, it is not needed to run it in a task nor AddTimer  
            });
        });
    }

 [CategoryName("Player")] // The name of the category the command will be in
 [ConsoleCommand("css_hello")] // The command will be trimmed and only show "hello" in the admin menu
 public void SayHelloCommand(CCSPlayerController player, CommandInfo info)
 {
    if (player == null || !player.IsValid || player.IsBot)
    {
        return;
    }
    if (AdminManager.PlayerHasPermissions(player, "@css/generic"))
    {
        player.PrintToChat("Hello admin!");
        return;
    }
    
    player.PrintChat("Hello!");
 }
}

    [CategoryName("Player", "css/generic")] // Adding a flag to the category, so admin(s) without the required permission will not be able to see the category in the admin menu
    [ConsoleCommand("css_example")] // The command will be trimmed and only show "hello" in the admin menu
    [RequiresPermissions("@css/generic")] // The command itself will only be shown in the category if the admin has the permission
    public void SayHelloCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }
        player.PrintChat("You have @css/generic flag!");
    }

    [CategoryName("Player", "@css/rcon", "@css/root")] // Adding a flag to the category, so admin(s) without the required permission will not be able to see the category in the admin menu
    [ConsoleCommand("css_perms")] // The command will be trimmed and only show "hello" in the admin menu
    [RequiresPermissionsOr("@css/rcon", "@css/root")] // The command itself will only be shown in the category if the admin has one of the permissions
    public void SayHelloCommand(CCSPlayerController player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot)
        {
            return;
        }
        player.PrintChat("You have @css/generic flag!");
    }
}
```
Manually adding commands:
* Available methods (they all return a boolean value showing if the action was successful):
```cs
AddCategory(string modulePath, CategoryNameAttribute category, params Command[] commands)
AddCommandToCategory(string modulePath, CategoryNameAttribute category, params Command[] commands)
AddFlagToCategory(string modulePath, CategoryNameAttribute category)
AddFlagToCommand(string modulePath, CategoryNameAttribute category, string commandName, params string[] flag)

RemoveCategory(string modulePath, CategoryNameAttribute category)
RemoveCategory(string modulePath, string category)
RemoveCommand(string modulePath, CategoryNameAttribute category, string command, string[] flags)
RemoveCommand(string modulePath, string category, string command, string[] flags)
RemoveFlagFromCategory(string modulePath, string category, string flag)
RemoveFlagFromCategory(string modulePath, CategoryNameAttribute category, string flag)
RemoveFlagFromCommand(string modulepath, CategoryNameAttribute category, string commandName, string flag)
```
* **CategoryNameAttribute**: The object that handles the category and it's permission(s)
* **Command**: The object that handles the command and it's permission(s)
```cs
    [ConsoleCommand("css_command")]
    public void TestCommand(CCSPlayerController player, CommandInfo info)
    {
        CategoryNameAttribute categoryName = new CategoryNameAttribute("fun"); // Example without flags in the category
        CategoryNameAttribute categoryName1 = new CategoryNameAttribute("fun", "@css/root"); // Example with flag in the category
        Command cmd = new()
        {
            CommandName = "css_command",
            Flag = new string[] { "@css/root", "@css/generic" }
        };

        bool value = AdminMenuUtilities.AddCategory(ModulePath, categoryName1, cmd);
        Console.WriteLine($"{categoryName1.CategoryName} added: {value}");
    }
```
### The reason for the plugin:
This plugin/extension is designed for those who have a separate admin system and want cross-support through different plugins to the same admin menu.

Future plan(s):

- [ ] Fix changing flag for the command, perhaps make it an array. It's currently hardcoded but still has the option in the config.
- [x] Make the plugin fetch data on mapchange.
- [x] Make the plugin fetch data with a command.
- [ ] Support parameter(s) (would probably be done by adding more items to the config)
- [ ] Test/support html colored text
- [ ] Support chat menus
- [x] Add Admin menu & commands menu title to config 
- [ ] Create a wiki
- [ ] Release as a NuGet package
