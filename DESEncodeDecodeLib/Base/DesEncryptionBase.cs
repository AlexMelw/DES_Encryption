namespace DESEncodeDecodeLib.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgorithmTables;
    using EasySharp.NHelpers.CustomExMethods;
    using Utils;

    abstract class DesEncryptionBase
    {
        protected const int BlockSize = 64;
        private readonly byte[] Data;
        private readonly byte[] Key;
        private Bit[] Key64BitArray;

        #region CONSTRUCTORS

        protected DesEncryptionBase(byte[] data, byte[] key)
        {
            Data = data;
            Key = key;
        }

        #endregion

        protected abstract void ApplyDesOn64BitBlock(Bit[][] subKeys, Bit[] data, int @from, int count);

        protected void InitializeDesEngine(out Bit[] paddedBitArray, out Bit[][] subKeys)
        {
            // bytes -> Bits
            Key64BitArray = TransformToBitArray(Key);
            paddedBitArray = ToPaddedBitArray(Data);

            // Phase I
            Bit[] permutedKeyByPc1 = PermuteKeyByPc1();

            // Phase II
            subKeys = GenerateSubKeys(permutedKeyByPc1);
        }

        protected void TransformDataApplyingSubkeys(Bit[][] subKeys, ref Bit[] paddedBitArray)
        {
            for (int index = 0; index <= paddedBitArray.Length - BlockSize; index += BlockSize)
            {
                ApplyDesOn64BitBlock(
                    subKeys,
                    paddedBitArray,
                    index,
                    BlockSize);
            }
        }


        protected Bit[] ComputeFinalIpBitsArray(Bit[] r16L16)
        {
            LinkedList<Bit> finalIpPermutationLinkedList = new LinkedList<Bit>();

            foreach (int currentPosition in FinalPermutationIP.Table)
            {
                finalIpPermutationLinkedList.AddLast(r16L16[currentPosition - 1]);
            }

            return finalIpPermutationLinkedList.ToArray();
        }

        protected static Bit[] ComputeIpDataVector(Bit[] currentDataBlock)
        {
            LinkedList<Bit> ipDataVector = new LinkedList<Bit>();

            foreach (int currentIpPosition in InitialPermutation.Table)
            {
                ipDataVector.AddLast(currentDataBlock[currentIpPosition - 1]);
            }

            return ipDataVector.ToArray();
        }

        protected void ApplyConsecutively16SubKeys(Bit[] ipDataVector, Bit[][] subKeys, out Bit[] lhsN, out Bit[] rhsN)
        {
            Bit[] lhsPrev = ipDataVector.Take(BlockSize / 2).ToArray();
            Bit[] rhsPrev = ipDataVector.Skip(BlockSize / 2).ToArray();

            lhsN = null;
            rhsN = null;
            for (int i = 0; i < 16; i++)
            {
                lhsN = rhsPrev.CloneArray();
                rhsN = BinaryUtil.XorArrays(
                    lhsPrev.CloneArray(),
                    XorRhsPrevWithNKey(rhsPrev, subKeys[i]));

                lhsPrev = lhsN.CloneArray();
                rhsPrev = rhsN.CloneArray();
            }
        }

        protected Bit[][] GenerateSubKeys(Bit[] permutedKeyByPc1)
        {
            CD[] cdParts = GenerateCdParts(permutedKeyByPc1);
            Bit[][] subKeys = Generate16SubKeys(cdParts);

            return subKeys;
        }

        protected Bit[] PermuteKeyByPc1()
        {
            LinkedList<Bit> permutedKeyLinkedList = new LinkedList<Bit>();

            foreach (int currentPositionPC1 in PC1Table.Table)
            {
                permutedKeyLinkedList.AddLast(Key64BitArray[currentPositionPC1 - 1]);
            }

            return permutedKeyLinkedList.ToArray();
        }

        protected Bit[] ToPaddedBitArray(byte[] bytes)
        {
            LinkedList<Bit> bitsList = BinaryUtil.TransformToBitLinkedList(bytes);

            PadWithZeros(bitsList, out int paddingBits, out LinkedList<Bit> paddedBitList);

            AppendPaddingBitsNumber(paddingBits, paddedBitList);

            return paddedBitList.ToArray();
        }

        protected Bit[] TransformToBitArray(byte[] bytes)
        {
            return BinaryUtil.TransformToBitLinkedList(bytes).ToArray();
        }


        private Bit[] XorRhsPrevWithNKey(Bit[] rhsPrev, Bit[] subKey)
        {
            Bit[] expandedTo48BitsBlock = ExpandFrom32To48(rhsPrev);

            Bit[] xored48BitBlock = BinaryUtil.XorArrays(subKey, expandedTo48BitsBlock);

            Bit[] xored32BitBlock = TransformXored48BitBlockTo32Bit(xored48BitBlock);

            Bit[] penultBitsPermutationArray = ExecutePenultPermutation(xored32BitBlock);

            return penultBitsPermutationArray.ToArray();
        }

        private Bit[] ExecutePenultPermutation(Bit[] xored32BitBlock)
        {
            LinkedList<Bit> penultPermutedBitsLinkedList = new LinkedList<Bit>(xored32BitBlock);

            foreach (int currentPosition in PenultPermutation.Table)
            {
                penultPermutedBitsLinkedList.AddLast(xored32BitBlock[currentPosition - 1]);
            }

            return penultPermutedBitsLinkedList.ToArray();
        }


        private Bit[] TransformXored48BitBlockTo32Bit(Bit[] BitsBlock48)
        {
            LinkedList<Bit> bitsBlock32 = new LinkedList<Bit>();

            int segmentSize = 6;
            int penultBlock = BitsBlock48.Length - segmentSize;

            for (int i = 0; i <= penultBlock; i += segmentSize)
            {
                Bit[] sixBits = BitsBlock48.Skip(i).Take(i + segmentSize).ToArray();
                int boxIndex = i / segmentSize;
                Bit[] fourBits = UnboxingFourBits(sixBits, boxIndex);

                foreach (Bit bit in fourBits)
                {
                    bitsBlock32.AddLast(bit);
                }
            }

            return bitsBlock32.ToArray();
        }


        private Bit[] UnboxingFourBits(Bit[] sixBits, int boxIndex)
        {
            Bit[] binRow = { sixBits.First(), sixBits.Last() };
            Bit[] binCol = { sixBits[1], sixBits[2], sixBits[3], sixBits[4] };

            int decRow = BinaryNumbersMapper.TwoBitsDecimalMapping[binRow];
            int decCol = BinaryNumbersMapper.FourBitsDecimalMapping[binCol];

            SBox sBox = SBoxMapper.S[boxIndex];
            byte decSBoxEquivalent = sBox[decRow][decCol];
            Bit[] strippedBits = StripFourBits(decSBoxEquivalent);
            return strippedBits;
        }


        private Bit[] ExpandFrom32To48(Bit[] rhs)
        {
            LinkedList<Bit> expandedBlock = new LinkedList<Bit>();

            foreach (int currentPosition in EBitSelection.Table)
            {
                expandedBlock.AddLast(rhs[currentPosition - 1]);
            }

            return expandedBlock.ToArray();
        }


        private Bit[] StripFourBits(byte octet)
        {
            Bit[] fourBits = new Bit[4];

            for (int i = 4; i < 8; i++)
            {
                int bitIndex = i - 4;
                fourBits[bitIndex] = (byte) (octet & (0b1000_0000 >> i));
            }

            return fourBits;
        }

        private CD[] GenerateCdParts(Bit[] permutedKeyByPc1)
        {
            int count = permutedKeyByPc1.Length;

            var tempC0 = permutedKeyByPc1.Take(count / 2);
            var tempD0 = permutedKeyByPc1.Skip(count / 2);

            var Cprev = new Queue<Bit>(tempC0);
            var Dprev = new Queue<Bit>(tempD0);

            CD[] cdParts = new CD[16];
            for (var index = 0; index < cdParts.Length; index++)
            {
                CD cd = new CD
                {
                    C = new Bit[28].SetToDefaults(),
                    D = new Bit[28].SetToDefaults()
                };
                cdParts[index] = cd;
            }

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

                cdParts[i].C = Cn.ToArray();
                cdParts[i].D = Dn.ToArray();

                Cprev = new Queue<Bit>(Cn);
                Dprev = new Queue<Bit>(Dn);
            }

            return cdParts;
        }

        private Bit[][] Generate16SubKeys(CD[] cdParts)
        {
            Bit[][] subKeys = new Bit[16][];

            for (int i = 0; i < 16; i++)
            {
                LinkedList<Bit> nKey = new LinkedList<Bit>();

                Bit[] cdSource = cdParts[i].C.Concat(cdParts[i].D).ToArray();

                foreach (int currentPositionPC2 in PC2Table.Table)
                {
                    nKey.AddLast(cdSource[currentPositionPC2 - 1]);
                }

                subKeys[i] = nKey.ToArray();
            }

            return subKeys;
        }

        private void AppendPaddingBitsNumber(int paddingBits, LinkedList<Bit> paddedBitList)
        {
            byte[] intBytes = BitConverter.GetBytes(paddingBits);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }

            byte paddingBitsCount = intBytes.Last();
            Bit[] paddingBitsCountAsBitsArray = BinaryUtil.StripBits(paddingBitsCount);

            BinaryUtil.PrintByteAsBinaryString(paddingBitsCount);

            foreach (Bit bit in paddingBitsCountAsBitsArray)
            {
                paddedBitList.AddLast(bit);
            }

            BinaryUtil.PrintBitArrayAsBinaryString(paddingBitsCountAsBitsArray);
        }


        private void PadWithZeros(LinkedList<Bit> bitsList,
            out int paddingBits, out LinkedList<Bit> paddedBitList)
        {
            int lastByteSize = 8;

            long originalLength = bitsList.LongCount() + lastByteSize;

            int toBePadded = (int) (originalLength % 64);

            paddingBits = toBePadded == 0
                ? lastByteSize
                : lastByteSize + toBePadded;
            paddedBitList = bitsList;

            for (int i = 0; i < toBePadded; i++)
            {
                paddedBitList.AddLast(0);
            }
        }


        private class CD
        {
            public Bit[] C { get; set; }
            public Bit[] D { get; set; }
        }
    }
}