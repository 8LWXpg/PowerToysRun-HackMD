# HackMD Plugin for PowerToys Run

A [PowerToys Run](https://aka.ms/PowerToysOverview_PowerToysRun) plugin for opening [HackMD](https://hackmd.io) notes.

Checkout the [Template](https://github.com/8LWXpg/PowerToysRun-PluginTemplate) for a starting point to create your own plugin.

## Installation

### Manual

1. Download the latest release of the from the releases page.
1. Extract the zip file's contents to `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`
1. Restart PowerToys.

### Via [ptr](https://github.com/8LWXpg/ptr)

```shell
ptr add HackMD 8LWXpg/PowerToysRun-HackMD
```

## Usage

1. Generate a API token in [settings](https://hackmd.io/settings#api)
1. Add the token to the plugin settings.
1. Open PowerToys Run (default shortcut is <kbd>Alt+Space</kbd>).
1. Type `hm`.

## Building

1. Clone the repository and the dependencies in `lib` with `copyLib.ps1`.
1. run `dotnet build -c Release`.

## Debugging

1. Build the project.
1. Run `debug.ps1`.
1. Attach to the process `PowerToys.PowerLauncher`.

## Contributing

### Localization

If you want to help localize this plugin, please check the [localization guide](./Localizing.md)
