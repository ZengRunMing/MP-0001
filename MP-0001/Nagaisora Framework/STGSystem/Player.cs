using Godot;

namespace NagaisoraFramework.STGSystem
{
	public partial class Player : STGEntity
	{
		public float HighSpeed;
		public float SoltSpeed;

		public bool IsSolt = false;
		public bool IsShoot;

		public float MoveVectorX;
		public float MoveVectorY;

		public override void Init()
		{
			base.Init();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			//Velocity = HighSpeed;

			if (IsSolt)
			{
			//	Velocity = SoltSpeed;
			}

			if (IsShoot)
			{
				Shoot();
			}

			AxisMove(MoveVectorX, MoveVectorY);
		}

		//public override void Move()
		//{
		//	base.Move();

		//	Vector2 position = Position;

		//	if (position.Y >= STGControler.PlayerMaxPosition.Y)
		//	{
		//		position.Y = STGControler.PlayerMaxPosition.Y;
		//	}
		//	if (position.Y <= -STGControler.PlayerMaxPosition.Y)
		//	{
		//		position.Y = -STGControler.PlayerMaxPosition.Y;
		//	}

		//	if (position.X >= STGControler.PlayerMaxPosition.X)
		//	{
		//		position.X = STGControler.PlayerMaxPosition.X;
		//	}
		//	if (position.X <= -STGControler.PlayerMaxPosition.X)
		//	{
		//		position.X = -STGControler.PlayerMaxPosition.X;
		//	}

		//	Position = position;
		//}

		//public override void OnKeyDown(InputKey inputKey)
		//{
		//	float x;
		//	float y;

		//	if (inputKey.Up)
		//	{
		//		y = 1;
		//	}
		//	else if (inputKey.Down)
		//	{
		//		y = -1;
		//	}
		//	else
		//	{
		//		y = 0;
		//	}

		//	if (inputKey.Left)
		//	{
		//		x = -1;
		//	}
		//	else if (inputKey.Right)
		//	{
		//		x = 1;
		//	}
		//	else
		//	{
		//		x = 0;
		//	}

		//	if (inputKey.Shoot)
		//	{
		//		IsShoot = true;
		//	}
		//	else
		//	{
		//		IsShoot = false;
		//	}

		//	if (inputKey.Slow)
		//	{
		//		IsSolt = true;
		//	}
		//	else
		//	{
		//		IsSolt = false;
		//	}

		//	MoveVectorX = x;
		//	MoveVectorY = y;
		//}

		public void AxisMove(float x, float y)
		{
			Vector2 Vector = new Vector2(x, y);

			if (x < 0)
			{
				//Animator.SetBool("MoveingL", true);
			}
			else
			{
				//Animator.SetBool("MoveingL", false);
			}

			if (x > 0)
			{
				//Animator.SetBool("MoveingR", true);
			}
			else
			{
				//Animator.SetBool("MoveingR", false);
			}

			if ((Position.Y >= STGControler.PlayerMaxPosition.Y && Vector.Y > 0) || (Position.Y <= -STGControler.PlayerMaxPosition.Y && Vector.Y < 0))
			{
				Vector.Y = 0;
			}
			if ((Position.X >= STGControler.PlayerMaxPosition.X && Vector.X > 0) || (Position.X <= -STGControler.PlayerMaxPosition.X && Vector.X < 0))
			{
				Vector.Y = 0;
			}

			//MoveVector = Vector;
		}

		public virtual void Shoot()
		{

		}
	}
}
