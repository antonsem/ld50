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

		private void OnEnemyDeath(Enemy obj)
		{
			Score++;
			Debug.Log($"Score: {Score.ToString()}");
		}
	}
}