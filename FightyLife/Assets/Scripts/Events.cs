using System;
using UnityEngine;

namespace FightyLife
{
	public static class Events
	{
		public static Action<Vector3, int> EnemyDead;
		public static Action<Vector3, int> PlayerDead;
		public static Action<Vector3, int, int> Hit;
	}
}