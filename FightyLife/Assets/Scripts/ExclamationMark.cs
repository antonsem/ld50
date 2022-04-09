using UnityEngine;
using UnityEngine.UI;

namespace FightyLife
{
	public class ExclamationMark : MonoBehaviour
	{
		[SerializeField] private Image regular;
		[SerializeField] private Image full;
		[SerializeField] private float fullPercentage = 0.75f;

		public void SetColor(Color color)
		{
			regular.color = color;
			full.color = color;
		}
		
		public void SetProgress(float progress)
		{
			regular.gameObject.SetActive(progress > 0);
			full.gameObject.SetActive(progress >= 1);
			regular.fillAmount = progress * fullPercentage;
		}
	}
}