using System;
using System.Collections.Concurrent;
using Hazelcast.Logging;
using Hazelcast.Net.Ext;
using Hazelcast.Util;


namespace Hazelcast.Logging
{
	public abstract class LoggerFactorySupport : ILoggerFactory
	{
        internal readonly ConcurrentDictionary<string, ILogger> mapLoggers = new ConcurrentDictionary<string, ILogger>();

        //internal readonly Func<string, ILogger> loggerConstructor;

		public ILogger GetLogger(string name)
		{
            return mapLoggers.GetOrAdd(name, CreateLogger(name));
		}

		protected internal abstract ILogger CreateLogger(string name);

	}
}
