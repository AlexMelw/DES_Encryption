namespace DESEncodeDecodeLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgorithmTables;
    using EasySharp.NHelpers.CustomExMethods;
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

            Bit[] encryptedDataBlocks = EncryptDataBlocks();
            byte[] encryptedBytes = TransformBitsToBytes(encryptedDataBlocks);

            return encryptedBytes;
        }

        private byte[] TransformBitsToBytes(Bit[] bits)
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

        private Bit[] EncryptDataBlocks()
        {
            for (int index = 0; index <= paddedBitArray.Length - BlockSize; index += BlockSize)
            {
                Encrypt64BitBlock(
                    data: paddedBitArray,
                    from: index,
                    count: BlockSize);
            }

            return paddedBitArray;
        }

        private void Encrypt64BitBlock(Bit[] data, int from, int count)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(count).ToArray();

            Bit[] ipDataVector = ComputeIpDataVector(currentDataBlock);

            Bit[] r16L16 = ComputeR16L16(ipDataVector);

            Bit[] finalIpPermutationBitArray = ComputeFinalIpBitsArray(r16L16);

            FillSourceDataBlockWithDesEncryptedData(data, finalIpPermutationBitArray, from, count);
        }

        private void FillSourceDataBlockWithDesEncryptedData(
            Bit[] sourceDataBits,
            Bit[] encryptedBlock,
            int from, int count)
        {
            int index = 0;
            for (int cursor = from; cursor < from + count; cursor++)
            {
                sourceDataBits[cursor] = encryptedBlock[index];
                index++;
            }
        }

        private Bit[] ComputeFinalIpBitsArray(Bit[] r16L16)
        {
            LinkedList<Bit> finalIpPermutationLinkedList = new LinkedList<Bit>();

            foreach (int currentPosition in FinalPermutationIP.Table)
            {
                finalIpPermutationLinkedList.AddLast(r16L16[currentPosition - 1]);
            }

            return finalIpPermutationLinkedList.ToArray();
        }

        private static Bit[] ComputeIpDataVector(Bit[] currentDataBlock)
        {
            LinkedList<Bit> ipDataVector = new LinkedList<Bit>();

            foreach (int currentIpPosition in InitialPermutation.Table)
            {
                ipDataVector.AddLast(currentDataBlock[currentIpPosition - 1]);
            }

            return ipDataVector.ToArray();
        }

        private Bit[] ComputeR16L16(Bit[] ipDataVector)
        {
            Bit[] lhsPrev = ipDataVector.Take(BlockSize / 2).ToArray();
            Bit[] rhsPrev = ipDataVector.Skip(BlockSize / 2).ToArray();

            Bit[] lhsN = null;
            Bit[] rhsN = null;
            for (int i = 0; i < 16; i++)
            {
                lhsN = CloneBitArray(rhsPrev);
                rhsN = XorArrays(
                    leftOperand: CloneBitArray(lhsPrev),
                    rightOperand: XorRhsPrevWithNKey(rhsPrev, subKeys[i]));

                lhsPrev = CloneBitArray(lhsN);
                rhsPrev = CloneBitArray(rhsN);
            }

            // rhsN == R16 and lhsN == L16
            Bit[] r16L16 = rhsN.Concat(lhsN).ToArray();

            return r16L16;
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