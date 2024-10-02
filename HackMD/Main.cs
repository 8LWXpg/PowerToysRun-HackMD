using Community.PowerToys.Run.Plugin.HackMD.Helpers;
using LazyCache;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;
using Wox.Infrastructure;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.HackMD;
public class Main : IPlugin, IPluginI18n, ISettingProvider, IReloadable, IDisposable
{
	private const string AuthToken = nameof(AuthToken);

	private string? _authToken;
	private CachingService? _cache;
	private PluginInitContext? _context;
	private string? _iconPath;
	private bool _disposed;
	public string Name => Properties.Resources.plugin_name;
	public string Description => Properties.Resources.plugin_description;
	public static string PluginID => "7ce12aff470442158be4b7c5d02ef63e";

	public IEnumerable<PluginAdditionalOption> AdditionalOptions =>
	[
		new()
		{
			PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
			Key = AuthToken,
			DisplayLabel = Properties.Resources.setting_api_token,
		},
	];

	public void UpdateSettings(PowerLauncherPluginSettings settings)
	{
		_authToken = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == AuthToken)?.TextValue ?? string.Empty;
		Helpers.NotesHelper.UpdateAuthToken(_authToken);
	}

	public List<Result> Query(Query query)
	{
		ArgumentNullException.ThrowIfNull(query);

		IEnumerable<Note> notes = _cache.GetOrAdd("notes", Helpers.NotesHelper.GetAllNotes);
		var results = notes.Select(note =>
		{
			MatchResult matches = StringMatcher.FuzzySearch(query.Search, note.Title);
			return new Result
			{
				Title = note.Title,
				IcoPath = _iconPath,
				Score = matches.Score,
				TitleHighlightData = matches.MatchData,
				Action = _ => NotesHelper.OpenInBrowser(note),
			};
		}).ToList();

		if (!string.IsNullOrEmpty(query.Search))
		{
			_ = results.RemoveAll(x => x.Score <= 0);
		}

		return results;
	}

	public void Init(PluginInitContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
		_context.API.ThemeChanged += OnThemeChanged;
		_cache = new CachingService();
		_cache.DefaultCachePolicy.DefaultCacheDurationSeconds = (int)TimeSpan.FromMinutes(1).TotalSeconds;
		UpdateIconPath(_context.API.GetCurrentTheme());
	}

	public string GetTranslatedPluginTitle() => Properties.Resources.plugin_name;

	public string GetTranslatedPluginDescription() => Properties.Resources.plugin_description;

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
