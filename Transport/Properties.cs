using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProtoBuf.Transport
{
    [DebuggerDisplay("Count = {Count}")]
    public class Properties
    {
        private readonly IList<DataPair> _dataPairs;

        public Properties()
            : this(new List<DataPair>())
        {
        }

        public Properties(IEnumerable<DataPair> dataPairs)
        {
            if (dataPairs == null) throw new ArgumentNullException("dataPairs");

            _dataPairs = new List<DataPair>();
            AddDataPairs(dataPairs);
        }

        public int Count
        {
            get { return _dataPairs.Count; }
        }

        public string this[string propertyName]
        {
            get { return GetPropertyValue(propertyName); }
            set
            {
                AddOrReplace(propertyName, value);
            }
        }

        public bool Contains(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            for (int i = 0; i < _dataPairs.Count; i++)
            {
                if (string.Equals(propertyName, _dataPairs[i].Name, StringComparison.InvariantCulture))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(string propertyName, string propertyValue)
        {
            string pv;
            if (TryGetPropertyValue(propertyName, out pv))
                return string.Equals(propertyValue, pv, StringComparison.InvariantCulture);

            return false;
        }

        public void AddOrReplace(DataPair dataPair)
        {
            if (dataPair == null) throw new ArgumentNullException("dataPair");
#if NET20 || NET30 || NET35
            if (StringExtension.IsNullOrWhiteSpace(dataPair.Name)) throw new ArgumentException("DataPair.Name must not be null or empty.");
#endif

#if NET40 || NET45
            if (string.IsNullOrWhiteSpace(dataPair.Name)) throw new ArgumentException("DataPair.Name must not be null or empty.");
#endif
            AddDataPair(dataPair);
        }

        public void AddOrReplace(string propertyName, string propertyValue = null)
        {
            AddOrReplace(new DataPair(propertyName, propertyValue));
        }

        public bool Remove(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            for (int i = 0; i < _dataPairs.Count; i++)
            {
                if (string.Equals(propertyName, _dataPairs[i].Name, StringComparison.InvariantCulture))
                {
                    _dataPairs.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public string GetPropertyValue(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            string propertyValue;
            if (TryGetPropertyValue(propertyName, out propertyValue))
                return propertyValue;

            throw new KeyNotFoundException(string.Format("Key not found: '{0}'.", propertyName));
        }

        public bool TryGetPropertyValue(string propertyName, out string propertyValue)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            for (int i = 0; i < _dataPairs.Count; i++)
            {
                if (string.Equals(propertyName, _dataPairs[i].Name, StringComparison.InvariantCulture))
                {
                    propertyValue = _dataPairs[i].Value;
                    return true;
                }
            }

            propertyValue = null;
            return false;
        }

        public string TryGetPropertyValue(string propertyName, string @default)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            for (int i = 0; i < _dataPairs.Count; i++)
            {
                if (string.Equals(propertyName, _dataPairs[i].Name, StringComparison.InvariantCulture))
                    return _dataPairs[i].Value;
            }
            
            return @default;
        }
        
        public IDictionary<string, string> GetProperties()
        {
            var result = new Dictionary<string, string>();
            foreach (var dataPair in _dataPairs)
            {
                result[dataPair.Name] = dataPair.Value;
            }

            return result;
        }

        public IList<DataPair> GetPropertiesList()
        {
            var result = new List<DataPair>();
            foreach (var kv in _dataPairs)
            {
                result.Add(kv.Clone());
            }

            return result;
        }

        private void AddDataPairs(IEnumerable<DataPair> dataPairs)
        {
            foreach (var dataPair in dataPairs)
            {
                AddDataPair(dataPair);
            }
        }

        private void AddDataPair(DataPair dataPair)
        {
            for (int i = 0; i < _dataPairs.Count; i++)
            {
                if (string.Equals(dataPair.Name, _dataPairs[i].Name, StringComparison.InvariantCulture))
                {
                    _dataPairs[i] = dataPair;
                    return;
                }
            }

            _dataPairs.Add(dataPair);
        }
    }
}
