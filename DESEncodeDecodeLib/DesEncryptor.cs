namespace DESEncodeDecodeLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgorithmTables;
    using Interfaces;
    using Utils;

    class DesEncryptor : SymmetricEncryption, IDesEncryptor
    {
        #region CONSTRUCTORS

        public DesEncryptor(byte[] data, byte[] key)
            : base(data, key) { }

        #endregion

        public byte[] EncryptData()
        {
            keyOriginalBitArray = TransformToBitArray(key);
            paddedBitArray = ToPaddedBitArray(data);

            Bit[] permutedKeyByPc1 = permuteKeyByPC1();
            GenerateSubKeys(permutedKeyByPc1);

            EncryptDataBlocks();

            return null;
        }

        private void EncryptDataBlocks()
        {
            for (int index = 0; index <= paddedBitArray.Length - BlockSize; index += BlockSize)
            {
                Encrypt64BitBlock(paddedBitArray,
                    from: index,
                    to: index + BlockSize);
            }
        }

        private void Encrypt64BitBlock(Bit[] data, int from, int to)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(to).ToArray();

            LinkedList<Bit> ipDataVector = new LinkedList<Bit>();

            foreach (int currentIpPosition in InitialPermutation.Table)
            {
                ipDataVector.AddLast(currentDataBlock[currentIpPosition - 1]);
            }

            Bit[] lhsPrev = ipDataVector.Take(BlockSize / 2).ToArray();
            Bit[] rhsPrev = ipDataVector.Skip(BlockSize / 2).ToArray();

            for (int i = 0; i < 16; i++)
            {
                Bit[] lhsN = CloneBitArray(rhsPrev);
                Bit[] rhsN = XorArrays(
                    leftOperand: CloneBitArray(lhsPrev),
                    rightOperand: XorRhsPrevWithNKey(rhsPrev, subKeys[i]));

                lhsPrev = CloneBitArray(lhsN);
                rhsPrev = CloneBitArray(rhsN);
            }

            throw new NotImplementedException();
        }

        private Bit[] CloneBitArray(Bit[] source)
        {
            Bit[] destination = new Bit[source.Length];
            Array.Copy(source, destination, source.Length);
            return destination;
        }

        private Bit[] XorRhsPrevWithNKey(Bit[] rhsPrev, Bit[] subKey)
        {
            Bit[] expandedTo48BitsBlock = ExpandFrom32To48(rhsPrev);

            Bit[] xored48BitBlock = XorArrays(subKey, expandedTo48BitsBlock);

            Bit[] xored32BitBlock = TransformXored48BitBlockTo32Bit(xored48BitBlock);

            Bit[] penultBitsPermutationArray = ExecutePenultPermutation(xored32BitBlock);

            throw new NotImplementedException();
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

        private Bit[] XorArrays(Bit[] leftOperand, Bit[] rightOperand)
        {
            Bit[] xoredBits = new Bit[leftOperand.Length];

            for (int i = 0; i < leftOperand.Length; i++)
            {
                xoredBits[i] = leftOperand[i] ^ rightOperand[i];
            }

            return xoredBits;
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
    }
}