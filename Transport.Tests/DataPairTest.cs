using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf.Transport;

namespace Transport.Tests
{
    [TestClass]
    public class DataPairTest
    {
        [TestMethod]
        public void Constructor_WithEmptyConstructor_MustCreateObject()
        {
            new DataPair();
        }

        [TestMethod]
        public void Constructor_WithSomeName_MustCreateObject()
        {
            new DataPair(TestHelper.NextString());
        }

        [TestMethod]
        public void Constructor_WithNullName_MustCreateObject()
        {
            TestHelper.MustThrowArgumentNullException(() => new DataPair(null), "name");
        }

        [TestMethod]
        public void Constructor_WithEmptyString_MustCreateObject()
        {
            TestHelper.MustThrowArgumentOutOfRangeException(() => new DataPair(string.Empty), "name");
        }

        [TestMethod]
        public void Constructor_WithWhiteSpaces_MustCreateObject()
        {
            TestHelper.MustThrowArgumentOutOfRangeException(() => new DataPair(" "), "name");
        }

        [TestMethod]
        public void Constructor_WithSomeValue_MustCreateObject()
        {
            new DataPair(TestHelper.NextString(), TestHelper.NextString());
        }

        [TestMethod]
        public void Constructor_WithNullValue_MustCreateObject()
        {
            new DataPair(TestHelper.NextString(), null);
        }

        [TestMethod]
        public void Name_FromDefaultConstructor_Null()
        {
            var target = new DataPair();

            string expected = null;
            string actual = target.Name;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Value_FromDefaultConstructor_Null()
        {
            var target = new DataPair();

            string expected = null;
            string actual = target.Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Name_ConstructorInitializeNameProperty_ExpectedValue()
        {
            var target = new DataPair("12345");

            string expected = "12345";
            string actual = target.Name;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Value_ConstructorInitializeValueProperty_ExpectedValue()
        {
            var target = new DataPair(TestHelper.NextString(), "123456");

            string expected = "123456";
            string actual = target.Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Clone_OnCall_ReturnClone()
        {
            var target = new DataPair(TestHelper.NextString(), TestHelper.NextString());

            var actual = target.Clone();

            Assert.IsNotNull(actual);
            Assert.AreNotSame(target, actual);
            Assert.AreEqual(target.Name, actual.Name, "Name");
            Assert.AreEqual(target.Value, actual.Value, "Value");
        }
    }
}
