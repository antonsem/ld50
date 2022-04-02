using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

/// <summary>
/// Responsible for retrieving installed packages
/// https://github.com/mitay-walle/Unity-PackageManager-Update-All/blob/master/PackageManagerUpdateAllUtility.cs
/// </summary>
public static class PackageRetriever
{
	private static ListRequest _listRequest;
	private static Action<ListRequest> _callback;

	public static void GetPackageListRequest(Action<ListRequest> callback)
	{
		_callback = callback;
		_listRequest = Client.List(true);
		EditorApplication.update += ListProgress;
		EditorUtility.DisplayProgressBar("Retrieving Packages", "Retrieving currently installed packages...", 0.5f);
	}

	private static void ListProgress()
	{
		if (_listRequest == null)
		{
			Stop();
			return;
		}

		if (!_listRequest.IsCompleted)
		{
			return;
		}

		EditorUtility.ClearProgressBar();
		EditorApplication.update -= ListProgress;

		if (_listRequest.Status == StatusCode.Failure)
		{
			Debug.LogError($"Couldn't get currently installed packages!\nERROR:{_listRequest.Error}");
			EditorUtility.DisplayDialog("ERROR",
				"Couldn't get currently installed packages! Check the console for details...", "OK :(");

			return;
		}

		_callback?.Invoke(_listRequest);
		_callback = null;
	}

	private static void Stop()
	{
		Debug.LogError("Something went wrong!");

		EditorApplication.update -= ListProgress;
		_callback = null;

		EditorUtility.ClearProgressBar();
		EditorUtility.DisplayDialog("ERROR", "Package list downloading couldn't start...", "OK :(");
	}
}