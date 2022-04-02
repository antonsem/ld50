using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

/// <summary>
/// Responsible for checking package versions and updating if needed
/// </summary>
public static class PackageUpdater
{
	private static AddRequest _addRequest;
	private static Queue<string> _needsUpdate = new Queue<string>();
	private static int _totalUpdateCount = 0;

	/// <summary>
	/// Goes through the packages, marks the ones which needs updating and starts the updating process
	/// </summary>
	/// <param name="packages">Packages to check for updates</param>
	public static void CheckAndUpdateIfNeeded(PackageCollection packages)
	{
		_needsUpdate.Clear();

		var total = packages.Count();
		var current = 0.0f;

		foreach (var package in packages)
		{
			current++;
			var cancel =
				EditorUtility.DisplayCancelableProgressBar("Checking", "Checking for updates...", current / total);

			if (cancel)
			{
				EditorUtility.ClearProgressBar();
				return;
			}

			if (package.version == package.versions.latest)
			{
				continue;
			}

			Debug.Log(
				$"'{package.name}' current:{package.version} last compatible:{package.versions.latestCompatible} verified:{package.versions.verified}");

			if (package.version == package.versions.latestCompatible || package.version == package.versions.verified)
			{
				continue;
			}

			Debug.Log($"'{package.name}' need update from: {package.version} to: {package.versions.latestCompatible}");

			_needsUpdate.Enqueue(package.name);
		}

		EditorUtility.ClearProgressBar();

		_totalUpdateCount = _needsUpdate.Count;

		if (_totalUpdateCount == 0)
		{
			Debug.Log("All packages are up to date!");
			return;
		}

		Debug.Log($"Need to Update {_totalUpdateCount.ToString()} packages");
		UpdateProgress();
		EditorApplication.update = null;
		EditorApplication.update += UpdateProgress;
	}

	/// <summary>
	/// Updates packages in _needsUpdate list
	/// </summary>
	private static void UpdateProgress()
	{
		if (_totalUpdateCount == 0)
		{
			Debug.LogError("Total update count is 0!");
			return;
		}

		var cancel =
			EditorUtility.DisplayCancelableProgressBar("Checking", "Checking for updates...",
				(float)_needsUpdate.Count / _totalUpdateCount);

		_addRequest ??= Client.Add(_needsUpdate.Dequeue());

		if (!_addRequest.IsCompleted)
		{
			return;
		}

		if (_addRequest.Status == StatusCode.Success)
		{
			Debug.Log($"{_addRequest.Result.name} updated to {_addRequest.Result.version}");
		}

		if (_addRequest.Status >= StatusCode.Failure)
		{
			Debug.LogError($"Couldn't update {_addRequest}!\nERROR:\n{_addRequest.Error.message}");
			_needsUpdate.Clear();
		}

		_addRequest = null;
		
		if (cancel)
		{
			_needsUpdate.Clear();
		}

		if (_needsUpdate.Count > 0)
		{
			return;
		}

		// finish
		Debug.Log("All packages are up to date");
		EditorApplication.update -= UpdateProgress;
		EditorUtility.ClearProgressBar();
		_totalUpdateCount = 0;
	}
}