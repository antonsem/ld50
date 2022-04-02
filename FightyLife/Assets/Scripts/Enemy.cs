using System.Collections;
using UnityEngine;

namespace FightyLife
{
	public class Enemy : MonoBehaviour, IHittable
	{
		[SerializeField] private Movement movement;
		[SerializeField] private CharacterAnimator animator;
		[SerializeField] private Weapon weapon;
		[SerializeField] private ExclamationMark exclamation;
		[SerializeField] private Transform eyes;
		[SerializeField] private float dashCooldown = 1;
		[SerializeField] private LayerMask visibleObjects;
		[SerializeField] private float visionDistance = 5;
		[SerializeField] private int health = 5;
		[SerializeField, Range(0, 1)] private float runFromBreakoutChance = 0.25f;
		[SerializeField] private Gradient healthGradient;
		[SerializeField] private SpriteRenderer visual;
		[SerializeField] private TrailRenderer trail;

		
		private Player _player;
		private float _lastDashTime = 0;
		private float _stunTime = 0;
		private bool _canHit = false;
		private int _hitCount = 0;
		private int _runTurns = 0;
		private int _fullHealth = 5;
		private int _attackIndex = 0;
		private IEnumerator _breakoutCoroutine;

		private void OnDisable()
		{
			if (_breakoutCoroutine != null)
			{
				StopCoroutine(_breakoutCoroutine);
				_breakoutCoroutine = null;
			}

			exclamation.SetProgress(0);
		}

		private void Start()
		{
			_player = FindObjectOfType<Player>();
		}

		private void Update()
		{
			if (!_player)
			{
				return;
			}

			animator.SetHorizontalVelocity(movement.Velocity.x);
			animator.SetVerticalVelocity(movement.Velocity.y);

			if (_stunTime > 0)
			{
				_stunTime -= Time.deltaTime;
				return;
			}

			var canDash = Time.time - _lastDashTime > dashCooldown;

			if (canDash)
			{
				Dash();
			}

			Hit();
		}

		private void Hit()
		{
			if (!_canHit || _stunTime > 0 || Mathf.Abs(movement.Velocity.x) < 0.1f)
			{
				return;
			}

			var breakable = weapon.CheckForHit(movement.Velocity.x);
			breakable?.Hit(1);
			var hit = breakable != null;

			if (hit)
			{
				_lastDashTime = Time.time - 0.5f;
				movement.Stop();
				var attack = Random.Range(1, 4);

				while (attack == _attackIndex)
				{
					attack = Random.Range(1, 4);
				}

				_attackIndex = attack;
				
				animator.SetAttack(attack);
			}

			_canHit = !hit;
		}

		private void Dash()
		{
			_lastDashTime = Time.time;

			var input = GetPlayerDirection();

			if (Mathf.Approximately(input.x, 0) && Mathf.Abs(input.y) > 0.1f)
			{
				_lastDashTime -= Random.Range(0.7f, 0.9f);
				movement.Jump();
				return;
			}

			_canHit = true;

			if (_player && _player.IsBreakingOut && Random.value < runFromBreakoutChance)
			{
				_runTurns = 2;
			}

			if (--_runTurns >= 0)
			{
				if (Random.value < 0.5f)
				{
					movement.Jump();
				}
				else
				{
					movement.Dash(-input.x);
					animator.FaceDirection(-input.x);
				}
			}
			else
			{
				movement.Dash(input.x);
				animator.FaceDirection(input.x);
			}

			exclamation.SetProgress(0);
		}

		private Vector2 GetPlayerDirection()
		{
			var dir = Vector2.zero;
			var eyesPosition = eyes.position;
			var rayDirection = (_player.transform.position - eyesPosition).normalized;

			var hit = Physics2D.Raycast(eyesPosition, rayDirection, visionDistance, visibleObjects);

			if (!hit)
			{
				return dir;
			}

			if (hit.transform.gameObject.layer == _player.gameObject.layer)
			{
				return _player.transform.position - transform.position;
			}

			if (hit.distance < 3 && movement.IsGrounded)
			{
				return Vector2.up;
			}

			return _player.transform.position - transform.position;
		}

		private IEnumerator BreakoutCoroutine(float breakoutIn = 0.3f)
		{
			exclamation.SetProgress(1);

			while (breakoutIn > 0)
			{
				breakoutIn -= Time.deltaTime;
				yield return null;
			}

			_stunTime = 0;
			_hitCount = 0;

			Dash();
			exclamation.SetProgress(0);

			_breakoutCoroutine = null;
		}

		public void Hit(int damage)
		{
			_runTurns = 0;

			if (_stunTime > 0)
			{
				_hitCount++;

				if (Random.Range(_hitCount, _hitCount + 3.0f) < _hitCount + 1 && _breakoutCoroutine == null)
				{
					_breakoutCoroutine = BreakoutCoroutine(1);
					StartCoroutine(_breakoutCoroutine);
				}
			}
			else
			{
				_hitCount = 1;
			}

			if (_breakoutCoroutine == null)
			{
				exclamation.SetProgress(_hitCount / (_hitCount + 3.0f));
			}

			_stunTime = 0.75f;
			animator.SetHurt();

			if (--health <= 0)
			{
				Events.EnemyDead?.Invoke(this);
			}

			var color = healthGradient.Evaluate((float)health / _fullHealth);
			SetColors(color);

			gameObject.SetActive(health > 0);
		}

		private void SetColors(Color color)
		{
			exclamation.SetColor(color);
			visual.color = color;
			for (var i = 0; i < trail.colorGradient.colorKeys.Length; i++)
			{
				trail.colorGradient.colorKeys[i].color = color;
			}
		}

		public void Resurrect(int hp)
		{
			_fullHealth = hp;
			health = hp;
			animator.ResetAttack();
			gameObject.SetActive(true);
			_stunTime = 0;
			_hitCount = 0;
			_runTurns = 0;
			_lastDashTime = Time.time;
			_canHit = true;
			
			var color = healthGradient.Evaluate((float)health / _fullHealth);
			SetColors(color);
		}
	}
}