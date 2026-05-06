using Godot;
using NagaisoraFramework.STGSystem;
using System;

namespace NagaisoraFramework
{
	public static class FrameworkMath
	{
		public static float[] zsin;
		public static float[] zcos;

		static FrameworkMath()
		{
			zsin = new float[9000];
			zcos = new float[9000];

			for (int i = 0; i < 9000; i++)
			{
				zsin[i] = Mathf.Sin(i * 0.04f * ((float)Math.PI / 180f));
				zcos[i] = Mathf.Cos(i * 0.04f * ((float)Math.PI / 180f));
			}
		}

		#region Basic
		public static float Dot(Vector2 lhs, Vector2 rhs)
		{
			return lhs.X * rhs.X + lhs.Y * rhs.Y;
		}
		#endregion

		#region Arc Calculation
		public static (float X, float Y) GetCircleCenter(float x1, float y1, float x2, float y2, float radius)
		{
			float x, y;

			float c1 = (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1) / (2 * (x2 - x1));
			float c2 = (y2 - y1) / (x2 - x1);
			float a = (c2 * c2 + 1);
			float b = (2 * x1 * c2 - 2 * c1 * c2 - 2 * y1);
			float c = x1 * x1 - 2 * x1 * c1 + c1 * c1 + y1 * y1 - radius * radius;
			
			y = (-b + (float)Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
			x = c1 - c2 * y;

			return (x, y);
		}

		public static Vector2 GetCircleCenter(Vector2 point1, Vector2 point2, float radius)
		{
			(float x, float y) = GetCircleCenter(point1.X, point1.Y, point2.X, point2.Y, radius);

			return new Vector2(x, y);
		}
		#endregion

		#region EulerAnglesMath
		public static float Sin(float In)
		{
			return zsin[(int)(In * 25f)];
		}

		public static float Cos(float In)
		{
			return zcos[(int)(In * 25f)];
		}

		public static float Tan(float In)
		{
			return Sin(In) / Cos(In);
		}

		public static float EulerAnglesADS(float a)
		{
			a %= 360f;
			if (a < 0f)
			{
				a += 360f;
			}
			if (a == 360f)
			{
				a = 0f;
			}
			return a;
		}

		public static float EulerAnglesADS2(float a)
		{
			a %= 360f;
			if (a < 0f)
			{
				a += 360f;
			}
			if (a >= 180f)
			{
				a -= 360f;
			}
			if (a == 180f)
			{
				a = -180f;
			}
			return a;
		}

		public static Vector2 GetVectorFromDirection(float Direction)
		{
			return new Vector2(Mathf.Sin(Direction), Mathf.Cos(Direction));
		}
		#endregion

		#region Distances
		public static float Distance(Vector2 p1, Vector2 p2)
		{
			return (p1 - p2).Length();
		}

		public static float DistanceLine(Vector2 a, Vector2 b, Vector2 c)
		{
			Vector2 vector = b - a;
			Vector2 rhs = c - a;
			float num = Dot(vector, rhs);
			if (num < 0f)
			{
				return Distance(c, a);
			}
			float num2 = Dot(vector, vector);
			if (num > num2)
			{
				return Distance(c, b);
			}
			num /= num2;
			Vector2 p = a + num * vector;
			return Distance(c, p);
		}

		public static float Direction(Vector2 basePosition, Vector2 targetPosition)
		{
			return (float)Math.PI + Mathf.Atan2(basePosition.X - targetPosition.X, basePosition.Y - targetPosition.Y);
		}

		public static Vector2 Point2lineVerticalPointPosition(Vector2 m, Vector2 a, Vector2 b)
		{
			Vector2 vector = b - a;
			Vector2 rhs = m - a;
			float num = Dot(vector, rhs);
			float num2 = Dot(vector, vector);
			num /= num2;
			return a + num * vector;
		}
		#endregion

		#region Scale
		public static float Scale(float i, float imin, float imax, float omin, float omax)
		{
			float a = i - imin / imax - imin;
			float @out = a * (omax - omin) + omin;
			return @out;
		}

		public static int Scale(int i, int imin, int imax, int omin, int omax)
		{
			int a = i - imin / imax - imin;
			int @out = a * (omax - omin) + omin;
			return @out;
		}

		public static long Scale(long i, long imin, long imax, long omin, long omax)
		{
			long a = i - imin / imax - imin;
			long @out = a * (omax - omin) + omin;
			return @out;
		}

		public static double Scale(double i, double imin, double imax, double omin, double omax)
		{
			double a = i - imin / imax - imin;
			double @out = a * (omax - omin) + omin;
			return @out;
		}
		#endregion
	}
}
