using System;
using Cinemachine;
using UnityEngine;

namespace FightyLife
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera cam;
		[SerializeField] private Vector3 defaultPos = new Vector3(0, 4, -1);
		[SerializeField] private float defaultSize = 5;
		[SerializeField] private float minSize = 2;
		[SerializeField] private float maxLerp = 5;
		[SerializeField] private float zoomOutSpeed = 2;
		[SerializeField] private float lerpStep = 0.25f;


		[SerializeField] private float xRange = 5;
		[SerializeField] private float minY = 1.25f;


		private float _lerpIn = 0;
		private Vector3 _hitPos = Vector3.zero;

		private void OnEnable()
		{
			Events.Hit += OnHit;
		}

		private void OnHit(Vector3 pos, int arg2, int hitCount)
		{
			_hitPos = pos;
			_hitPos.z = defaultPos.z;
			_lerpIn = Mathf.Clamp(_lerpIn + lerpStep, 0, maxLerp);
		}

		private void Update()
		{
			if (_lerpIn > 0)
			{
				_lerpIn -= Time.deltaTime * zoomOutSpeed;
			}

			var lerp = _lerpIn / maxLerp;
			var size = Mathf.Lerp(defaultSize, minSize, lerp);
			cam.m_Lens.OrthographicSize = size;
			cam.transform.position = Vector3.Lerp(defaultPos, _hitPos, lerp);
		}
	}
}