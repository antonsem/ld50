using UnityEngine;

namespace FightyLife
{
	public class DisableAfter : MonoBehaviour
	{
		[SerializeField] private float lifeTime;

		private float _enabledAt = 0;

		private void OnEnable()
		{
			_enabledAt = Time.time;
		}

		private void Update()
		{
			if (Time.time - _enabledAt > lifeTime)
			{
				gameObject.SetActive(false);
			}
		}
	}
}