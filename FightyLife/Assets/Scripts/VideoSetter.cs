using UnityEngine;
using UnityEngine.Video;

public class VideoSetter : MonoBehaviour
{
	[SerializeField] private string videoName;
	[SerializeField] private VideoPlayer player;

	private void Awake()
	{
		player.url = $"{Application.dataPath}/StreamingAssets/{videoName}.mp4";
		player.Play();
	}
}