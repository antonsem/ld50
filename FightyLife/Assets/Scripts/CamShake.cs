using Cinemachine;
using UnityEngine;

namespace FightyLife
{
	public class CamShake : MonoBehaviour
	{
		[SerializeField] private float hitIntensity;
		[SerializeField] private float hitTime;
		[SerializeField] private float deathIntensity;
		[SerializeField] private float deathTime;
		[SerializeField] private float maxIntensity;
		
		private CinemachineVirtualCamera _cam;
		private float _shakeTime;
		private CinemachineBasicMultiChannelPerlin _perlin;

		private void Awake()
		{
			_cam = GetComponent<CinemachineVirtualCamera>();
			_perlin = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		}

		private void OnEnable()
		{
			Events.Hit += OnHit;
			Events.EnemyDead += OnDeath;
			Events.PlayerDead += OnDeath;
		}

		private void OnDisable()
		{
			Events.Hit -= OnHit;
			Events.EnemyDead -= OnDeath;
			Events.PlayerDead -= OnDeath;
		}

		private void OnHit(Vector3 pos, int dir, int hitCount)
		{
			ShakeCamera(Mathf.Min(hitIntensity * hitCount), hitTime);
		}
		
		private void OnDeath(Vector3 arg1, int arg2)
		{
			ShakeCamera(Mathf.Min(deathIntensity, maxIntensity), deathTime);
		}

		private void ShakeCamera(float intensity, float time)
		{
			_perlin.m_AmplitudeGain = intensity;
			_shakeTime = time;
		}

		private void Update()
		{
			if (_shakeTime <= 0)
			{
				return;
			}

			_shakeTime -= Time.deltaTime;

			if (_shakeTime > 0f)
			{
				return;
			}

			_perlin.m_AmplitudeGain = 0f;
		}
	}
}