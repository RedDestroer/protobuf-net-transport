using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf.Transport;

namespace Transport.Tests
{
    public partial class TestHelper
    {
        public static class Assertion
        {
            public static void AreEqual(Stream expected, Stream actual)
            {
                Assert.IsNotNull(expected, "Expected stream is null");
                Assert.IsNotNull(actual, "Actual stream is null");
                Assert.AreEqual(expected.Length, actual.Length, "Length of streams doesn't match");
                Assert.AreEqual(expected.Position, actual.Position, "Position of streams doesn't match");

                expected.Position = 0;
                actual.Position = 0;

                for (long i = 0; i < expected.Length; i++)
                {
                    int e = expected.ReadByte();
                    int a = actual.ReadByte();
                    Assert.AreEqual(e, a, "Bytes at position {0} (0x{0:X}) doesn't match (0x{1:X} != 0x{2:X})", i, e, a);
                }
            }

            public static void AreEqual(byte[] expected, byte[] actual)
            {
                Assert.AreEqual(expected.Length, actual.Length);
                
                for (long i = 0; i < expected.Length; i++)
                {
                    int e = expected[i];
                    int a = actual[i];
                    Assert.AreEqual(e, a, "Bytes at position {0} (0x{0:X}) doesn't match (0x{1:X} != 0x{2:X})", i, e, a);
                }
            }

            public static void AreEqual(DataPack expected, DataPack actual)
            {
                Assert.AreEqual(expected.PrefixSize, actual.PrefixSize);
                Assert.AreEqual(expected.DateCreate, actual.DateCreate);
                Assert.AreEqual(expected.Description, actual.Description);
                AreEqual(expected.Headers, actual.Headers);
                AreEqual(expected.Properties, actual.Properties);
                AreEqual(expected.DataParts, actual.DataParts);
            }

            public static void AreEqual(IList<DataPart> expected, IList<DataPart> actual)
            {
                Assert.AreEqual(expected.Count, actual.Count);

                for (int i = 0; i < expected.Count; i++)
                {
                    AreEqual(expected[i], actual[i]);
                }
            }

            public static void AreEqual(DataPart expected, DataPart actual)
            {
                AreEqual(expected.Headers, actual.Headers);
                AreEqual(expected.Properties, actual.Properties);
                AreEqual(expected.CreateStream(), actual.CreateStream());
            }

            public static void AreEqual(Properties expected, Properties actual)
            {
                Assert.AreEqual(expected.Count, actual.Count);

                var expectedList = expected.GetPropertiesList();
                var actualList = actual.GetPropertiesList();

                for (int i = 0; i < expected.Count; i++)
                {
                    AreEqual(expectedList[i], actualList[i]);
                }
            }

            public static void AreEqual(Headers expected, Headers actual)
            {
                Assert.AreEqual(expected.Count, actual.Count);

                for (int i = 0; i < expected.Count; i++)
                {
                    AreEqual(expected[i], actual[i]);
                }
            }

            public static void AreEqual(DataPair expected, DataPair actual)
            {
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Value, actual.Value);
            }
        }
    }
}