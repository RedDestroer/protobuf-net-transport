using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }
    }
}