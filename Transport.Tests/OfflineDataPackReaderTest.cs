using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf.Transport;

namespace Transport.Tests
{
    [TestClass]
    public class OfflineDataPackReaderTest
    {
        [TestMethod]
        public void Read_DefaultDataStream_GetsExpectedDataPack()
        {
            var target = new OfflineDataPackReader();

            DataPack actual;
            using (var stream = TestHelper.Defaults.Stream1())
            {
                stream.Position = 0;
                actual = target.Read(stream, (string)null);
            }

            Assert.AreEqual(actual.PrefixSize, (byte)0);
            Assert.IsNull(actual.DateCreate, "DateCreate is not null");
            Assert.IsNull(actual.Description, "Description is not null");
            Assert.AreEqual(actual.Headers.Count, 0);
            Assert.AreEqual(actual.Properties.Count, 0);
            Assert.AreEqual(actual.DataParts.Count, 0);
        }

        [TestMethod]
        public void Read_SerializeAndDeserializeDataPack1_GetsExpectedDataPack()
        {
            var target = new OfflineDataPackReader();
            var writer = new OfflineDataPackWriter();

            DataPack expected = TestHelper.Defaults.DataPack1();
            DataPack actual;
            using (var stream = new MemoryStream())
            {
                writer.Write(expected, stream);
                stream.Position = 0;
                actual = target.Read(stream, expected.GetPrefix());
            }

            TestHelper.Assertion.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Read_SerializeAndDeserializeDataPack2_GetsExpectedDataPack()
        {
            var target = new OfflineDataPackReader();
            var writer = new OfflineDataPackWriter();

            DataPack expected = TestHelper.Defaults.DataPack2();
            DataPack actual;
            using (var stream = new MemoryStream())
            {
                writer.Write(expected, stream);

                WriteAllBytes(@"W:\Temp\stream.bin", stream);

                stream.Position = 0;
                actual = target.Read(stream, expected.GetPrefix());
            }

            TestHelper.Assertion.AreEqual(expected, actual);
        }

        private void WriteAllBytes(string fullFileName, Stream stream)
        {
            var pos = stream.Position;
            stream.Position = 0;
            using (var output = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.CopyTo(output);
                output.Flush();
            }

            stream.Position = pos;
        }
    }
}