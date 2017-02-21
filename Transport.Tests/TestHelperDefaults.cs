using System;
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
                    bw.Write((byte)0); // Prefix size
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

            public static DataPack DataPack2()
            {
                string prefix = "ABC";
                DateTime? dateTime = new DateTime(2017, 02, 21, 11, 35, 45, 999);
                string description = "Descr";
                
                var headers = new Headers();
                headers.Add("H", "V");
                
                var properties = new Properties();
                properties.AddOrReplace("P", "V2");
                
                var obj = DataPackHelper.Create(prefix, dateTime, description, headers, properties, new [] { DataPart2() });

                return obj;
            }

            public static DataPart DataPart1()
            {
                var stub = IStreamContainerHelper.CreateMock(MockBehavior.Strict);
                stub.Setup(o => o.GetStream())
                    .Returns(
                        () =>
                        {
                            var stream = new MemoryStream();
                            stream.WriteByte(170);
                            stream.WriteByte(255);
                            stream.WriteByte(255);
                            stream.WriteByte(255);
                            stream.WriteByte(171);
                            stream.Position = 0;

                            return stream;
                        });
                stub.Setup(o => o.CopyToStream(It.IsAny<Stream>()))
                    .Callback<Stream>(output =>
                    {
                        output.WriteByte(170);
                        output.WriteByte(255);
                        output.WriteByte(255);
                        output.WriteByte(255);
                        output.WriteByte(171);
                    });

                var obj = DataPartHelper.Create(stub.Object, null, null);

                return obj;
            }

            public static DataPart DataPart2()
            {
                var stub = IStreamContainerHelper.CreateMock(MockBehavior.Strict);
                stub.Setup(o => o.GetStream())
                    .Returns(
                        () =>
                        {
                            var stream = new MemoryStream();
                            stream.WriteByte(170);
                            stream.WriteByte(255);
                            stream.WriteByte(255);
                            stream.WriteByte(255);
                            stream.WriteByte(171);
                            stream.Position = 0;

                            return stream;
                        });
                stub.Setup(o => o.CopyToStream(It.IsAny<Stream>()))
                    .Callback<Stream>(output =>
                    {
                        output.WriteByte(170);
                        output.WriteByte(255);
                        output.WriteByte(255);
                        output.WriteByte(255);
                        output.WriteByte(171);
                    });

                var headers = new Headers();
                headers.Add("H1", "V1");
                headers.Add("H2", "V2");

                var properties = new Properties();
                properties.AddOrReplace("P1", "V1");
                properties.AddOrReplace("P2", "V2");

                var obj = DataPartHelper.Create(stub.Object, headers, properties);

                return obj;
            }

            


            /// <summary>
            /// Stream of DataPack2()
            /// </summary>
            /// <returns></returns>
            public static Stream Stream2()
            {
                var obj = new MemoryStream();
                using (var wraper = new NonClosingStreamWrapper(obj))
                using (var bw = new BinaryWriter(wraper))
                {
                    bw.Write((byte)3); // Prefix size
                    bw.Write((byte)65);
                    bw.Write((byte)66);
                    bw.Write((byte)67);

                    bw.Write((byte)0); // No sign
                    
                    bw.Write((byte)1); // Start of Info section
                    bw.Write((uint)57); // Size of Info section
                    bw.Write((ushort)2); // Count of inner properties
                    bw.Write((byte)0x21);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x44);
                    bw.Write((byte)0x61);
                    bw.Write((byte)0x74);
                    bw.Write((byte)0x65);
                    bw.Write((byte)0x43);
                    bw.Write((byte)0x72);
                    bw.Write((byte)0x65);
                    bw.Write((byte)0x61);
                    bw.Write((byte)0x74);
                    bw.Write((byte)0x65);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x13);
                    bw.Write((byte)0x32);
                    bw.Write((byte)0x30);
                    bw.Write((byte)0x31);
                    bw.Write((byte)0x37);
                    bw.Write((byte)0x30);
                    bw.Write((byte)0x32);
                    bw.Write((byte)0x32);
                    bw.Write((byte)0x31);
                    bw.Write((byte)0x54);
                    bw.Write((byte)0x31);
                    bw.Write((byte)0x31);
                    bw.Write((byte)0x33);
                    bw.Write((byte)0x35);
                    bw.Write((byte)0x34);
                    bw.Write((byte)0x35);
                    bw.Write((byte)0x2E);
                    bw.Write((byte)0x39);
                    bw.Write((byte)0x39);
                    bw.Write((byte)0x39);
                    bw.Write((byte)0x14);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x0B);
                    bw.Write((byte)0x44);
                    bw.Write((byte)0x65);
                    bw.Write((byte)0x73);
                    bw.Write((byte)0x63);
                    bw.Write((byte)0x72);
                    bw.Write((byte)0x69);
                    bw.Write((byte)0x70);
                    bw.Write((byte)0x74);
                    bw.Write((byte)0x69);
                    bw.Write((byte)0x6F);
                    bw.Write((byte)0x6E);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x05);
                    bw.Write((byte)0x44);
                    bw.Write((byte)0x65);
                    bw.Write((byte)0x73);
                    bw.Write((byte)0x63);
                    bw.Write((byte)0x72);

                    bw.Write((byte)1); // Start of Info section 2
                    bw.Write((uint)9); // Size of Info section 2
                    bw.Write((ushort)1); // Count of headers
                    bw.Write((byte)0x06);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x01);
                    bw.Write((byte)0x48);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x01);
                    bw.Write((byte)0x56);

                    bw.Write((byte)1); // Start of Info section 3
                    bw.Write((uint)10); // Size of Info section 3
                    bw.Write((ushort)1); // Count of public properties
                    bw.Write((byte)0x07);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x01);
                    bw.Write((byte)0x50);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x56);
                    bw.Write((byte)0x32);

                    bw.Write((byte)1); // Start of Info section 4
                    bw.Write((uint)30); // Size of Info section 4
                    bw.Write((ushort)1); // Count of DataPart's

                    bw.Write((uint)136); // Headers address
                    bw.Write((ushort)2); // Headers count
                    bw.Write((uint)18); // Headers size
                    bw.Write((uint)154); // Properties address
                    bw.Write((ushort)2); // Properties count
                    bw.Write((uint)18); // Properties size
                    bw.Write((uint)172); // Data address
                    bw.Write((uint)5); // Data size

                    bw.Write((byte)2); // Start of Data section
                    bw.Write((uint)41); // Size of Data section

                    bw.Write((byte)0x08);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x48);
                    bw.Write((byte)0x31);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x56);
                    bw.Write((byte)0x31);

                    bw.Write((byte)0x08);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x48);
                    bw.Write((byte)0x32);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x56);
                    bw.Write((byte)0x32);

                    bw.Write((byte)0x08);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x50);
                    bw.Write((byte)0x31);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x56);
                    bw.Write((byte)0x31);

                    bw.Write((byte)0x08);
                    bw.Write((byte)0x0A);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x50);
                    bw.Write((byte)0x32);
                    bw.Write((byte)0x12);
                    bw.Write((byte)0x02);
                    bw.Write((byte)0x56);
                    bw.Write((byte)0x32);

                    bw.Write((byte)170);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)171);
                }

                return obj;
            }

            /// <summary>
            /// Empty DataPack with one minimal DataPart
            /// </summary>
            /// <returns></returns>
            public static DataPack DataPack3()
            {
                var obj = DataPackHelper.Create(null, null, null, null, null, new[] { DataPart1() });

                return obj;
            }

            /// <summary>
            /// Stream of DataPack3()
            /// </summary>
            /// <returns></returns>
            public static Stream Stream3()
            {
                var obj = new MemoryStream();
                using (var wraper = new NonClosingStreamWrapper(obj))
                using (var bw = new BinaryWriter(wraper))
                {
                    bw.Write((byte)0); // Prefix size
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

                    bw.Write((uint)0); // Headers address
                    bw.Write((ushort)0); // Headers count
                    bw.Write((uint)0); // Headers size
                    bw.Write((uint)0); // Properties address
                    bw.Write((ushort)0); // Properties count
                    bw.Write((uint)0); // Properties size
                    bw.Write((uint)63); // Data address
                    bw.Write((uint)5); // Data size

                    bw.Write((byte)2); // Start of Data section
                    bw.Write((uint)5); // Size of Data section

                    bw.Write((byte)170);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)171);
                }

                return obj;
            }
        }
    }
}