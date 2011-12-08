using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using CuttingEdge.Conditions;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace MemcacheIt.Configuration
{
	public class MemcachedClientApiConfiguration : IMemcachedClientConfiguration
	{
		private ISocketPoolConfiguration _socketPoolConfiguration;
		private IMemcachedKeyTransformer _keyTransformer;
		private IMemcachedNodeLocator _nodeLocator;
		private Func<IMemcachedNodeLocator> _nodeLocatorFactory;
		private ITranscoder _transcoder;
		private IAuthenticationConfiguration _authentication;
		private IList<IPEndPoint> _servers;

		private MemcachedClientApiConfiguration(
			IEnumerable<IPEndPoint> servers,
			ISocketPoolConfiguration socketPoolConfiguration,
			IMemcachedKeyTransformer keyTransformer, 
			IMemcachedNodeLocator nodeLocator, 
			Func<IMemcachedNodeLocator> nodeLocatorFactory, 
			ITranscoder transcoder, 
			IAuthenticationConfiguration authentication, 
			MemcachedProtocol protocol, 
			IPerformanceMonitor performanceMonitor)
		{
			Condition.Requires(socketPoolConfiguration, "socket pool configuration").IsNotNull();
			Condition.Requires(keyTransformer, "key transformer").IsNotNull();
			Condition.Requires(transcoder, "transcoder").IsNotNull();
			Condition.Requires(authentication, "authentication").IsNotNull();
			Condition.Requires(nodeLocator, "node locator")
				.Evaluate(!(nodeLocator == null && nodeLocatorFactory == null),
				"Both node locator and node locator factory are not set. Requires only one to be set.");
			Condition.Requires(nodeLocator, "node locator")
				.Evaluate(!(nodeLocator == null && nodeLocatorFactory == null), 
				"Both node locator and node locator are set. Requires only one to be set.");
			Condition.Requires(servers, "servers").IsNotNull();
			
			_socketPoolConfiguration = socketPoolConfiguration;
			_keyTransformer = keyTransformer;
			_nodeLocator = nodeLocator;
			_nodeLocatorFactory = nodeLocatorFactory;
			_transcoder = transcoder;
			_authentication = authentication;
			PerformanceMonitor = performanceMonitor;
			Protocol = protocol;
			_servers = servers.ToList();
		}

		public static MemcachedClientApiConfiguration UseConfigFile()
		{
			var baseConfig = (IMemcachedClientConfiguration) GetMemcachedConfigurationSection();
			return new MemcachedClientApiConfiguration(
				baseConfig.Servers,
				CopySocketPool(baseConfig.SocketPool),
				baseConfig.CreateKeyTransformer(),
				baseConfig.CreateNodeLocator(),
				null,
				baseConfig.CreateTranscoder(),
				baseConfig.Authentication,
				baseConfig is MemcachedClientConfiguration
					? ((MemcachedClientConfiguration) baseConfig).Protocol
					: ((MemcachedClientSection) baseConfig).Protocol,
				baseConfig.CreatePerformanceMonitor());
		}

		public static MemcachedClientApiConfiguration UseDefaults()
		{
			return new MemcachedClientApiConfiguration(
				Enumerable.Empty<IPEndPoint>(),
				new SocketPoolConfiguration(),
				new DefaultKeyTransformer(),
				new DefaultNodeLocator(),
				null,
				new DefaultTranscoder(),
				new AuthenticationConfiguration(),
				MemcachedProtocol.Binary,
				null);
		}

		private static MemcachedClientSection GetMemcachedConfigurationSection()
		{
			var memcachedSection = (MemcachedClientSection) ConfigurationManager.GetSection("enyim.com/memcached");
			if(memcachedSection == null)
			{
				throw new Exception("Config file does not contain memcached section");
			}
			return memcachedSection;
		}

		private static ISocketPoolConfiguration CopySocketPool(ISocketPoolConfiguration source)
		{
			var socketPool = (ISocketPoolConfiguration) new SocketPoolConfiguration();
			socketPool.ConnectionTimeout = source.ConnectionTimeout;
			socketPool.DeadTimeout = source.DeadTimeout;
			socketPool.MaxPoolSize = source.MaxPoolSize;
			socketPool.MinPoolSize = source.MinPoolSize;
			socketPool.QueueTimeout = source.QueueTimeout;
			socketPool.ReceiveTimeout = source.ReceiveTimeout;
			return socketPool;
		}

	

		public IMemcachedKeyTransformer	KeyTransformer
		{
			get { return _keyTransformer; }
			set { _keyTransformer = Condition.Requires(value).IsNotNull().Value; }
		}
	
		public ISocketPoolConfiguration SocketPool 
		{
			get { return _socketPoolConfiguration ?? (_socketPoolConfiguration = new SocketPoolConfiguration()); }
			private set { _socketPoolConfiguration = value; }
		}

		public IMemcachedNodeLocator NodeLocator
		{
			get { return _nodeLocator; }
			set
			{
				_nodeLocator = Condition.Requires(value)
					.Evaluate(v => !(v == null && NodeLocatorFactory == null),
					"Cannot reset both NodeLocator and NodeLocatorFactory.").Value;
			}
		}

		public Func<IMemcachedNodeLocator> NodeLocatorFactory
		{
			get { return _nodeLocatorFactory; }
			set
			{
				_nodeLocatorFactory = Condition.Requires(value)
					.Evaluate(v => !(v == null && NodeLocator == null),
					"Cannot reset both NodeLocator and NodeLocatorFactory.").Value;
			}
		}

		public ITranscoder Transcoder
		{
			get { return _transcoder; }
			set { _transcoder = Condition.Requires(value).IsNotNull().Value; }
		}

		public IAuthenticationConfiguration Authentication
		{
			get { return _authentication; }
			private set { _authentication = Condition.Requires(value).IsNotNull().Value; }
		}

		public IPerformanceMonitor PerformanceMonitor { get; set; }

		public MemcachedProtocol Protocol { get; set; }
		
		
		public IMemcachedKeyTransformer CreateKeyTransformer()
		{
			return KeyTransformer;
		}

		public IMemcachedNodeLocator CreateNodeLocator()
		{
			var nodeLocator = NodeLocator ?? NodeLocatorFactory();
			Condition.WithExceptionOnFailure<InvalidOperationException>()
				.Requires(nodeLocator)
				.IsNotNull("Memcached node locator is not set by NodeLocator property and cannot be built by node locator factory.");
			return nodeLocator;
		}

		public ITranscoder CreateTranscoder()
		{
			return Transcoder;
		}

		public IServerPool CreatePool()
		{
			throw new NotImplementedException();
		}

		public IPerformanceMonitor CreatePerformanceMonitor()
		{
			return PerformanceMonitor;
		}

		public IList<IPEndPoint> Servers
		{
			get { return _servers; }
		}
	}
}