﻿using System.Collections.Generic;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public class OnlineDataPackReader
        : DataPackReader
    {
        protected override void ReadDataParts(DataPack dataPack, BinaryReader br, List<DataPartInfo> dataPartInfos)
        {
            var stream = br.BaseStream;
            foreach (var dataPartInfo in dataPartInfos)
            {
                IStreamGetter streamGetter;
                using (var filteredStream = new FilteredStream(stream, dataPartInfo.DataAddress, dataPartInfo.DataSize))
                {
                    streamGetter = new OnlineStreamGetter(filteredStream);
                }

                var dataPart = new DataPart(streamGetter);

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