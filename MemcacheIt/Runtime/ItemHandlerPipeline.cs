using System;
using System.Collections.Generic;
using System.Linq;

namespace MemcacheIt.Runtime
{
	public class ItemHandlerPipeline
	{
		private IList<IItemHandler> _handlers = new List<IItemHandler>();

		public ItemHandlerPipeline(IEnumerable<IItemHandler> handlers)
		{
			_handlers = handlers.ToList();
		}

		public void Append(IItemHandler handler)
		{
			_handlers.Add(handler);
		}
		
		public IList<IItemHandler> Handlers
		{
			get { return _handlers; }
		}



		public virtual CacheItem Process(CacheItem cacheItem, CommandContext commandContext)
		{
			/*
			 * create new cache item
			 * assign original item
			 * pass to default handler
			 * then pass to remaining handlers
			 */
			throw new NotImplementedException();
		}
	}
}