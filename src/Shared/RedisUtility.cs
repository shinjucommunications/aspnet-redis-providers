//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
//

using System;
using System.Collections.Generic;

namespace Microsoft.Web.Redis
{
    internal sealed class RedisUtility
    {
        public static ICustomSerializable[] CustomSerializers = new ICustomSerializable[0];

        private readonly ProviderConfiguration _configuration;
        internal readonly ISerializer _serializer;

        public RedisUtility(ProviderConfiguration configuration)
        {
            _configuration = configuration;
            _serializer = GetSerializer();
        }

        private ISerializer GetSerializer()
        {
            string serializerTypeName = _configuration.RedisSerializerType;
            if (!string.IsNullOrWhiteSpace(serializerTypeName))
            {
                var serializerType = Type.GetType(serializerTypeName, true);
                if (serializerType != null)
                {
                    return (ISerializer)Activator.CreateInstance(serializerType);
                }
            }
            return new BinarySerializer();
        }

        public int AppendRemoveItemsInList(ChangeTrackingSessionStateItemCollection sessionItems, List<object> list)
        {
            int noOfItemsRemoved = 0;
            if (sessionItems.GetDeletedKeys() != null && sessionItems.GetDeletedKeys().Count != 0)
            {
                foreach (string delKey in sessionItems.GetDeletedKeys())
                {
                    list.Add(delKey);
                    noOfItemsRemoved++;
                }
            }
            return noOfItemsRemoved;
        }

        public int AppendUpdatedOrNewItemsInList(ChangeTrackingSessionStateItemCollection sessionItems, List<object> list)
        {
            int noOfItemsUpdated = 0;
            if (sessionItems.GetModifiedKeys() != null && sessionItems.GetModifiedKeys().Count != 0)
            {
                foreach (string key in sessionItems.GetModifiedKeys())
                {
                    list.Add(key);
                    list.Add(GetBytesFromObject(key, sessionItems[key]));
                    noOfItemsUpdated++;
                }
            }
            return noOfItemsUpdated;
        }

        public List<object> GetNewItemsAsList(ChangeTrackingSessionStateItemCollection sessionItems)
        {
            List<object> list = new List<object>(sessionItems.Keys.Count * 2);
            foreach (string key in sessionItems.Keys)
            {
                list.Add(key);
                list.Add(GetBytesFromObject(key, sessionItems[key]));
            }
            return list;
        }

        internal byte[] GetBytesFromObject(string key, object data)
        {
            foreach (ICustomSerializable customSerializer in CustomSerializers)
            {
                if (customSerializer.MatchKey(key, data))
                {
                    return customSerializer.GetBytesFromObject(data);
                }
            }

            return _serializer.Serialize(data);
        }

        internal object GetObjectFromBytes(string key, byte[] dataAsBytes)
        {
            foreach (ICustomSerializable customSerializer in CustomSerializers)
            {
                if (customSerializer.MatchKey(key))
                {
                    return customSerializer.GetObjectFromBytes(dataAsBytes);
                }
            }

            return _serializer.Deserialize(dataAsBytes);
        }
    }
}
