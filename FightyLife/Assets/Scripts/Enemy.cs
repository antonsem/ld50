using System.Collections;
using UnityEngine;

namespace FightyLife
{
	public class Enemy : MonoBehaviour, IHittable
	{
		[SerializeField] private Movement movement;
		[SerializeField] private CharacterAnimator animator;
		[SerializeField] private Weapon weapon;
		[SerializeField] private Transform eyes;
		[SerializeField] private float dashCooldown = 1;
		[SerializeField] private LayerMask visibleObjects;
		[SerializeField] private float visionDistance = 5;
		[SerializeField] private int health = 5;


		private Player _player;
		private float _lastDashTime = 0;
		private float _stunTime = 0;
		private bool _canHit = false;
		private int _hitCount = 0;
		private IEnumerator _breakoutCoroutine;


		private void Start()
		{
			_player = FindObjectOfType<Player>();
		}

		private void Update()
		{
			animator.SetHorizontalVelocity(movement.Velocity.x);
			animator.SetVerticalVelocity(movement.Velocity.y);

			if (_stunTime > 0)
			{
				_stunTime -= Time.deltaTime;
				return;
			}

			var canDash = Time.time - _lastDashTime > dashCooldown;

			Dash(canDash);

			Hit();
		}

		private void Hit()
		{
			if (!_canHit || _stunTime > 0 || Mathf.Abs(movement.Velocity.x) < 0.1f)
			{
				return;
			}

			var breakable = weapon.CheckForHit(movement.Velocity.x * Time.deltaTime);
			breakable?.Hit(1);
			var hit = breakable != null;

			if (hit)
			{
				animator.SetAttack();
			}

			_canHit = !hit;
		}

		private void Dash(bool canDash)
		{
			var direction = GetPlayerDirection();

			if (!canDash || Mathf.Approximately(direction.x, 0) || Mathf.Abs(direction.y) > 1)
			{
				return;
			}

			_lastDashTime = Time.time;
			_canHit = true;
			movement.Dash(direction.x);
		}

		private Vector2 GetPlayerDirection()
		{
			var dir = Vector2.zero;
			var eyesPosition = eyes.position;
			var rayDirection = (_player.transform.position - eyesPosition).normalized;

			var hit = Physics2D.Raycast(eyesPosition, rayDirection, visionDistance, visibleObjects);

			if (hit && hit.transform.gameObject.layer == _player.gameObject.layer)
			{
				//Debug.DrawLine(eyesPosition, hit.point, Color.red, 1);
				return _player.transform.position - transform.position;
			}

			//Debug.DrawLine(eyesPosition, eyesPosition + rayDirection * visionDistance, Color.green, 1);
			return dir;
		}

		private IEnumerator BreakoutCoroutine(float breakoutIn = 0.3f)
		{
			while (breakoutIn > 0)
			{
				breakoutIn -= Time.deltaTime;
				yield return null;
			}

			_stunTime = 0;
			_hitCount = 0;

			Dash(true);

			_breakoutCoroutine = null;
		}

		public void Hit(int damage)
		{
			if (_stunTime > 0)
			{
				_hitCount++;

				if (Random.Range(_hitCount, _hitCount + 3.0f) < _hitCount + 1 && _breakoutCoroutine == null)
				{
					Debug.LogError("Breakout!");
					_breakoutCoroutine = BreakoutCoroutine(1);
					StartCoroutine(_breakoutCoroutine);
				}
			}
			else
			{
				_hitCount = 1;
			}

			_stunTime = 2f;
			animator.SetHurt();
			gameObject.SetActive(health > 0);
		}
	}
}