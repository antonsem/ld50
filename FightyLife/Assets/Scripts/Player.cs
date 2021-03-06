using System.Collections;
using ExtraTools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FightyLife
{
	public class Player : MonoBehaviour, IHittable
	{
		[Header("Core")]
		[SerializeField] private Movement movement;
		[SerializeField] private Weapon weapon;
		[SerializeField] private CharacterAnimator animator;
		[SerializeField] private ExclamationMark exclamation;
		[SerializeField] private int maxHealth;
		[SerializeField] private TrailRenderer trail;
		[SerializeField] private Color playerColor;
		[SerializeField] private Transform[] hitPositions;
		[SerializeField] private AudioClip[] hit;
		[SerializeField] private AudioClip death;
		[SerializeField] private AudioClip exclamationSound;
		[SerializeField] private AudioClip nopeSound;
		[SerializeField] private LayerMask enemyMask;
		[SerializeField] private Transform center;


		[Header("Stats")]
		[SerializeField] private float dashCooldown = 0.25f;

		public Vector3 Center => center.position;

		public bool IsBreakingOut => _breakoutCoroutine != null;

		public float Health => _health / Mathf.Max(1, maxHealth);

		private float _health;
		private bool _canHit = false;
		private float _lastDashTime = 0;
		private int _hitCount = 0;
		private int _attackIndex = 0;
		private IEnumerator _breakoutCoroutine;
		private ScoreKeeper _scoreKeeper;


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

		private void Awake()
		{
			_scoreKeeper = FindObjectOfType<ScoreKeeper>();
			Resurrect(maxHealth);
		}

		private void OnEnable()
		{
			Events.EnemyDead += OnEnemyDeath;
		}

		private void OnDisable()
		{
			Events.EnemyDead -= OnEnemyDeath;
		}

		private void OnEnemyDeath(Vector3 pos, int dir)
		{
			_lastDashTime = 0;
			_health = Mathf.Clamp(_health + (_scoreKeeper.Score + 1) * 2, 0, maxHealth);
		}

		private void Update()
		{
			animator.SetHorizontalVelocity(movement.Velocity.x);
			animator.SetVerticalVelocity(movement.Velocity.y);

			var canDash = Time.time - _lastDashTime > dashCooldown;

			if (canDash)
			{
				Dash(true, GetMovementInput());
			}
			else if (_hitCount > 0 && !Mathf.Approximately(GetMovementInput().x, 0))
			{
				_hitCount = 0;
				exclamation.SetProgress(0);
				AudioPlayer.PlayOneShot(nopeSound, 0.2f, 0.05f, 0.05f);
			}

			Hit();
		}

		private void Hit()
		{
			if (!_canHit || Mathf.Abs(movement.Velocity.x) < 0.2f)
			{
				return;
			}

			var attack = Random.Range(1, 4);

			while (attack == _attackIndex)
			{
				attack = Random.Range(1, 4);
			}

			_attackIndex = attack;

			var breakable = weapon.CheckForHit(movement.Velocity.x * Time.deltaTime);
			breakable?.Hit(1, attack - 1, weapon.transform.position.x);
			var didHit = breakable != null;

			if (didHit)
			{
				movement.Stop();
				animator.SetAttack(attack);
			}

			_canHit = !didHit;
		}

		private void Dash(bool canDash, Vector2 input)
		{
			if (!Mathf.Approximately(input.x, 0) && canDash)
			{
				_lastDashTime = Time.time;
				_canHit = true;
				_hitCount = 0;
				exclamation.SetProgress(0);
				movement.Dash(input.x);
				animator.FaceDirection(input.x);
				animator.ResetAttack();
			}
			else if (!Mathf.Approximately(input.y, 0) && canDash && movement.IsGrounded)
			{
				animator.ResetAttack();
				movement.Jump();
			}
		}

		private IEnumerator BreakoutCoroutine(float breakoutIn = 0.3f)
		{
			AudioPlayer.PlayOneShot(exclamationSound, 0.2f, 0.1f, 0.1f);
			exclamation.SetProgress(1);
			var input = Vector2.zero;

			while (breakoutIn > 0)
			{
				breakoutIn -= Time.deltaTime;
				input = GetMovementInput();

				if (!Mathf.Approximately(input.sqrMagnitude, 0))
				{
					break;
				}

				yield return null;
			}

			Dash(true, input);
			_hitCount = 0;
			_lastDashTime = Mathf.Approximately(input.sqrMagnitude, 0)
				? _lastDashTime = Time.time + 0.1f
				: 0;

			if (_lastDashTime <= 0)
			{
				var isHitting = Mathf.Abs(input.x) > 0;
				Enemy closestEnemy = null;

				if (isHitting)
				{
					var hit2D = Physics2D.Raycast(weapon.Origin, input, 2, enemyMask);

					if (hit2D)
					{
						hit2D.transform.TryGetComponent(out closestEnemy);
					}
				}

				Events.PlayerRage?.Invoke();
				var enemies = FindObjectsOfType<Enemy>();

				foreach (var enemy in enemies)
				{
					if (enemy != closestEnemy)
					{
						enemy.Push(transform.position + Vector3.down * 0.25f);
					}
				}
			}
			else
			{
				AudioPlayer.PlayOneShot(nopeSound, 0.2f, 0.05f, 0.05f);
			}

			exclamation.SetProgress(0);
			_breakoutCoroutine = null;
		}


		public void Hit(int damage, int attackArea, float xPosition)
		{
			if (_lastDashTime > 0)
			{
				_hitCount++;

				var probability = Random.Range(_hitCount / 5f, 1);

				if (_hitCount > 1 && probability > 0.75f && _breakoutCoroutine == null)
				{
					_breakoutCoroutine = BreakoutCoroutine(1);
					StartCoroutine(_breakoutCoroutine);
				}
			}
			else
			{
				_hitCount = 1;
			}

			var rotation = (int)Mathf.Sign(transform.position.x - xPosition);
			Events.Hit?.Invoke(hitPositions[attackArea].position, rotation, _hitCount);

			if (_breakoutCoroutine == null)
			{
				exclamation.SetProgress(_hitCount / (_hitCount + 3.0f));
			}

			if (--_health <= 0)
			{
				AudioPlayer.PlayOneShot(death, 0.25f, 0.25f);
				animator.SetDeath();
				Events.PlayerDead?.Invoke(transform.position, rotation > 0 ? 1 : -1);

				if (_breakoutCoroutine != null)
				{
					StopCoroutine(_breakoutCoroutine);
					exclamation.SetProgress(0);
				}

				enabled = false;
			}

			_lastDashTime = Time.time + 0.15f;
			animator.SetHurt();
			AudioPlayer.PlayOneShot(hit.GetRandom(), 1, 0.25f, 0.5f);
		}

		private void Resurrect(int health)
		{
			enabled = true;
			maxHealth = health;
			_health = health;
			SetColors(playerColor);
		}

		private void SetColors(Color color)
		{
			exclamation.SetColor(color);

			for (var i = 0; i < trail.colorGradient.colorKeys.Length; i++)
			{
				trail.colorGradient.colorKeys[i].color = color;
			}
		}
	}
}