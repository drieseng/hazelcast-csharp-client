// Copyright (c) 2008-2018, Hazelcast, Inc. All Rights Reserved.
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

using Hazelcast.Client.Protocol;
using Hazelcast.Util;

#pragma warning disable CS1591
 namespace Hazelcast.Client.Spi
{
    /// <summary>
    /// Client service to add/remove remote listeners.
    /// </summary>
    /// <remarks>
    /// For smart client, it registers local  listeners to all nodes in the cluster.
    /// For dummy client, it registers global listener to one node.
    /// </remarks>
    public interface IClientListenerService
    {
        bool AddEventHandler(long correlationId, DistributedEventHandler eventHandler);

        bool RemoveEventHandler(long correlationId);

        string RegisterListener(IClientMessage registrationMessage, DecodeRegisterResponse responseDecoder,
            EncodeDeregisterRequest encodeDeregisterRequest, DistributedEventHandler eventHandler);

        bool DeregisterListener(string userRegistrationId);
        
        void HandleResponseMessage(IClientMessage message);
        
    }
}