using UnityEngine;

namespace FightyLife
{
	public class ScoreKeeper : MonoBehaviour
	{
		public int Score { get; private set; }
		
		private void OnEnable()
		{
			Score = 0;
			Events.EnemyDead += OnEnemyDeath;
		}

		private void OnDisable()
		{
			Events.EnemyDead -= OnEnemyDeath;
		}

		private void OnEnemyDeath(Vector3 pos, int dir)
		{
			Score++;
		}
	}
}