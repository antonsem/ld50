using UnityEngine;

namespace FightyLife
{
	public class ExclamationMark : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer regular;
		[SerializeField] private SpriteRenderer full;
		[SerializeField] private RectTransform mask;
		[SerializeField] private Vector2 startPos;
		[SerializeField] private Vector2 endPos;

		public void SetColor(Color color)
		{
			regular.color = color;
			full.color = color;
		}
		
		public void SetProgress(float progress)
		{
			regular.gameObject.SetActive(progress > 0);
			full.gameObject.SetActive(progress >= 1);
			mask.localPosition = Vector2.Lerp(startPos, endPos, progress);
		}
	}
}