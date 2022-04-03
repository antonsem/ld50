using System.Collections.Generic;
using UnityEngine;

namespace FightyLife
{
	public class BloodPool : MonoBehaviour
	{
		[SerializeField] private GameObject bloodExplosionPrefab;
		[SerializeField] private GameObject bloodPrefab;

		private List<ParticleSystem> _explosionsPool = new ();
		private List<ParticleSystem> _bloodPool = new ();

		private void OnEnable()
		{
			Events.EnemyDead += SpawnExplosion;
			Events.PlayerDead += SpawnExplosion;
			Events.Hit += SpawnBlood;
		}

		private void OnDisable()
		{
			Events.EnemyDead -= SpawnExplosion;
			Events.PlayerDead -= SpawnExplosion;
			Events.Hit -= SpawnBlood;
		}

		private void SpawnExplosion(Vector3 position, int direction)
		{
			Spawn(_explosionsPool, bloodExplosionPrefab, position, direction > 0 ? 0 : 180);
		}

		private void SpawnBlood(Vector3 position, int direction, int hitCount)
		{
			Spawn(_bloodPool, bloodPrefab, position, 90 * direction);
		}

		private static void Spawn(List<ParticleSystem> pool, GameObject prefab, Vector3 position, int rotation)
		{
			foreach (var o in pool)
			{
				if (o.gameObject.activeSelf)
				{
					continue;
				}

				o.transform.position = position;
				o.transform.localRotation = Quaternion.Euler(0, rotation, 0);
				o.gameObject.SetActive(true);
				o.Play();
				return;
			}

			var newSystem = Instantiate(prefab, position, Quaternion.Euler(0, rotation, 0))
				.GetComponent<ParticleSystem>();
			pool.Add(newSystem);
			newSystem.Play();
		}
	}
}