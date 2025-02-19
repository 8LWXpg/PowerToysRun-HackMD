using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Wox.Infrastructure;
using Wox.Plugin.Common;

namespace Community.PowerToys.Run.Plugin.HackMD.Helpers;

public static class NotesHelper
{
	private static readonly HttpClient Client = new();
	private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	public static void UpdateAuthToken(string authToken) => Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

	public static IEnumerable<Note> GetAllNotes()
	{
		try
		{
			HttpResponseMessage res = Client.GetAsync("https://api.hackmd.io/v1/notes").Result;
			_ = res.EnsureSuccessStatusCode();
			var json = res.Content.ReadAsStringAsync().Result;
			Note[] notes = JsonSerializer.Deserialize<Note[]>(json, Options)!;
			return notes;
		}
		catch
		{
			throw;
		}
	}

	public static bool OpenInBrowser(Note note, NoteViewMode viewMode) =>
		Helper.OpenCommandInShell(DefaultBrowserInfo.Path, DefaultBrowserInfo.ArgumentsPattern, $"https://hackmd.io/{note.Id}?{viewMode.ToUrlString()}");
}

public record Note(string Id, string Title, string PublishLink);

public enum NoteViewMode
{
	View = 0,
	Both = 1,
	Edit = 2,
}

public static class NoteViewModeExtensions
{
	public static string ToUrlString(this NoteViewMode mode) => mode switch
	{
		NoteViewMode.View => "view",
		NoteViewMode.Both => "both",
		NoteViewMode.Edit => "edit",
		_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
	};
}
