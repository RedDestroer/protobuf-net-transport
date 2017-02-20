using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf.Transport;

namespace Transport.Tests
{
    [TestClass]
    public class DataPackWriterTest
    {
        [TestMethod]
        public void Write_DefaultDataPack_GetsExpectedStream()
        {
            var target = new OfflineDataPackWriter();
            var dataPack = TestHelper.Defaults.DataPack1();

            using (var expected = TestHelper.Defaults.Stream1())
            using (var actual = new MemoryStream())
            {
                target.Write(dataPack, actual);

                TestHelper.Assertion.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Write_DataPackWithOneDataPart_GetsExpectedStream()
        {
            var target = new OfflineDataPackWriter();
            var dataPack = TestHelper.Defaults.DataPackWithOneDataPart();

            using (var expected = TestHelper.Defaults.Stream2())
            using (var actual = new MemoryStream())
            {
                target.Write(dataPack, actual);

                TestHelper.Assertion.AreEqual(expected, actual);
            }
        }
    }
}