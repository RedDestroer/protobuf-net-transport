using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Properties of <see cref="DataPack"/>
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class Properties
    {
        private readonly IList<DataPair> _dataPairs;

        /// <summary>
        /// Creates <see cref="Properties"/> instance
        /// </summary>
        public Properties()
            : this(new List<DataPair>())
        {
        }

        /// <summary>
        /// Creates <see cref="Properties"/> instance
        /// </summary>
        /// <param name="dataPairs">Data pairs for properties</param>
        public Properties(IEnumerable<DataPair> dataPairs)
        {
            if (dataPairs == null) throw new ArgumentNullException("dataPairs");

            _dataPairs = new List<DataPair>();
            AddDataPairs(dataPairs);
        }

        /// <summary>
        /// Count of properties
        /// </summary>
        public int Count
        {
            get { return _dataPairs.Count; }
        }

        /// <summary>
        /// Gets or sets value of property by its name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get { return GetPropertyValue(propertyName); }
            set
            {
                AddOrReplace(propertyName, value);
            }
        }

        /// <summary>
        /// Returns if properties contains property with given name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns if properties contains property with given name and given value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyValue">Property value</param>
        /// <returns></returns>
        public bool Contains(string propertyName, string propertyValue)
        {
            string pv;
            if (TryGetPropertyValue(propertyName, out pv))
                return string.Equals(propertyValue, pv, StringComparison.InvariantCulture);

            return false;
        }

        /// <summary>
        /// Adds or replaces property from data pair
        /// </summary>
        /// <param name="dataPair">Data pair</param>
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

        /// <summary>
        /// Adds or replaces property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyValue">Property value</param>
        public void AddOrReplace(string propertyName, string propertyValue = null)
        {
            AddOrReplace(new DataPair(propertyName, propertyValue));
        }

        /// <summary>
        /// Removes property with given name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets value of given property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public string GetPropertyValue(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            string propertyValue;
            if (TryGetPropertyValue(propertyName, out propertyValue))
                return propertyValue;

            throw new KeyNotFoundException(string.Format("Key not found: '{0}'.", propertyName));
        }

        /// <summary>
        /// Tries to get property value of given property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyValue">Property value</param>
        /// <returns></returns>
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

        /// <summary>
        /// Tries to get property value of given property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="default">Default value if property not found</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Returns dictionary of all properties
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetProperties()
        {
            var result = new Dictionary<string, string>();
            foreach (var dataPair in _dataPairs)
            {
                result[dataPair.Name] = dataPair.Value;
            }

            return result;
        }

        /// <summary>
        /// Returns list of properties as data pairs
        /// </summary>
        /// <returns></returns>
        public IList<DataPair> GetPropertiesList()
        {
            var result = new List<DataPair>();
            foreach (var kv in _dataPairs)
            {
                result.Add(kv.Clone());
            }

            return result;
        }

        /// <summary>
        /// Add property to properties
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Property value</param>
        /// <returns></returns>
        public Properties With(string propertyName, string value = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            AddOrReplace(propertyName, value);

            return this;
        }

        /// <summary>
        /// Add header to headers
        /// </summary>
        /// <param name="item">Data pair</param>
        /// <returns></returns>
        public Properties With(DataPair item)
        {
            if (item == null) throw new ArgumentNullException("item");

            AddOrReplace(item.Name, item.Value);

            return this;
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
