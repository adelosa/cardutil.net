namespace Cardutil
{
    public class BitArray
    {
        public static bool[] getBits(byte[] inputData)
        {
            bool[] output = [];
            foreach (byte data in inputData) {
                var dataBool = BitArray.ConvertByteToBoolArray(data);
                output = [.. output, .. dataBool];
            }
            
            // push index + 1 so can reference bit using index
            bool[] finalOutput = new bool[output.Length + 1];
            finalOutput[0] = false;  // set the zero index
            Array.Copy(output, 0, finalOutput, 1, output.Length);
            
            return finalOutput;
        }

        // https://stackoverflow.com/questions/24322417/how-to-convert-bool-array-in-one-byte-and-later-convert-back-in-bool-array
        private static bool[] ConvertByteToBoolArray(byte b) {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++) {
                result[i] = (b & (1 << i)) != 0;
            }

            // reverse the array
            Array.Reverse(result);

            return result;
        }
    }
}
