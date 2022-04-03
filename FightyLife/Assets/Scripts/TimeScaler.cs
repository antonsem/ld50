using System.Collections;
using UnityEngine;

namespace FightyLife
{
	public class TimeScaler : MonoBehaviour
	{
		[SerializeField] private float slow = 0.25f;
		[SerializeField] private float slowIncrement = 1f;
		[SerializeField, Range(0, 1)] private float speedUpTime = 0.3f;
		
		private float _slowTime = 0;
		private IEnumerator _slowCoroutine;
		
		private void OnEnable()
		{
			Events.EnemyDead += OnDeath;
		}

		private void OnDisable()
		{
			_slowCoroutine = null;
			Time.timeScale = 1;
			Events.EnemyDead -= OnDeath;
		}

		private void OnDeath(Vector3 arg1, int arg2)
		{
			_slowTime += slowIncrement;

			if (_slowCoroutine != null)
			{
				return;
			}

			_slowCoroutine = SlowDown();
			StartCoroutine(_slowCoroutine);
		}

		private IEnumerator SlowDown()
		{
			Time.timeScale = slow;
			while (_slowTime > 0)
			{
				_slowTime -= Time.deltaTime;

				Time.timeScale = _slowTime < speedUpTime 
					? Mathf.Lerp(1, slow, _slowTime / speedUpTime) 
					: slow;
				
				yield return null;
			}

			Time.timeScale = 1;
			_slowCoroutine = null;
		}
	}
}