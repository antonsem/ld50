using UnityEngine;
using UnityEngine.UI;

namespace FightyLife
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField] private Slider healthSlider;

		private Player _player;

		private void Start()
		{
			_player = FindObjectOfType<Player>();
		}

		private void Update()
		{
			if (!_player)
			{
				return;
			}

			healthSlider.value = Mathf.Lerp(healthSlider.value, _player.Health, Time.deltaTime * 5);
		}
	}
}