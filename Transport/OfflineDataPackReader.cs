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
        protected override void ReadDataParts(DataPack dataPack, BinaryReader br, List<DataPartInfo> dataPartInfos)
        {
            var stream = br.BaseStream;
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