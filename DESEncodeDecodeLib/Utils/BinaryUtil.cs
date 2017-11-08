namespace DESEncodeDecodeLib.Utils
{
    using System;
    using System.Collections.Generic;
    using EasySharp.NHelpers.CustomExMethods;

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

        public static Bit[] XorArrays(Bit[] leftOperand, Bit[] rightOperand)
        {
            Bit[] xoredBits = new Bit[leftOperand.Length];

            for (int i = 0; i < leftOperand.Length; i++)
            {
                xoredBits[i] = leftOperand[i] ^ rightOperand[i];
            }

            return xoredBits;
        }

        public static byte[] TransformBitsToBytes(Bit[] bits)
        {
            int bytesCount = bits.Length / 8;
            byte[] octets = new byte[bytesCount];
            octets.SelfSetToDefaults();

            for (int byteIdx = 0; byteIdx < bytesCount; byteIdx++)
            {
                int bitOffset = byteIdx * 8;

                for (int bitIdx = bitOffset; bitIdx < bitOffset + 8; bitIdx++)
                {
                    octets[byteIdx] = (byte) (octets[byteIdx] | (bits[bitIdx]
                                                  ? 0b1000_0000 >> (bitIdx % 8)
                                                  : 0x0));
                }
            }

            return octets;
        }

        public static Bit[] StripBits(byte octet)
        {
            Bit[] eightBits = new Bit[8];

            for (int i = 0; i < 8; i++)
            {
                eightBits[i] = (byte) (octet & (0b1000_0000 >> i));
            }

            return eightBits;
        }

        public static LinkedList<Bit> TransformToBitLinkedList(byte[] bytes)
        {
            LinkedList<Bit> bitsList = new LinkedList<Bit>();

            foreach (byte octet in bytes)
            {
                Bit[] eightBits = StripBits(octet);

                foreach (Bit bit in eightBits)
                {
                    bitsList.AddLast(bit);
                }
            }

            return bitsList;
        }
    }
}