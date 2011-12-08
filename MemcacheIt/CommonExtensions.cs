using System;

namespace MemcacheIt
{
	public static class CommonExtensions
	{
		public static string FormatString(this string template, params object[] args)
		{
			return String.Format(template, args);
		}
	}
}