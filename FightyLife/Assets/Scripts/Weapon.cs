using UnityEngine;

namespace FightyLife
{
	public class Weapon : MonoBehaviour
	{
		[SerializeField] private BoxCollider2D weaponCol;
		[SerializeField] private LayerMask mask;
		[SerializeField] private float reach = 0.2f;

		public Vector3 Origin => weaponCol.transform.position;
		
		public IHittable CheckForHit(float direction)
		{
			if (Mathf.Approximately(direction, 0))
			{
				return null;
			}

			direction = Mathf.Sign(direction);

			var distance = Mathf.Abs(direction) * reach;
			var origin = (Vector2)weaponCol.transform.position + weaponCol.offset;

			var hit = Physics2D.BoxCast(origin, weaponCol.size, 0, Vector2.right * direction, distance,
				mask);

			if (hit && hit.transform.TryGetComponent(out IHittable breakable))
			{
				return breakable;
			}
			
			return null;
		}
	}
}