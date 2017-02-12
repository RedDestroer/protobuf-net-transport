using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if NET30 || NET35 || NET40 || NET45
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

#if NET20
            _dataPairs = new Dictionary<string, DataPair>();
#endif

#if NET30 || NET35 || NET40 || NET45
            _dataPairs = new ConcurrentDictionary<string, DataPair>();
#endif

            AddDataPairs(dataPairs);
        }

        public bool Contains(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            return _dataPairs.ContainsKey(propertyName);
        }

        public void AddOrReplace(DataPair dataPair)
        {
            if (dataPair == null) throw new ArgumentNullException("dataPair");

            _dataPairs[dataPair.Name] = dataPair;
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

        public ICollection<DataPair> GetProperties()
        {
            var list = new List<DataPair>();
            foreach (var dataPair in _dataPairs.Values)
            {
                list.Add(dataPair.Clone());
            }

            return new ReadOnlyCollection<DataPair>(list);
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
