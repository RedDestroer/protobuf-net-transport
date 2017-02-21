using System;
using System.Collections.Generic;

#if NET40 || NET45
using System.Collections.Concurrent;
#endif

namespace ProtoBuf.Transport
{
    public class Properties
    {
        private readonly IDictionary<string, DataPair> _dataPairs;

        public Properties()
            : this(new List<DataPair>())
        {
        }

        public Properties(IEnumerable<DataPair> dataPairs)
        {
            if (dataPairs == null) throw new ArgumentNullException("dataPairs");

#if NET20 || NET30 || NET35
            _dataPairs = new Dictionary<string, DataPair>();
#endif

#if NET40 || NET45
            _dataPairs = new ConcurrentDictionary<string, DataPair>();
#endif

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

            return _dataPairs.ContainsKey(propertyName);
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
            _dataPairs[dataPair.Name] = dataPair;
        }

        public void AddOrReplace(string propertyName, string propertyValue = null)
        {
            AddOrReplace(new DataPair(propertyName, propertyValue));
        }

        public bool Remove(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            return _dataPairs.Remove(propertyName);
        }

        public string GetPropertyValue(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            return _dataPairs[propertyName].Value;
        }

        public bool TryGetPropertyValue(string propertyName, out string propertyValue)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            DataPair dataPair;
            if (_dataPairs.TryGetValue(propertyName, out dataPair))
            {
                propertyValue = dataPair.Value;
                return true;
            }

            propertyValue = null;
            return false;
        }

        public string TryGetPropertyValue(string propertyName, string @default)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            DataPair dataPair;
            if (_dataPairs.TryGetValue(propertyName, out dataPair))
                return dataPair.Value;

            return @default;
        }

        public bool Exists(string propertyName, string propertyValue)
        {
            string pv;
            if (TryGetPropertyValue(propertyName, out pv))
                return string.Equals(propertyValue, pv, StringComparison.InvariantCulture);

            return false;
        }

        public IDictionary<string, string> GetProperties()
        {
            var result = new Dictionary<string, string>();
            foreach (var dataPair in _dataPairs.Values)
            {
                result[dataPair.Name] = dataPair.Value;
            }

            return result;
        }

        public IList<DataPair> GetPropertiesList()
        {
            var result = new List<DataPair>();
            foreach (var dataPair in _dataPairs.Values)
            {
                result.Add(dataPair);
            }

            return result;
        }

        private void AddDataPairs(IEnumerable<DataPair> dataPairs)
        {
            foreach (var dataPair in dataPairs)
            {
                _dataPairs[dataPair.Name] = dataPair;
            }
        }
    }
}
