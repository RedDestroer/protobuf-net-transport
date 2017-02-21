using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf.Transport;

namespace Transport.Tests
{
    [TestClass]
    public class OfflineDataPackWriterTest
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
        public void Write_DataPackWithAllPartsFilled_GetsExpectedStream()
        {
            var target = new OfflineDataPackWriter();
            var dataPack = TestHelper.Defaults.DataPack2();

            using (var expected = TestHelper.Defaults.Stream2())
            using (var actual = new MemoryStream())
            {
                target.Write(dataPack, actual);

                ////TestHelper.WriteAllBytes(@"X:\Temp\expected.bin", expected);
                ////TestHelper.WriteAllBytes(@"X:\Temp\actual.bin", actual);

                TestHelper.Assertion.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Write_DataPackWithOnlyOneDataPart_GetsExpectedStream()
        {
            var target = new OfflineDataPackWriter();
            var dataPack = TestHelper.Defaults.DataPack3();

            using (var expected = TestHelper.Defaults.Stream3())
            using (var actual = new MemoryStream())
            {
                target.Write(dataPack, actual);

                ////TestHelper.WriteAllBytes(@"X:\Temp\expected.bin", expected);
                ////TestHelper.WriteAllBytes(@"X:\Temp\actual.bin", actual);

                TestHelper.Assertion.AreEqual(expected, actual);
            }
        }
    }
}