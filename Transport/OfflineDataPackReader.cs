﻿using System.Collections.Generic;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Offline reader for files, written in protobuf-net-transport format. It provides functionality to read data from DataParts even if source stream is already closed
    /// </summary>
    public class OfflineDataPackReader
        : DataPackReader
    {
        /// <summary>
        /// Read all data parts from <see cref="DataPack"/>
        /// </summary>
        /// <param name="dataPack"></param>
        /// <param name="br">Binary reader</param>
        /// <param name="dataPartInfos">List of datapart information</param>
        /// <param name="stream">Stream of transport container</param>
        protected override void ReadDataParts(DataPack dataPack, BinaryReader br, List<DataPartInfo> dataPartInfos, Stream stream)
        {
            foreach (var dataPartInfo in dataPartInfos)
            {
                IDataContainer dataContainer;
                using (var filteredStream = new FilteredStream(stream, dataPartInfo.DataAddress, dataPartInfo.DataSize))
                {
                    dataContainer = new TempFileDataContainer(filteredStream);
                }

                var dataPart = new DataPart(dataContainer);

                if (dataPartInfo.HeadersCount > 0)
                {
                    stream.Seek(dataPartInfo.HeadersAddress, SeekOrigin.Begin);
                    using (var filter = new FilteredStream(stream, dataPartInfo.HeadersAddress, dataPartInfo.HeadersSize))
                    {
                        for (ushort i = 0; i < dataPartInfo.HeadersCount; i++)
                        {
                            dataPart.Headers.Add(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                        }
                    }
                }

                if (dataPartInfo.PropertiesCount > 0)
                {
                    stream.Seek(dataPartInfo.PropertiesAddress, SeekOrigin.Begin);
                    using (var filter = new FilteredStream(stream, dataPartInfo.PropertiesAddress, dataPartInfo.PropertiesSize))
                    {
                        for (ushort i = 0; i < dataPartInfo.PropertiesCount; i++)
                        {
                            dataPart.Properties.AddOrReplace(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                        }
                    }
                }

                dataPack.DataParts.Add(dataPart);
            }
        }
    }
}