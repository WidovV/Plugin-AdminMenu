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
The config will look like this ish:
```json
{
  "MenuItems": [
    {
      "Title": "Admin commands",
       "Commands": [
          "css_kick"
          ]
    },
  "MenuItems": [
    {
      "Title": "Fun commands",
       "Commands": [
          "css_slap"
          ]
    }
  "AdminMenuCommands": [
    "css_admin",
    "css_adminmenu"
    ],
    "AdminMenuFlag": "css/generic"
}
```
### The reason for the plugin:
For those who have a seperate admin system and wants cross-support through different plugins to the same admin menu, this can be used.

### *Note:*
This was developed while cs# and metamod did not work, so currently untested, but every kind of contribution is welcome.
A release will be created once I can test it, publishing now for the contribution/guidance part.

Future plan(s):
```markdown
- [ ] Fix changing flag for the command, perhaps make it an array. It's currently hardcoded but still has the option in the config.
- [ ] Make the plugin fetch data on mapchange.
- [ ] Make the plugin fetch data with a command.
- [ ] Support parameter(s) (would probably be done by adding more items to the config)
- [ ] Test/support html colored text
- [ ] Support chat menus
- [x] Add Admin menu & commands menu title to config 
```