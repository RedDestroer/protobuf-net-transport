using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Headers of <see cref="DataPack"/>
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class Headers
        : IList<DataPair>
    {
        private readonly IList<DataPair> _dataPairs;

        /// <summary>
        /// Creates <see cref="Headers"/> instance
        /// </summary>
        public Headers()
        {
            _dataPairs = new List<DataPair>();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<DataPair> GetEnumerator()
        {
            return _dataPairs.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>
        /// Adds new header
        /// </summary>
        /// <param name="item">Header <see cref="DataPair"/></param>
        public void Add(DataPair item)
        {
            _dataPairs.Add(item);
        }

        /// <summary>
        /// Adds new header
        /// </summary>
        /// <param name="header">Header key</param>
        /// <param name="value">Header value</param>
        public void Add(string header, string value = null)
        {
            Add(new DataPair(header, value));
        }

        /// <summary>
        /// Adds header if it is not exists yet
        /// </summary>
        /// <param name="header">Header key</param>
        public void AddIfNotExists(string header)
        {
            if (header == null) throw new ArgumentNullException("header");

            if (Contains(header))
                return;

            Add(header);
        }

        /// <summary>
        /// Adds header if it is not exists yet
        /// </summary>
        /// <param name="header">Header key</param>
        /// <param name="value">Header value</param>
        public void AddIfNotExists(string header, string value)
        {
            if (header == null) throw new ArgumentNullException("header");

            if (Contains(header, value))
                return;

            Add(header, value);
        }

        /// <summary>
        /// Adds header if it is not exists yet
        /// </summary>
        /// <param name="dataPair">Header <see cref="DataPair"/></param>
        public void AddIfNotExists(DataPair dataPair)
        {
            if (dataPair == null) throw new ArgumentNullException("dataPair");

            if (Contains(dataPair))
                return;

            Add(dataPair);
        }

        /// <summary>
        /// Clear all headers
        /// </summary>
        public void Clear()
        {
            _dataPairs.Clear();
        }

        /// <summary>
        /// Determines whether headers contains a specific one
        /// </summary>
        /// <returns>true if <paramref name="item" /> is found in the headers; otherwise, false</returns>
        /// <param name="item">Header <see cref="DataPair"/></param>
        public bool Contains(DataPair item)
        {
            return Contains(item.Name, item.Value);
        }

        /// <summary>
        /// Determines whether headers contains a specific one with given name
        /// </summary>
        /// <param name="header">Header key</param>
        /// <returns></returns>
        public bool Contains(string header)
        {
            foreach (var dataPair in _dataPairs)
            {
                if (string.Equals(dataPair.Name, header, StringComparison.InvariantCulture))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether headers contains a specific one with given name and value
        /// </summary>
        /// <param name="header">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns></returns>
        public bool Contains(string header, string value)
        {
            foreach (var dataPair in _dataPairs)
            {
                if (string.Equals(dataPair.Name, header, StringComparison.InvariantCulture))
                {
                    if (string.Equals(dataPair.Value, value, StringComparison.InvariantCulture))
                        return true;
                }
            }

            return false;
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(DataPair[] array, int arrayIndex)
        {
            _dataPairs.CopyTo(array, arrayIndex);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(DataPair item)
        {
            return _dataPairs.Remove(item);
        }

        /// <summary>
        /// Removes all headers with given name
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public bool Remove(string header)
        {
            if (header == null) throw new ArgumentNullException("header");

            var toDelete = new List<DataPair>();
            foreach (var dataPair in _dataPairs)
            {
                if (string.Equals(dataPair.Name, header, StringComparison.InvariantCulture))
                    toDelete.Add(dataPair);
            }

            foreach (var dataPair in toDelete)
            {
                Remove(dataPair);
            }

            return toDelete.Count > 0;
        }

        /// <summary>
        /// Removes all headers with given name and value
        /// </summary>
        /// <param name="header"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(string header, string value)
        {
            if (header == null) throw new ArgumentNullException("header");

            var toDelete = new List<DataPair>();
            foreach (var dataPair in _dataPairs)
            {
                if (string.Equals(dataPair.Name, header, StringComparison.InvariantCulture))
                    toDelete.Add(dataPair);
            }

            foreach (var dataPair in toDelete)
            {
                Remove(dataPair);
            }

            return toDelete.Count > 0;
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count { get { return _dataPairs.Count; } }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly { get { return _dataPairs.IsReadOnly; } }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public int IndexOf(DataPair item)
        {
            return _dataPairs.IndexOf(item);
        }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public void Insert(int index, DataPair item)
        {
            _dataPairs.Insert(index, item);
        }

        /// <summary>Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public void RemoveAt(int index)
        {
            _dataPairs.RemoveAt(index);
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public DataPair this[int index]
        {
            get { return _dataPairs[index]; }
            set { _dataPairs[index] = value; }
        }

        /// <summary>
        /// Add header to headers
        /// </summary>
        /// <param name="header">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns></returns>
        public Headers With(string header, string value = null)
        {
            if (header == null) throw new ArgumentNullException("header");

            Add(header, value);

            return this;
        }

        /// <summary>
        /// Add header to headers
        /// </summary>
        /// <param name="item">Data pair</param>
        /// <returns></returns>
        public Headers With(DataPair item)
        {
            if (item == null) throw new ArgumentNullException("item");

            Add(item.Name, item.Value);

            return this;
        }
    }
}