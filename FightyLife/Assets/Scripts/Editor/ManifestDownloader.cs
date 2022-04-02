using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// Responsible for downloading and replacing the manifest file
/// </summary>
public static class ManifestDownloader
{
	public static async void LoadManifest(string url)
	{
		var content = await GetContents(url);
		ReplacePackageFile(content);
	}

	/// <summary>
	/// Downloads a text file from a given url
	/// </summary>
	/// <param name="url">Url pointing to a text file</param>
	/// <returns>Text file residing on the url</returns>
	private static async Task<string> GetContents(string url)
	{
		using var client = new HttpClient();

		var response = await client.GetAsync(url);
		var content = await response.Content.ReadAsStringAsync();
		
		return content;
	}

	/// <summary>
	/// Rewrites the manifest.json file in the Packages folder
	/// </summary>
	/// <param name="content">New manifest.json file</param>
	private static void ReplacePackageFile(string content)
	{
		var existing = Path.Combine(Application.dataPath, "../Packages/manifest.json");
		File.WriteAllText(existing, content);
		Client.Resolve();
	}
}