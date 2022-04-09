using ExtraTools;
using UnityEngine;

namespace FightyLife
{
	public class Movement : MonoBehaviour
	{
		[SerializeField] private Rigidbody2D rigid;
		[SerializeField] private BoxCollider2D col;
		[SerializeField] private float dashSpeed = 10;
		[SerializeField] private float jumpSpeed = 10;
		[SerializeField, Range(0, 1)] private float drag = 0.8f;
		[SerializeField] private float gravity = 1;
		[SerializeField] private LayerMask movementMask;
		[SerializeField] private AudioClip landSound;
		[SerializeField] private AudioClip dashSound;

		public Vector2 Velocity
		{
			get => rigid.velocity;
			private set => rigid.velocity = value;
		}

		public bool IsGrounded { get; private set; }

		public void Dash(float dir)
		{
			var dashVelocity = new Vector2(Mathf.Sign(dir) * dashSpeed, 0);
			Velocity = dashVelocity;

			if (dashSound)
			{
				AudioPlayer.PlayOneShot(dashSound, 0.05f, 0.25f, 0.25f);
			}
		}

		public void Stop()
		{
			Velocity = Vector2.zero;
		}

		public void Jump()
		{
			Velocity = new Vector2(0, jumpSpeed);
		}

		private void ApplyDrag()
		{
			var velocity = Velocity.x;

			if (Mathf.Approximately(velocity, 0))
			{
				return;
			}

			velocity *= drag;

			Velocity = Mathf.Abs(velocity) <= 0.5f
				? new Vector2(0, Velocity.y)
				: new Vector2(velocity, 0);
		}

		private void ApplyGravity(float delta)
		{
			var vVelocity = Velocity.y;

			if (vVelocity > 0)
			{
				IsGrounded = false;
				Velocity += Physics2D.gravity * gravity;
				return;
			}

			var distance = Mathf.Abs(vVelocity + Physics2D.gravity.y * gravity) * delta;
			var origin = (Vector2)col.transform.position + col.offset;

			var hit = Physics2D.BoxCast(origin, col.size, 0, Vector2.down, distance, movementMask);

			if (!IsGrounded && hit && landSound && vVelocity < -0.1f)
			{
				AudioPlayer.PlayOneShot(landSound, 0.1f, 0.25f, 0.15f);
			}

			IsGrounded = hit;

			if (!hit)
			{
				Velocity += Physics2D.gravity * gravity;
				return;
			}

			if (rigid.position.y - hit.point.y > 0.1f)
			{
				rigid.position = new Vector2(rigid.position.x, hit.point.y);
			}

			Velocity = new Vector2(Velocity.x, 0);
		}

		public void Push(Vector3 velocity)
		{
			Stop();
			Velocity = velocity;
		}

		private void FixedUpdate()
		{
			ApplyDrag();

			if (Mathf.Approximately(Velocity.x, 0))
			{
				ApplyGravity(Time.fixedDeltaTime);
			}
		}
	}
}