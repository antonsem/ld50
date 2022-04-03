using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FightyLife
{
	public class EnemyManager : MonoBehaviour
	{
		[SerializeField] private Transform[] spawnPoints;
		[SerializeField] private ScoreKeeper score;
		[SerializeField] private GameObject enemyPrefab;
		[SerializeField] private int baseHealth = 5;


		private List<Enemy> _enemyPool = new List<Enemy>(10);
		private int _lastIndex = 0;
		private int _enemiesToSpawn = 0;
		private IEnumerator _spawningMultiple;

		private void OnEnable()
		{
			OnEnemyDeath(Vector3.zero, 0);
			_enemiesToSpawn = 1;
			Events.EnemyDead += OnEnemyDeath;
		}

		private void OnDisable()
		{
			Events.EnemyDead -= OnEnemyDeath;
		}

		private void OnEnemyDeath(Vector3 pos, int dir)
		{
			_enemiesToSpawn += 2;

			if (_spawningMultiple != null)
			{
				return;
			}

			_spawningMultiple = SpawnMultipleCoroutine();
			StartCoroutine(_spawningMultiple);
		}

		private IEnumerator SpawnMultipleCoroutine()
		{
			while (_enemiesToSpawn > 0)
			{
				yield return StartCoroutine(SpawnCoroutine(Random.Range(1f, 2f)));
				_enemiesToSpawn--;
			}

			_spawningMultiple = null;
		}

		private IEnumerator SpawnCoroutine(float spawnAfter)
		{
			while (spawnAfter > 0)
			{
				spawnAfter -= Time.deltaTime;
				yield return null;
			}

			var spawnIndex = Random.Range(0, spawnPoints.Length);

			while (spawnIndex == _lastIndex)
			{
				spawnIndex = Random.Range(0, spawnPoints.Length);
				yield return null;
			}

			_lastIndex = spawnIndex;


			for (var i = 0; i < _enemyPool.Count; i++)
			{
				if (_enemyPool[i].gameObject.activeSelf)
				{
					continue;
				}

				_enemyPool[i].transform.position = spawnPoints[spawnIndex].position;
				_enemyPool[i].Resurrect(baseHealth + score.Score * 2);
				yield break;
			}

			var enemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity)
				.GetComponent<Enemy>();
			enemy.Resurrect(baseHealth + score.Score * 2);
			_enemyPool.Add(enemy);
		}
	}
}