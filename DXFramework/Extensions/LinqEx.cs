using System.Collections.Generic;

namespace System.Linq
{
	public static class LinqEx
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T element in source)
			{
				action(element);
			}
		}
	}
}