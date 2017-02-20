using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProtoBuf.Transport;
using ProtoBuf.Transport.Abstract;

namespace Transport.Tests
{
    [TestClass]
    public class TransportSerializerTest
    {
        [TestMethod]
        public void DataPackReader_NotNull()
        {
            Assert.IsNotNull(TransportSerializer.DataPackReader);
        }

        [TestMethod]
        public void DataPackReader_OnNotNullAssign_ReturnsIt()
        {
            var expected = TestHelper.IDataPackReaderHelper.Create();
            TransportSerializer.DataPackReader = expected;
            var actual = TransportSerializer.DataPackReader;

            Assert.IsNotNull(TransportSerializer.DataPackReader);
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void DataPackReader_OnNullAssign_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.DataPackReader = null; }, "value");
        }

        [TestMethod]
        public void DataPackWriter_NotNull()
        {
            Assert.IsNotNull(TransportSerializer.DataPackWriter);
        }

        [TestMethod]
        public void DataPackWriter_OnNotNullAssign_ReturnsIt()
        {
            var expected = TestHelper.IDataPackWriterHelper.Create();
            TransportSerializer.DataPackWriter = expected;
            var actual = TransportSerializer.DataPackWriter;

            Assert.IsNotNull(TransportSerializer.DataPackWriter);
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void DataPackWriter_OnNullAssign_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.DataPackWriter = null; }, "value");
        }

        [TestMethod]
        public void Serialize_WithNullDataPack_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.Serialize(null, new MemoryStream(), TestHelper.ISignAlgorithmHelper.Create()); }, "dataPack");
        }

        [TestMethod]
        public void Serialize_WithNullStream_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.Serialize(TestHelper.DataPackHelper.Create(), null, TestHelper.ISignAlgorithmHelper.Create()); }, "stream");
        }

        [TestMethod]
        public void Deserialize_WithNullStream_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.Deserialize(null); }, "stream");
        }

        [TestMethod]
        public void Deserialize_WithNullStream2_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.Deserialize(null, new byte[] { }); }, "stream");
        }

        [TestMethod]
        public void Deserialize_WithNullStream3_Throws()
        {
            TestHelper.MustThrowArgumentNullException(() => { TransportSerializer.Deserialize(null, TestHelper.NextString()); }, "stream");
        }

        [TestMethod]
        public void Serialize_OnCall_AccessDataPackWriter()
        {
            var p1 = TestHelper.DataPackHelper.Create();
            var p2 = new MemoryStream();
            var p3 = TestHelper.ISignAlgorithmHelper.Create();

            var mock = TestHelper.IDataPackWriterHelper.CreateMock(MockBehavior.Loose);

            TransportSerializer.DataPackWriter = mock.Object;
            TransportSerializer.Serialize(p1, p2, p3);

            mock.Verify(o => o.Write(It.Is<DataPack>(p => ReferenceEquals(p, p1)), It.Is<Stream>(p => ReferenceEquals(p, p2)), It.Is<ISignAlgorithm>(p => ReferenceEquals(p, p3))), Times.Once);
        }

        [TestMethod]
        public void Deserialize_OnCall_AccessDataPackReader()
        {
            var p1 = new MemoryStream();

            var mock = TestHelper.IDataPackReaderHelper.CreateMock(MockBehavior.Loose);

            TransportSerializer.DataPackReader = mock.Object;
            TransportSerializer.Deserialize(p1);

            mock.Verify(o => o.Read(It.Is<Stream>(p => ReferenceEquals(p, p1)), It.Is<string>(p => p == null)), Times.Once);
        }

        [TestMethod]
        public void Deserialize_OnCall2_AccessDataPackReader()
        {
            var p1 = new MemoryStream();
            var p2 = new byte[] { };

            var mock = TestHelper.IDataPackReaderHelper.CreateMock(MockBehavior.Loose);

            TransportSerializer.DataPackReader = mock.Object;
            TransportSerializer.Deserialize(p1, p2);

            mock.Verify(o => o.Read(It.Is<Stream>(p => ReferenceEquals(p, p1)), It.Is<byte[]>(p => ReferenceEquals(p, p2))), Times.Once);
        }

        [TestMethod]
        public void Deserialize_OnCall3_AccessDataPackReader()
        {
            var p1 = new MemoryStream();
            var p2 = TestHelper.NextString();

            var mock = TestHelper.IDataPackReaderHelper.CreateMock(MockBehavior.Loose);

            TransportSerializer.DataPackReader = mock.Object;
            TransportSerializer.Deserialize(p1, p2);

            mock.Verify(o => o.Read(It.Is<Stream>(p => ReferenceEquals(p, p1)), It.Is<string>(p => ReferenceEquals(p, p2))), Times.Once);
        }
    }
}