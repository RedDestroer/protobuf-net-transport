using System.IO;
using Moq;
using ProtoBuf.Transport;

namespace Transport.Tests
{
    public partial class TestHelper
    {
        public static class Defaults
        {
            /// <summary>
            /// Empty DataPack
            /// </summary>
            /// <returns></returns>
            public static DataPack DataPack1()
            {
                var obj = DataPackHelper.Create(null, null, null, null, null, null);

                return obj;
            }

            /// <summary>
            /// Minimal stream for empty DataPack
            /// </summary>
            /// <returns></returns>
            public static Stream Stream1()
            {
                var obj = new MemoryStream();
                using (var wraper = new NonClosingStreamWrapper(obj))
                using (var bw = new BinaryWriter(wraper))
                {
                    bw.Write((byte)255); // No prefix, so we begin with 0xFF
                    bw.Write((byte)0); // No sign
                    bw.Write((byte)1); // Start of Info section
                    bw.Write((uint)2); // Size of Info section
                    bw.Write((ushort)0); // Count of inner properties
                    bw.Write((byte)1); // Start of Info section 2
                    bw.Write((uint)2); // Size of Info section 2
                    bw.Write((ushort)0); // Count of headers
                    bw.Write((byte)1); // Start of Info section 3
                    bw.Write((uint)2); // Size of Info section 3
                    bw.Write((ushort)0); // Count of public properties
                    bw.Write((byte)1); // Start of Info section 4
                    bw.Write((uint)2); // Size of Info section 4
                    bw.Write((ushort)0); // Count of DataPart's
                    bw.Write((byte)2); // Start of Data section
                    bw.Write((uint)0); // Size of Data section
                }
                
                return obj;
            }

            public static DataPack DataPackWithOneDataPart()
            {
                var obj = DataPackHelper.Create(null, null, null, null, null, new [] { DataPart1() });

                return obj;
            }

            public static DataPart DataPart1()
            {
                var stream = new MemoryStream();
                stream.WriteByte(255);
                stream.WriteByte(255);
                stream.WriteByte(255);
                stream.WriteByte(255);
                stream.WriteByte(255);
                stream.Position = 0;
                var stub = IStreamGetterHelper.CreateMock(MockBehavior.Strict);
                stub.Setup(o => o.CreateStream())
                    .Returns(stream);
                var obj = DataPartHelper.Create(stub.Object);

                return obj;
            }

            /// <summary>
            /// Stream of DataPackWithOneDataPart()
            /// </summary>
            /// <returns></returns>
            public static Stream Stream2()
            {
                var obj = new MemoryStream();
                using (var wraper = new NonClosingStreamWrapper(obj))
                using (var bw = new BinaryWriter(wraper))
                {
                    bw.Write((byte)255); // No prefix, so we begin with 0xFF
                    bw.Write((byte)0); // No sign
                    bw.Write((byte)1); // Start of Info section
                    bw.Write((uint)2); // Size of Info section
                    bw.Write((ushort)0); // Count of inner properties
                    bw.Write((byte)1); // Start of Info section 2
                    bw.Write((uint)2); // Size of Info section 2
                    bw.Write((ushort)0); // Count of headers
                    bw.Write((byte)1); // Start of Info section 3
                    bw.Write((uint)2); // Size of Info section 3
                    bw.Write((ushort)0); // Count of public properties
                    bw.Write((byte)1); // Start of Info section 4
                    bw.Write((uint)30); // Size of Info section 4
                    bw.Write((ushort)1); // Count of DataPart's

                    bw.Write((uint)0);
                    bw.Write((ushort)0);
                    bw.Write((uint)0);
                    bw.Write((uint)0);
                    bw.Write((ushort)0);
                    bw.Write((uint)0);
                    bw.Write((uint)0);
                    bw.Write((uint)0);

                    bw.Write((byte)2); // Start of Data section
                    bw.Write((uint)5); // Size of Data section

                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                }

                return obj;
            }
        }
    }
}