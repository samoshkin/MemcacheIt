namespace MemcacheIt.Ideas
{
	public enum StoreFailureReason
	{
		None = 0,

		KeyExists,
		KeyDoesNotExist
	}

/*
	public class StoreOperationHandler : IOperationHandler<StoreCommand>
	{
		private readonly ItemHandlerPipeline _itemHandlerPipeline;
		private readonly KeyBuilder _keyBuilder;
		private readonly FormattingService _formatter;
		private readonly MemcachedClient _client;

		public StoreOperationHandler(
			ItemHandlerPipeline itemHandlerPipeline,
			KeyBuilder keyBuilder,
			FormattingService formatter,
			MemcachedClient client)
		{
			_itemHandlerPipeline = itemHandlerPipeline;
			_keyBuilder = keyBuilder;
			_client = client;
			_formatter = formatter;

			// TODO: init operation factory
		}

		public ItemHandlerPipeline	ItemHandlerPipeline { get { return _itemHandlerPipeline; } }
		public KeyBuilder			KeyBuilder { get { return _keyBuilder; } }
		public FormattingService	Formatter { get { return _formatter; } }
		public MemcachedClient		Client { get { return _client; } }
		

		public void Handle(StoreCommand command)
		{
			var item = command.BuildItem();
			var operationContext = new CommandContext(command, item);

			ItemHandlerPipeline.Process(item, operationContext);
			var key = KeyBuilder.Build(item, operationContext);
			var value = Formatter.Serialize(item.Data, item, operationContext);
			StoreItem(command, item, key, value);
		}

		private void StoreItem(StoreCommand command, CacheItem item, Key key, object value)
		{
			var stored = StoreNeverExpiringItem(command, item, key, value)
				?? StoreExpiresAtItem(command, item, key, value)
				?? StoreValidForItem(command, item, key, value)
				?? StoreNeverExpiringItemWithCAS(command, item, key, value)
				?? StoreExpiresAtItemWithCAS(command, item, key, value)
				?? StoreValidForItemWithCAS(command, item, key, value);
			if(stored == null)
			{
				throw new CachingException(
					"Cannot select store strategy for item '{0}' and operation '{1}'."
					.FormatString(item, command));
			}
		}


		private bool? StoreNeverExpiringItemWithCAS(StoreCommand command, CacheItem item, Key key, object value)
		{
			if(command.CheckAndSave && command.TimeToLive.NeverExpires)
			{
				var response = _client.Cas(command.StoreMode, key, value, item.Cas);
				ProcessCasResponse(response, command, item);
				return true;
			}
			return null;
		}

		private bool? StoreExpiresAtItemWithCAS(StoreCommand command, CacheItem item, Key key, object value)
		{
			if(command.CheckAndSave && command.TimeToLive.DoesExpireAtAbsoluteTime)
			{
				var response = _client.Cas(command.StoreMode, key, value, command.TimeToLive.ExpiresAt, item.Cas);
				ProcessCasResponse(response, command, item);
				return true;
			}
			return null;
		}

		private bool? StoreValidForItemWithCAS(StoreCommand command, CacheItem item, Key key, object value)
		{
			if (command.CheckAndSave && command.TimeToLive.DoesExpireAtRelativeTime)
			{
				var response = _client.Cas(command.StoreMode, key, value, command.TimeToLive.ValidFor, item.Cas);
				ProcessCasResponse(response, command, item);
				return true;
			}
			return null;
		}

		private bool? StoreNeverExpiringItem(StoreCommand command, CacheItem item, Key key, object value)
		{
			if (!command.CheckAndSave && command.TimeToLive.NeverExpires)
			{
				var response = _client.Store(command.StoreMode, key, value);
				ProcessResponse(response, command, item);
				return true;
			}
			return null;
		}

		private bool? StoreExpiresAtItem(StoreCommand command, CacheItem item, Key key, object value)
		{
			if (!command.CheckAndSave && command.TimeToLive.DoesExpireAtAbsoluteTime)
			{
				var response = _client.Store(command.StoreMode, key, value, command.TimeToLive.ExpiresAt);
				ProcessResponse(response, command, item);
				return true;
			}
			return null;
		}

		private bool? StoreValidForItem(StoreCommand command, CacheItem item, Key key, object value)
		{
			if (!command.CheckAndSave && command.TimeToLive.DoesExpireAtRelativeTime)
			{
				var response = _client.Store(command.StoreMode, key, value, command.TimeToLive.ValidFor);
				ProcessResponse(response, command, item);
				return true;
			}
			return null;
		}


		private void ProcessCasResponse(CasResult<bool> casResponse, StoreCommand command, CacheItem item)
		{
			if (casResponse.Result)
			{
				item.Cas = casResponse.Cas;
			}
			ProcessResponse(casResponse.Result, command, item);
		}

		private void ProcessResponse(bool response, StoreCommand command, CacheItem item)
		{
			if(response)
			{
				command.ItemIsStored(item);
			}
			else
			{
				StoreFailureReason failureReason; 
				switch (command.StoreMode)
				{
					case StoreMode.Add:
						failureReason = StoreFailureReason.KeyExists;
						break;
					case StoreMode.Replace:
						failureReason = StoreFailureReason.KeyDoesNotExist;
						break;
					case StoreMode.Set:
						throw OnItemWasNotStoredInSetMode(item);
					default:
						throw new NotSupportedException("Store mode '{0}' is not supported".FormatString(command.StoreMode));
				}
				command.ItemNotStored(item, failureReason);
			}
		}

		private Exception OnItemWasNotStoredInSetMode(CacheItem item)
		{
			return new CachingException(
				"Item '{0}' was not stored in '{1}' store mode. Seemsm, memcached client does something wrong.".FormatString(
				item,
				StoreMode.Set));
		}
	}
*/
}