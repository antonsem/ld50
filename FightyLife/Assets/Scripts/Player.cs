using UnityEngine;

namespace FightyLife
{
	public class Player : MonoBehaviour, IHittable
	{
		[Header("Core")]
		[SerializeField] private Movement movement;
		[SerializeField] private Weapon weapon;
		[SerializeField] private CharacterAnimator animator;

		[Header("Stats")]
		[SerializeField] private float dashCooldown = 0.25f;


		private bool _canHit = false;
		private float _lastDashTime = 0;


		private static Vector2 GetMovementInput()
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				return Vector2.left;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				return Vector2.right;
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				return Vector2.up;
			}

			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				return Vector2.down;
			}

			return Vector2.zero;
		}

		private void Update()
		{
			animator.SetHorizontalVelocity(movement.Velocity.x);
			animator.SetVerticalVelocity(movement.Velocity.y);

			var input = GetMovementInput();
			var canDash = Time.time - _lastDashTime > dashCooldown;

			if (!Mathf.Approximately(input.x, 0) && canDash)
			{
				_lastDashTime = Time.time;
				_canHit = true;
				movement.Dash(input.x);
			}
			else if (!Mathf.Approximately(input.y, 0) && movement.IsGrounded)
			{
				movement.Jump();
			}

			if (_canHit)
			{
				var breakable = weapon.CheckForHit(movement.Velocity.x * Time.deltaTime);
				breakable?.Hit(1);
				var hit = breakable != null;

				if (hit)
				{
					animator.SetAttack();
				}

				_canHit = !hit;
			}
		}

		public void Hit(int damage)
		{
			animator.SetHurt();
		}
	}
}