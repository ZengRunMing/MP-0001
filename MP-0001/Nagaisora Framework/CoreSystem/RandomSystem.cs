using System;

namespace NagaisoraFramework
{
	public static class RandomSystem
	{

		public static Random Random;

		public static int RandomSeed;

		public static void Initialize()
		{
			Random = new Random();
			RandomSeed = Random.Next();

			Random = new Random(RandomSeed);
		}

		public static void Initialize(int seed)
		{
			RandomSeed = seed;
			Random = new Random(seed);
		}

		public static int RandomInt()
		{
			return RandomInt(int.MinValue, int.MaxValue);
		}

		public static int RandomInt(int min, int max)
		{
			return Random.Next(min, max);
		}

		public static float RandomFloat()
		{
			return RandomFloat(float.MinValue, float.MaxValue);
		}

		public static float RandomFloat(float min, float max)
		{
			return (float)FrameworkMath.Scale(Random.NextDouble(), 0, 1, min, max);
		}
	}
}
