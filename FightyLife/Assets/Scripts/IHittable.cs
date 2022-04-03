namespace FightyLife
{
	public interface IHittable
	{
		public void Hit(int damage, int attackArea, float xPosition);
	}
}