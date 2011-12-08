using System;

namespace MemcacheIt.Runtime
{
	public class FormattingService
	{
		public virtual object Serialize(object data, CommandContext executingCommand)
		{
			throw new NotImplementedException();
		}

		public virtual object Deserialize(object value, CommandContext executingCommand)
		{
			throw new NotImplementedException();
		}
	}
}