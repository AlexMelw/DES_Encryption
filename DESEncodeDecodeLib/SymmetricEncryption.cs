namespace DESEncodeDecodeLib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Security.Cryptography;
    using AlgorithmTables;
    using Utils;

    class SymmetricEncryption
    {
        protected static int BlockSize = 64;
        protected readonly byte[] data;
        protected readonly byte[] key;
        protected Bit[] keyOriginalBitArray;
        protected Bit[] paddedBitArray;
        private static CD[] _cdParts;
        private Bit[][] _subKeys;

        protected SymmetricEncryption(byte[] data, byte[] key)
        {
            this.data = data;
            this.key = key;
        }

        protected void GenerateSubKeys(Bit[] permutedKeyByPc1)
        {
            GenerateCDpartsarts(permutedKeyByPc1);
            Generate16SubKeys();
        }

        private static void GenerateCDpartsarts(Bit[] permutedKeyByPc1)
        {
            int count = permutedKeyByPc1.Length;

            var tempC0 = permutedKeyByPc1.Take(count / 2);
            var tempD0 = permutedKeyByPc1.Skip(count / 2);

            var Cprev = new Queue<Bit>(tempC0);
            var Dprev = new Queue<Bit>(tempD0);

            _cdParts = new CD[16];

            for (int i = 0; i < 16; i++)
            {
                var Cn = new Queue<Bit>(Cprev);
                var Dn = new Queue<Bit>(Dprev);

                int iteration = i + 1;
                int shiftNumber = ShiftsIterationsRelation.ShiftsNumberDictionary[iteration];
                for (int j = 0; j < shiftNumber; j++)
                {
                    Cn.Enqueue(Cn.Dequeue());
                    Dn.Enqueue(Dn.Dequeue());
                }

                _cdParts[i].C = Cn.ToArray();
                _cdParts[i].D = Dn.ToArray();
            }
        }

        private void Generate16SubKeys()
        {
            _subKeys = new Bit[16][];

            for (int i = 0; i < 16; i++)
            {
                LinkedList<Bit> nKey = new LinkedList<Bit>();

                Bit[] cdSource = _cdParts[i].C.Concat(_cdParts[i].D).ToArray();

                foreach (int currentPositionPC2 in PC2Table.Table)
                {
                    nKey.AddLast(cdSource[currentPositionPC2]);
                }

                _subKeys[i] = nKey.ToArray();
            }
        }

        protected Bit[] permuteKeyByPC1()
        {
            LinkedList<Bit> permutedKeyLinkedList = new LinkedList<Bit>();

            foreach (int currentPositionPC1 in PC1Table.Table)
            {
                permutedKeyLinkedList.AddLast(keyOriginalBitArray[currentPositionPC1]);
            }

            return permutedKeyLinkedList.ToArray();
        }


        class CD
        {
            public Bit[] C { get; set; }
            public Bit[] D { get; set; }
        }

        protected Bit[] ToPaddedBitArray(byte[] bytes)
        {
            LinkedList<Bit> bitsList = TransformToBitLinkedList(bytes);

            PadWithZeros(bitsList, out int paddingBits, out LinkedList<Bit> paddedBitList);

            AppendPaddingBitsNumber(paddingBits, paddedBitList);

            return paddedBitList.ToArray();
        }

        private void AppendPaddingBitsNumber(int paddingBits, LinkedList<Bit> paddedBitList)
        {
            byte[] intBytes = BitConverter.GetBytes(paddingBits);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }

            byte paddingBitsCount = intBytes.Last();
            Bit[] paddingBitsCountAsBitsArray = StripBits(paddingBitsCount);

            BinaryUtil.PrintByteAsBinaryString(paddingBitsCount);

            foreach (Bit bit in paddingBitsCountAsBitsArray)
            {
                paddedBitList.AddLast(bit);
            }

            BinaryUtil.PrintBitArrayAsBinaryString(paddingBitsCountAsBitsArray);
        }


        private static void PadWithZeros(LinkedList<Bit> bitsList, out int paddingBits,
            out LinkedList<Bit> paddedBitList)
        {
            int lastByteSize = 8;

            long originalLength = bitsList.LongCount() + lastByteSize;

            int toBePadded = (int)(originalLength % 64);

            paddingBits = toBePadded == 0
                ? lastByteSize
                : lastByteSize + toBePadded;
            paddedBitList = bitsList;
            for (int i = 0; i < toBePadded; i++)
            {
                paddedBitList.AddLast(0);
            }
        }


        private LinkedList<Bit> TransformToBitLinkedList(byte[] bytes)
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

        protected Bit[] TransformToBitArray(byte[] bytes)
        {
            return TransformToBitLinkedList(bytes).ToArray();
        }

        private Bit[] StripBits(byte octet)
        {
            Bit[] eightBits = new Bit[8];

            for (int i = 0; i < 8; i++)
            {
                eightBits[i] = (byte)(octet & (0b1000_0000 >> i));
            }

            return eightBits;
        }
    }
}