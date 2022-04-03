using Cinemachine;
using UnityEngine;

namespace FightyLife
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera cam;
		[SerializeField] private Vector3 defaultPos = new (0, 4, -1);
		[SerializeField] private float defaultSize = 5;
		[SerializeField] private float minSize = 2;
		[SerializeField] private float maxLerp = 5;
		[SerializeField] private float zoomOutSpeed = 2;
		[SerializeField] private float lerpStep = 0.25f;


		private float _lerpIn = 0;
		private Vector3 _hitPos = Vector3.zero;
		private bool _playerIsDead = false;

		private void OnEnable()
		{
			Events.Hit += OnHit;
			Events.PlayerDead += OnPlayerDeath;
		}
		
		private void OnDisable()
		{
			Events.Hit -= OnHit;
			Events.PlayerDead -= OnPlayerDeath;
		}
		
		private void OnPlayerDeath(Vector3 pos, int arg2)
		{
			_hitPos = pos;
			_hitPos.z = defaultPos.z;
			_playerIsDead = true;
		}


		private void OnHit(Vector3 pos, int arg2, int hitCount)
		{
			_hitPos = pos;
			_hitPos.z = defaultPos.z;
			_lerpIn = Mathf.Clamp(_lerpIn + lerpStep, 0, maxLerp);
		}

		private void Update()
		{
			if (_playerIsDead)
			{
				ZoomOnPlayer();
				return;
			}
			
			if (_lerpIn > 0)
			{
				_lerpIn -= Time.deltaTime * zoomOutSpeed;
			}

			var lerp = _lerpIn / maxLerp;
			var size = Mathf.Lerp(defaultSize, minSize, lerp);
			cam.m_Lens.OrthographicSize = size;
			cam.transform.position = Vector3.Lerp(defaultPos, _hitPos, lerp);
		}

		private void ZoomOnPlayer()
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, _hitPos, Time.deltaTime * 0.5f);
			cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 2, Time.deltaTime * 0.5f);
		}
	}
}