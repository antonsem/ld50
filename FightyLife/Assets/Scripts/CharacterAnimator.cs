using UnityEngine;

namespace FightyLife
{
	public class CharacterAnimator : MonoBehaviour
	{
		[SerializeField] private Animator anim;
		[SerializeField] private SpriteRenderer[] visuals;


		private readonly int _verticalSpeed = Animator.StringToHash("VerticalSpeed");
		private readonly int _horizontalSpeed = Animator.StringToHash("HorizontalSpeed");
		private readonly int _attack = Animator.StringToHash("Attack");
		private readonly int _hurt = Animator.StringToHash("Hurt");
		private readonly int _dead = Animator.StringToHash("Dead");


		public void SetVerticalVelocity(float velocity)
		{
			anim.SetFloat(_verticalSpeed, velocity);
		}

		public void SetHorizontalVelocity(float velocity)
		{
			anim.SetFloat(_horizontalSpeed, Mathf.Abs(velocity));
		}

		public void FaceDirection(float velocity)
		{
			foreach (var visual in visuals)
			{
				visual.flipX = velocity < 0;
			}
		}

		public void SetAttack(int id)
		{
			anim.SetInteger(_attack, id);
		}

		public void ResetAttack()
		{
			anim.SetInteger(_attack, 0);
		}

		public void SetHurt()
		{
			ResetAttack();
			anim.SetTrigger(_hurt);
		}

		public void SetDeath()
		{
			anim.SetBool(_dead, true);
		}
	}
}