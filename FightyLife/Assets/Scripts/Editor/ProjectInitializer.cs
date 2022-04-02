using UnityEditor;

public static class ProjectInitializer
{
	[MenuItem("Extra Tools/Initialization/Download Manifest")]
	private static void DownloadManifest()
	{
		// My github username
		const string user = "antonsem";
		// ID of a particular gist
		const string id = "de9793bb45d16b63792ce438a42b2fd2";

		// This url points to a gist with my preferred default manifest
		// Feel free to replace with your own
		var url = $"https://gist.githubusercontent.com/{user}/{id}/raw";
		
		ManifestDownloader.LoadManifest(url);
	}

	[MenuItem("Extra Tools/Initialization/Update Installed Packages")]
	private static void RetrieveAndUpdateCurrentPackages()
	{
		PackageRetriever.GetPackageListRequest(val => PackageUpdater.CheckAndUpdateIfNeeded(val?.Result));
	}
}