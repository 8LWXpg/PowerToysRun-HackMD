using Community.PowerToys.Run.Plugin.HackMD.Helpers;
using Community.PowerToys.Run.Plugin.HackMD.Properties;
using LazyCache;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wox.Infrastructure;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.HackMD;
public class Main : IPlugin, IPluginI18n, ISettingProvider, IContextMenu, IReloadable, IDisposable
{
	private const string AuthToken = nameof(AuthToken);
	private const string ViewMode = nameof(ViewMode);

	private string? _authToken;
	private int? _viewMode;
	private CachingService? _cache;
	private PluginInitContext? _context;
	private string? _iconPath;
	private bool _disposed;
	public string Name => Resources.plugin_name;
	public string Description => Resources.plugin_description;
	public static string PluginID => "7ce12aff470442158be4b7c5d02ef63e";

	private record ResultData(Note Note);

	public IEnumerable<PluginAdditionalOption> AdditionalOptions =>
	[
		new()
		{
			PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
			Key = AuthToken,
			DisplayLabel = Resources.setting_api_token,
		},
		new()
		{
			PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
			Key = ViewMode,
			DisplayLabel = Resources.setting_view_mode,
			DisplayDescription = Resources.setting_view_mode_desc,
			ComboBoxItems =
			[
				new(Resources.setting_view_mode_view, "0"),
				new(Resources.setting_view_mode_both, "1"),
				new(Resources.setting_view_mode_edit, "2"),
			],
			ComboBoxValue = 1,
		}
	];

	public void UpdateSettings(PowerLauncherPluginSettings settings)
	{
		_authToken = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == AuthToken)?.TextValue ?? string.Empty;
		NotesHelper.UpdateAuthToken(_authToken);
		_viewMode = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == ViewMode)?.ComboBoxValue ?? 1;
	}

	public List<Result> Query(Query query)
	{
		if (string.IsNullOrEmpty(_authToken))
		{
			return
			[
				new Result
				{
					Title = Resources.error_no_auth_token,
					SubTitle = Resources.error_no_auth_token_desc,
					IcoPath = _iconPath,
				},
			];
		}

		IEnumerable<Note> notes = _cache.GetOrAdd("notes", NotesHelper.GetAllNotes);
		var results = notes.Select(note =>
		{
			MatchResult matches = StringMatcher.FuzzySearch(query.Search, note.Title);
			return new Result
			{
				Title = note.Title,
				IcoPath = _iconPath,
				Score = matches.Score,
				TitleHighlightData = matches.MatchData,
				ContextData = new ResultData(note),
				Action = _ => NotesHelper.OpenInBrowser(note, (NoteViewMode)_viewMode!),
			};
		}).ToList();

		if (!string.IsNullOrEmpty(query.Search))
		{
			_ = results.RemoveAll(x => x.Score <= 0);
		}

		return results;
	}

	public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
	{
		return selectedResult.ContextData is not ResultData selectedData
			? []
			: (List<ContextMenuResult>)[
			new ()
			{
				PluginName = Name,
				Title = Resources.context_copy_link,
				Glyph = "\xE8C8",
				FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
				AcceleratorKey = Key.C,
				AcceleratorModifiers = ModifierKeys.Control,
				Action = _ =>
				{
					Clipboard.SetText(selectedData.Note.PublishLink);
					return true;
				},
			},
			new ()
			{
				PluginName = Name,
				Title = Resources.context_open_edit,
				Glyph="\xE70F",
				FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
				AcceleratorKey = Key.E,
				AcceleratorModifiers = ModifierKeys.Control | ModifierKeys.Alt,
				Action = _ => NotesHelper.OpenInBrowser(selectedData.Note, NoteViewMode.Edit),
			},
			new ()
			{
				PluginName = Name,
				Title = Resources.context_open_both,
				Glyph = "\xE736",
				FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
				AcceleratorKey = Key.B,
				AcceleratorModifiers = ModifierKeys.Control | ModifierKeys.Alt,
				Action = _ => NotesHelper.OpenInBrowser(selectedData.Note, NoteViewMode.Both),
			},
			new ()
			{
				PluginName = Name,
				Title = Resources.context_open_view,
				Glyph = "\xE890",
				FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
				AcceleratorKey = Key.V,
				AcceleratorModifiers = ModifierKeys.Control | ModifierKeys.Alt,
				Action = _ => NotesHelper.OpenInBrowser(selectedData.Note, NoteViewMode.View),
			},
		];
	}

	public void Init(PluginInitContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
		_context.API.ThemeChanged += OnThemeChanged;
		_cache = new CachingService();
		_cache.DefaultCachePolicy.DefaultCacheDurationSeconds = (int)TimeSpan.FromMinutes(1).TotalSeconds;
		UpdateIconPath(_context.API.GetCurrentTheme());
	}

	public string GetTranslatedPluginTitle() => Resources.plugin_name;

	public string GetTranslatedPluginDescription() => Resources.plugin_description;

	private void OnThemeChanged(Theme oldtheme, Theme newTheme) => UpdateIconPath(newTheme);

	private void UpdateIconPath(Theme theme) => _iconPath = theme is Theme.Light or Theme.HighContrastWhite ? "Images/HackMD.light.png" : "Images/HackMD.dark.png";

	public Control CreateSettingPanel() => throw new NotImplementedException();

	public void ReloadData()
	{
		if (_context is null)
		{
			return;
		}

		UpdateIconPath(_context.API.GetCurrentTheme());
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			if (_context != null && _context.API != null)
			{
				_context.API.ThemeChanged -= OnThemeChanged;
			}

			_disposed = true;
		}
	}
}
