using UnityEngine;

namespace FightyLife
{
	public class CharacterAnimator : MonoBehaviour
	{
		[SerializeField] private Animator anim;

		private readonly int _verticalSpeed = Animator.StringToHash("VerticalSpeed");
		private readonly int _horizontalSpeed = Animator.StringToHash("HorizontalSpeed");
		private readonly int _attack = Animator.StringToHash("Attack");
		private readonly int _hurt = Animator.StringToHash("Hurt");

		
		public void SetVerticalVelocity(float velocity)
		{
			anim.SetFloat(_verticalSpeed, velocity);
		}

		public void SetHorizontalVelocity(float velocity)
		{
			anim.SetFloat(_horizontalSpeed, Mathf.Abs(velocity));
		}

		public void SetAttack()
		{
			anim.SetTrigger(_attack);
		}

		public void SetHurt()
		{
			anim.SetTrigger(_hurt);
		}
	}
}