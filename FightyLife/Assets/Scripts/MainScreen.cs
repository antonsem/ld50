using ExtraTools;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FightyLife
{
	public class MainScreen : MonoBehaviour
	{
		[SerializeField] private Canvas canvas;
		[SerializeField] private TextMeshProUGUI motivation;
		[SerializeField] private TutorialScreen tutorialScreen;
		[SerializeField] private CreditsScreen creditsScreen;
		[SerializeField] private GameObject keys;
		[SerializeField] private string[] motivationalLines;

		private float _delayTime = 0;
		private bool _firstStart = true;

		private void Set()
		{
			_delayTime = 0.1f;

			if (!_firstStart)
			{
				motivation.text = motivationalLines.GetRandom();
			}

			keys.SetActive(false);
			canvas.enabled = true;
			_firstStart = false;
		}

		private void Update()
		{
			if (!canvas.enabled)
			{
				return;
			}

			if (_delayTime > 0)
			{
				_delayTime -= Time.deltaTime;
				return;
			}

			keys.SetActive(true);

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				SceneManager.LoadScene("Main");
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				canvas.enabled = false;
				tutorialScreen.Set(Set);
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				canvas.enabled = false;
				creditsScreen.Set(Set);
			}
		}
	}
}