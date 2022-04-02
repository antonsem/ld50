using System;

namespace FightyLife
{
	public static class Events
	{
		public static Action<Enemy> EnemyDead;
		public static Action PlayerDead;
	}
}