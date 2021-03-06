﻿// Copyright (c) 2008-2018, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using Hazelcast.Client;
using Hazelcast.Config;

namespace Hazelcast.Examples.Map
{
    internal class MapNearCacheExample
    {
        public static void Run(string[] args)
        {
            Environment.SetEnvironmentVariable("hazelcast.logging.level", "info");
            Environment.SetEnvironmentVariable("hazelcast.logging.type", "console");

            var config = new ClientConfig();

            var nearCacheConfig = new NearCacheConfig();
            nearCacheConfig.SetMaxSize(1000)
                .SetInvalidateOnChange(true)
                .SetEvictionPolicy("Lru")
                .SetInMemoryFormat(InMemoryFormat.Binary);

            config.AddNearCacheConfig("nearcache-map-*", nearCacheConfig);
            config.GetNetworkConfig().AddAddress("127.0.0.1");

            var client = HazelcastClient.NewHazelcastClient(config);

            var map = client.GetMap<string, string>("nearcache-map-1");

            for (var i = 0; i < 1000; i++)
            {
                map.Put("key" + i, "value" + i);
            }

            var sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < 1000; i++)
            {
                map.Get("key" + i);
            }
            Console.WriteLine("Got values in " + sw.ElapsedMilliseconds + " millis");

            sw.Restart();
            for (var i = 0; i < 1000; i++)
            {
                map.Get("key" + i);
            }
            Console.WriteLine("Got cached values in " + sw.ElapsedMilliseconds + " millis");

            map.Destroy();
            client.Shutdown();
        }
    }
}