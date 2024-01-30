using Xunit;
using Cardutil;
using Xunit.Sdk;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;

namespace Cardutil.UnitTests
{    
    public class BitArrayTests
    {
        [Fact]
        public void EmptyByteArrayTest()
        {
            byte[] inputData = [];
            bool[] result = BitArray.getBits(inputData);
            bool[] expectedResult = {false};  // just index 0  
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void OneByteTo8BitsTest()
        {
            byte[] inputData = [0x01];
            bool[] result = BitArray.getBits(inputData);
            bool[] expectedResult = {false, false, false, false, false, false, false, false, true};
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TwoBytesTo16BitsTest()
        {
            byte[] inputData = [0x01, 0x02];
            bool[] result = BitArray.getBits(inputData);
            bool[] expectedResult = {
                false,
                false, false, false, false, false, false, false, true,
                false, false, false, false, false, false, true, false
            };

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KnownBitmapCorrect()
        {
            byte[] bitmapBytes = [0xF0,0x10,0x05,0x42,0x84,0x61,0x80,0x02,0x02,0x00,0x00,0x04,0x00,0x00,0x00,0x00];
            bool[] result = Cardutil.BitArray.getBits(bitmapBytes);
            int[] trueBits = [1,2,3,4,12,22,24,26,31,33,38,42,43,48,49,63,71,94];
            foreach (var bitIndex in trueBits) {
                Assert.True(result[bitIndex], $"Bit {bitIndex} is true");
            }
        }

    }
}