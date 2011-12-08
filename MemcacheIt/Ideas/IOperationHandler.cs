using System;

namespace MemcacheIt.Ideas
{
	public interface IOperationHandler<TOperation>
	{
		Func<TOperation> OperationFactory { get; }
		void Handle(TOperation command);
	}
}