//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
//

namespace Microsoft.Web.Redis
{
    internal class ValueWrapper
    {
        object _actualValue;
        private readonly string key;
        byte[] _serializedvalue;

        public ValueWrapper(string key, byte[] serializedvalue)
        {
            this.key = key;
            _serializedvalue = serializedvalue;
        }

        public ValueWrapper(string key, object actualValue)
        {
            this.key = key;
            _actualValue = actualValue;
        }

        public object GetActualValue(RedisUtility utility)
        {
            if (_actualValue == null)
            {
                _actualValue = utility.GetObjectFromBytes(key, _serializedvalue);
            }
            return _actualValue;
        }

        public void SetActualValue(object actualValue)
        {
            _actualValue = actualValue;
            // Null serialized value just for completeness
            _serializedvalue = null;
        }

        // This method should be used in test projects only
        internal object GetActualValue()
        {
            return _actualValue;
        }

        // This method should be used in test projects only
        internal object GetSerializedvalue()
        {
            return _serializedvalue;
        }
    }
}
