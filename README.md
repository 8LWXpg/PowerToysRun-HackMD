# HackMD Plugin for PowerToys Run

A [PowerToys Run](https://aka.ms/PowerToysOverview_PowerToysRun) plugin to open [HackMD](https://hackmd.io) notes.

Checkout the [Template](https://github.com/8LWXpg/PowerToysRun-PluginTemplate) for a starting point to create your own plugin.

## Installation

### Manual

1. Download the latest release of the from the releases page.
2. Extract the zip file's contents to `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`
3. Restart PowerToys.

### Via [ptr](https://github.com/8LWXpg/ptr)

```shell
ptr add HackMD 8LWXpg/PowerToysRun-HackMD
```

## Usage

1. Open PowerToys Run (default shortcut is <kbd>Alt+Space</kbd>).
2. Type `hm`.

## Building

1. Clone the repository and the dependencies in `lib` with `copyLib.ps1`.
2. run `dotnet build -c Release`.

## Debugging

1. Build the project.
2. Run `debug.ps1`.
3. Attach to the process `PowerToys.PowerLauncher`.

## Contributing

### Localization

If you want to help localize this plugin, please check the [localization guide](./Localizing.md)
