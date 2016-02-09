using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace DXFramework.Util
{
	/// <summary>
	/// Helper class for random value generation.
	/// </summary>
	public static class Randomizer
	{
		private static Random random;

		static Randomizer()
		{
			random = new Random();
		}

		/// <summary>
		/// Static Random instance created at runtime. Use this instance for all random generation.
		/// </summary>
		public static Random Random
		{
			get { return random; }
		}

		/// <summary>
		/// Returns true if a given probability is greater than the next random number.
		/// </summary>
		/// <param name="probability">Decimal probability value between 0 and 1 inclusive.</param>
		public static bool DoAction(double probability)
		{
			return probability > random.NextDouble();
		}

		#region Random range
		/// <summary>
		/// Returns an integer betwean an inclusive minimum and an inclusive maximum range.
		/// </summary>
		/// <param name="min">Inclusive minimum value.</param>
		/// <param name="max">Inclusive maximum value.</param>
		public static int Range(int min, int max)
		{
			return random.Next(min, max + 1);
		}

		/// <summary>
		/// Returns a float betwean an inclusive minimum and an inclusive maximum range.
		/// </summary>
		/// <param name="min">Inclusive minimum value.</param>
		/// <param name="max">Inclusive maximum value.</param>
		public static float Range(float min, float max)
		{
			return min + (max - min) * (float)random.NextDouble();
		}

		/// <summary>
		/// Returns a double betwean an inclusive minimum and an inclusive maximum range.
		/// </summary>
		/// <param name="min">Inclusive minimum value.</param>
		/// <param name="max">Inclusive maximum value.</param>
		public static double Range(double min, double max)
		{
			return min + (max - min) * random.NextDouble();
		}

		/// <summary>
		/// Returns a Vector2 betwean an inclusive minimum and an inclusive maximum range.
		/// </summary>
		/// <param name="min">Inclusive minimum vector.</param>
		/// <param name="max">Inclusive maximum vector.</param>
		public static Vector2 Range(Vector2 min, Vector2 max)
		{
			Vector2 vec = Vector2.Zero;
			vec.X = min.X + (max.X - min.X) * (float)random.NextDouble();
			vec.Y = min.Y + (max.Y - min.Y) * (float)random.NextDouble();
			return vec;
		}

		/// <summary>
		/// Calculates a Vector2 betwean an inclusive minimum and an inclusive maximum range.
		/// </summary>
		/// <param name="min">Inclusive minimum vector.</param>
		/// <param name="max">Inclusive maximum vector.</param>
		/// <param name="randomVector">Random vector.</param>
		public static void Range(ref Vector2 min, ref Vector2 max, out Vector2 randomVector)
		{
			randomVector.X = min.X + (max.X - min.X) * (float)random.NextDouble();
			randomVector.Y = min.Y + (max.Y - min.Y) * (float)random.NextDouble();
		}

		/// <summary>
		/// Returns a Vector2 from a random point within a rectangle region.
		/// </summary>
		/// <param name="region">Rectangle region to get random Vector2 from. Region boundaries are inclusive.</param>
		public static Vector2 Range(Rectangle region)
		{
			Vector2 vec = Vector2.Zero;
			vec.X = Range(region.Left, region.Right);
			vec.Y = Range(region.Top, region.Bottom);
			return vec;
		}

		/// <summary>
		/// Calculates a Vector2 from a random point within a rectangle region.
		/// </summary>
		/// <param name="region">Rectangle region to get random Vector2 from. Region boundaries are inclusive.</param>
		/// <param name="randomVector">Random vector.</param>
		public static void Range(ref Rectangle region, out Vector2 randomVector)
		{
			randomVector.X = Range(region.Left, region.Right);
			randomVector.Y = Range(region.Top, region.Bottom);
		}

		public static Vector2 Range(Vector2 position, double radius)
		{
			Vector2 pos = Vector2.Zero;

			//double r = random.NextDouble(0, radius);
			//double radians = random.NextDouble(0, Math.PI * 2);
			//pos.X = (float)(Math.Cos(radians) * r);
			//pos.Y = (float)(Math.Sin(radians) * r);

			pos.X = (float)Range(-radius, radius);
			pos.Y = (float)Range(-radius, radius);
			return pos;
		}
		#endregion

		#region Random selection
		/// <summary>
		/// Returns a random element from the array.
		/// </summary>
		/// <typeparam name="T">Array type.</typeparam>
		public static T RandomElement<T>(this T[] array)
		{
			return array[random.Next(array.Length)];
		}

		///// <summary>
		///// Shuffle a collection
		///// http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
		///// </summary>
		//public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
		//{
		//	int n = collection.Count();
		//	while (n > 1)
		//	{
		//		n--;
		//		int k = random.Next(n + 1);
		//		T value = collection[k];
		//		collection[k] = collection[n];
		//		collection[n] = value;
		//	}
		//}

		public static T SelectFirstRandom<T>(this IEnumerable<T> collection)
		{
			return collection.SelectRandom(1).First();
		}

		public static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> collection, int count)
		{
			int picked = 0;
			double itemsLeft = collection.Count();
			var enumerator = collection.GetEnumerator();

			while (picked < count && enumerator.MoveNext())
			{
				double probability = count / itemsLeft--;

				if (DoAction(probability))
				{
					picked++;
					yield return enumerator.Current;
				}
			}
		}
		#endregion
	}
}