using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FightyLife
{
	public class GameOverScreen : MonoBehaviour
	{
		[SerializeField] private Canvas canvas;
		[SerializeField] private TextMeshProUGUI scoreText;
		[SerializeField] private ScoreKeeper scoreKeeper;
		[SerializeField] private float delay = 1;
		[SerializeField] private GameObject keysObject;
		[SerializeField] private TutorialScreen tutorialScreen;
		

		private float _delayTime = 0;
		
		private void Awake()
		{
			canvas.enabled = false;
		}

		private void OnEnable()
		{
			keysObject.SetActive(false);
			Events.PlayerDead += OnPlayerDeath;
		}
		
		private void OnDisable()
		{
			Events.PlayerDead -= OnPlayerDeath;
		}

		private void OnPlayerDeath(Vector3 pos, int dir)
		{
			scoreText.text = $"Invaders killed: {scoreKeeper.Score}";
			Set(delay);
		}

		private void Set(float countdown = 0.1f)
		{
			_delayTime = countdown;
			keysObject.SetActive(false);
			canvas.enabled = true;
		}

		private static void OnRestart()
		{
			SceneManager.LoadScene("Main");
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
			
			keysObject.SetActive(true);

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				OnRestart();
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				tutorialScreen.Set(() => Set());
				canvas.enabled = false;
			}
			
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				SceneManager.LoadScene("Menu");
			}
		}
	}
}