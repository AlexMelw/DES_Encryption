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
            for (int index = 0; index < paddedBitArray.Length - BlockSize + 1; index += BlockSize)
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
                ipDataVector.AddLast(currentDataBlock[currentIpPosition-1]);
            }

            Bit[] lhsPrev = ipDataVector.Take(BlockSize / 2).ToArray();
            Bit[] rhsPrev = ipDataVector.Skip(BlockSize / 2).ToArray();

            for (int i = 0; i < 16; i++)
            {
                Bit[] lhsN = new Bit[32];
                Array.Copy(rhsPrev, lhsN, rhsPrev.Length);

                Bit[] rhsN = new Bit[32];
                Array.Copy(lhsPrev, rhsN, lhsPrev.Length);

                XorRhsPrevWithNKey(rhsPrev, subKeys[i]);

                lhsPrev = rhsN;
                rhsPrev = rhsN;
            }
        }

        private Bit[] XorRhsPrevWithNKey(Bit[] rhsPrev, Bit[] subKey)
        {
            Bit[] expandedTo48BitsBlock = ExpandFrom32To48(rhsPrev);

            Bit[] xoredBits = new Bit[48];

            for (int i = 0; i < subKey.Length; i++)
            {
                xoredBits[i] = expandedTo48BitsBlock[i] ^ subKey[i];
            }

            return TransformXored48BitBlockTo32Bit(xoredBits);
        }

        private Bit[] TransformXored48BitBlockTo32Bit(Bit[] BitsBlock48)
        {
            LinkedList<Bit> bitsBlock32 = new LinkedList<Bit>();

            int segmentSize = 6;
            int penultBlock = BitsBlock48.Length - segmentSize;

            for (int i = 0; i < penultBlock; i += segmentSize)
            {
                Bit[] sixBits = BitsBlock48.Skip(i).Take(i + segmentSize).ToArray();
                Bit[] fourBits = UnboxingFourBits(sixBits);

                foreach (Bit bit in fourBits)
                {
                    bitsBlock32.AddLast(bit);
                }
            }

            return bitsBlock32.ToArray();
        }

        private Bit[] UnboxingFourBits(Bit[] sixBits)
        {
            Bit[] binRow = { sixBits.First(), sixBits.Last() };
            Bit[] binCol = { sixBits[1], sixBits[2], sixBits[3], sixBits[4] };

            int decRow = BinaryNumbersMapper.TwoBitsDecimalMapping[binRow];
            int decCol = BinaryNumbersMapper.FourBitsDecimalMapping[binCol];

            return null;
        }

        private Bit[] ExpandFrom32To48(Bit[] rhs)
        {
            LinkedList<Bit> expandedBlock = new LinkedList<Bit>();

            foreach (int currentPosition in EBitSelection.Table)
            {
                expandedBlock.AddLast(rhs[currentPosition-1]);
            }

            return expandedBlock.ToArray();
        }
    }
}