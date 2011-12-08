using System;
using System.Runtime.Serialization;

namespace MemcacheIt
{
	[Serializable] 
	public class CachingException : Exception
	{
		public CachingException()
		{}

		public CachingException(string message) : base(message)
		{}

		public CachingException(string message, Exception inner) : base(message, inner)
		{}

		protected CachingException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{}
	}
}