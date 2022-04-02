using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FightyLife
{
	public class GameOverScreen : MonoBehaviour
	{
		[SerializeField] private Canvas canvas;
		[SerializeField] private TextMeshProUGUI scoreText;
		[SerializeField] private Button restartButton;
		[SerializeField] private ScoreKeeper scoreKeeper;
		
		private void Awake()
		{
			canvas.enabled = false;
			restartButton.onClick.AddListener(OnRestart);
		}

		private void OnEnable()
		{
			Events.PlayerDead += OnPlayerDeath;
		}
		
		private void OnDisable()
		{
			Events.PlayerDead -= OnPlayerDeath;
		}

		private void OnPlayerDeath()
		{
			scoreText.text = $"Score: {scoreKeeper.Score}";
			canvas.enabled = true;
		}

		private static void OnRestart()
		{
			SceneManager.LoadScene("Main");
		}
	}
}