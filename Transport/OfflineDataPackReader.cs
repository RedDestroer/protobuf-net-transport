using System.Collections.Generic;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public class OfflineDataPackReader
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
                    streamGetter = new TempFileStreamGetter(filteredStream);
                }

                var dataPart = new DataPart(streamGetter);

                stream.Seek(dataPartInfo.PropertiesAddress, SeekOrigin.Begin);
                using (var filter = new FilteredStream(stream, dataPartInfo.PropertiesAddress, dataPartInfo.DataAddress - dataPartInfo.PropertiesAddress))
                {
                    for (ushort i = 0; i < dataPartInfo.PropertiesCount; i++)
                    {
                        dataPart.Properties.AddOrReplace(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                    }
                }

                dataPack.DataParts.Add(dataPart);
            }
        }
    }
}