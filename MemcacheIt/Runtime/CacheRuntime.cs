using System;
using MemcacheIt.Commands;

namespace MemcacheIt.Runtime
{
	public class CacheRuntime
	{
		private readonly KeyBuilder _keyBuilder;
		private readonly ItemHandlerPipeline _itemHandlerPipeline;
		private readonly IMemcachedContract _memcachedClient;
		private readonly FormattingService _formattingServices;
		
		public CacheRuntime(
			KeyBuilder keyBuilder,
			ItemHandlerPipeline itemHandlerPipeline,
			FormattingService formattingService,
			IMemcachedContract memcachedClient)
		{
			_keyBuilder = keyBuilder;
			_itemHandlerPipeline = itemHandlerPipeline;
			_memcachedClient = memcachedClient;
			_formattingServices = formattingService;
		}


		public GetCommand Get(Action<ItemBuilderSyntax> item)
		{
			return HandleCommand(new GetCommand(GetItem(item), false), OnHandleGet);
		}

		private void OnHandleGet(GetCommand command)
		{
			var item = command.Item;
			var commandContext = new CommandContext(command, item);
			var key = ProcessItemAndBuildKey(commandContext);
			var value = _memcachedClient.Get(key);
			if (value != null)
			{
				item.Data = _formattingServices.Deserialize(value, commandContext);
				command.ItemFound(item);
			}
			else
			{
				command.ItemNotFound();
			}
		}

		public GetCommand GetWithCas(Action<ItemBuilderSyntax> item)
		{
			return HandleCommand(new GetCommand(GetItem(item), true), OnHandleGetWithCas);
		}

		private void OnHandleGetWithCas(GetCommand command)
		{
			var item = command.Item;
			var commandContext = new CommandContext(command, item);
			var key = ProcessItemAndBuildKey(commandContext);
			ulong uniqueID;
			var value = _memcachedClient.Gets(key, out uniqueID);
			if (value != null)
			{
				item.Data = _formattingServices.Deserialize(value, commandContext);
				item.Cas = uniqueID;
				command.ItemFound(item);
			}
			else
			{
				command.ItemNotFound();
			}
		}


		public StoreCommand Set(Action<ItemBuilderSyntax> item, TimeToLive timeToLive)
		{
			return HandleCommand(new StoreCommand(GetItem(item), timeToLive, StoreMode.Set), OnHandleStore);
		}
		
		public StoreCommand Prepend(Action<ItemBuilderSyntax> item, TimeToLive timeToLive)
		{
			return HandleCommand(new StoreCommand(GetItem(item), timeToLive, StoreMode.Prepend), OnHandleStore);
		}

		public StoreCommand Append(Action<ItemBuilderSyntax> item, TimeToLive timeToLive)
		{
			return HandleCommand(new StoreCommand(GetItem(item), timeToLive, StoreMode.Append), OnHandleStore);
		}

		public StoreCommand Add(Action<ItemBuilderSyntax> item, TimeToLive timeToLive)
		{
			return HandleCommand(new StoreCommand(GetItem(item), timeToLive, StoreMode.Add), OnHandleStore);
		}

		public StoreCommand Replace(Action<ItemBuilderSyntax> item, TimeToLive timeToLive)
		{
			return HandleCommand(new StoreCommand(GetItem(item), timeToLive, StoreMode.Replace), OnHandleStore);
		}

		public StoreCommand CheckAndSet(Action<ItemBuilderSyntax> item, ulong uniqueID, TimeToLive timeToLive)
		{
			return HandleCommand(new StoreCommand(GetItem(item), timeToLive, uniqueID), OnHandleCheckAndSet);
		}

		private void OnHandleStore(StoreCommand storeCommand)
		{
			var item = storeCommand.Item;
			var commandContext = new CommandContext(storeCommand, item);
			var key = ProcessItemAndBuildKey(commandContext);
			var value = _formattingServices.Serialize(item.Data, commandContext);
			var succeded = _memcachedClient.Store(storeCommand.StoreMode, key, value, storeCommand.TimeToLive);
			storeCommand.SetResult(succeded);
		}

		private void OnHandleCheckAndSet(StoreCommand command)
		{
			var item = command.Item;
			var commandContext = new CommandContext(command, item);
			var key = ProcessItemAndBuildKey(commandContext);
			var value = _formattingServices.Serialize(item.Data, commandContext);
			var casResult = _memcachedClient.Cas(key, value, command.UniqueID, command.TimeToLive);
			command.SetResult(casResult);
		}
		

	
		public DeltaCommand Increment(Action<ItemBuilderSyntax> item, ulong delta)
		{
			return HandleCommand(
				new DeltaCommand(GetItem(item), DeltaMode.Increment, delta),
				OnHandleIncrement);
		}

		private void OnHandleIncrement(DeltaCommand command)
		{
			var item = command.Item;
			var commandContext = new CommandContext(command, item);
			var key = ProcessItemAndBuildKey(commandContext);
			var result = _memcachedClient.Increment(key, command.Delta);
			command.SetResult(result);
		}

		public DeltaCommand Decrement(Action<ItemBuilderSyntax> item, ulong delta)
		{
			return HandleCommand(
				new DeltaCommand(GetItem(item), DeltaMode.Decrement, delta),
				OnHandleDecrement);
		}

		private void OnHandleDecrement(DeltaCommand command)
		{
			var item = command.Item;
			var commandContext = new CommandContext(command, item);
			var key = ProcessItemAndBuildKey(commandContext);
			var result = _memcachedClient.Decrement(key, command.Delta);
			command.SetResult(result);
		}



		public RemoveCommand Remove(Action<ItemBuilderSyntax> item)
		{
			return HandleCommand(new RemoveCommand(GetItem(item)), OnHandleRemove);
		}

		private void OnHandleRemove(RemoveCommand command)
		{
			var item = command.Item;
			var commandContext = new CommandContext(command, item);
			var key = ProcessItemAndBuildKey(commandContext);
			var deleted = _memcachedClient.Delete(key);
			command.SetResult(deleted);
		}


		public void FlushAll(TimeSpan delay)
		{
			_memcachedClient.FlushAll(delay);
		}

		public void FlushAll()
		{
			FlushAll(TimeSpan.Zero);
		}

		
		
		private T HandleCommand<T>(T command, Action<T> defaultHandler)
		{
			var handled = OnHandle(command);
			if(!handled)
			{
				defaultHandler(command);
			}
			return command;
		}
		
		protected virtual bool OnHandle(object command)
		{
			return false;
		}
		
		private CacheItem GetItem(Action<ItemBuilderSyntax> itemBuilder)
		{
			var ib = new ItemBuilderSyntax();
			itemBuilder(ib);
			return ib.BuildItem();
		}

		private Key ProcessItemAndBuildKey(CommandContext executingCommand)
		{
			_itemHandlerPipeline.Process(executingCommand.Item, executingCommand);
			var key =  _keyBuilder.Build(executingCommand.Item, executingCommand);
			return key;
		}
	}
}