namespace DESEncodeDecodeLib.Utils
{
    using System;

    public static class BinaryUtil
    {
        public static void PrintBitArrayAsBinaryString(Bit[] paddingBitsCountAsBitsArray)
        {
            foreach (Bit bit in paddingBitsCountAsBitsArray)
            {
                Console.Out.Write(bit ? 1 : 0);
            }
        }

        public static void PrintByteAsBinaryString(byte paddingBitsCount)
        {
            Console.Out.WriteLine(
                Convert.ToString(paddingBitsCount, 2)
                .PadLeft(8, '0'));
        }
    }
}