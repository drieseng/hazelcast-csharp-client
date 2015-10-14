﻿using System.IO;
using Hazelcast.Config;
using NUnit.Framework;
using Properties;

namespace Hazelcast.Client.Test.Config
{
    [TestFixture]
    public class ClientXmlTest
    {
        [Test]
        public virtual void TestXmlParserWithConfigFile()
        {
            ClientConfig clientConfig = XmlClientConfigBuilder.Build(@"..\..\..\Hazelcast.Net\Resources\hazelcast-client-full.xml");

            Assert.NotNull(clientConfig);
        }

        [Test]
        public virtual void TestXmlParserWithReader()
        {
            var clientConfig = XmlClientConfigBuilder.Build(new StringReader(Resources.hazelcast_config_full));
            Assert.NotNull(clientConfig);
        }

        [Test]
        public virtual void TestXmlParserDefault()
        {
            ClientConfig clientConfig = XmlClientConfigBuilder.Build();

            Assert.NotNull(clientConfig);
        }

        //[Test]
        public virtual void TestConfig()
        {
            var config = new ClientConfig();
            var networkConfig = new ClientNetworkConfig();
            networkConfig.SetAddresses(new[] { "127.0.0.1:5701" });
            config.SetNetworkConfig(networkConfig);
            config.SetGroupConfig(new GroupConfig("mike-local", "password"));
            var _client = HazelcastClient.NewHazelcastClient(config);


            Assert.NotNull(_client);

            _client.Shutdown();
        }
    }
}