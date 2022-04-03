using System;
using UnityEngine;

namespace FightyLife
{
	public class CreditsScreen : MonoBehaviour
	{
		[SerializeField] private Canvas canvas;
		[SerializeField] private GameObject keys;

		private float _delayTime = 0;
		private Action _callback;

		public void Set(Action callback)
		{
			_delayTime = 0.1f;
			_callback = callback;
			keys.SetActive(false);
			canvas.enabled = true;
		}

		private void OnBack()
		{
			canvas.enabled = false;
			_callback?.Invoke();
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

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				OnBack();
			}
		}
	}
}