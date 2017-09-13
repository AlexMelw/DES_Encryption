namespace DESEncodeDecodeLib.Encryption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgorithmTables;
    using Base;
    using EasySharp.NHelpers.CustomExMethods;
    using Interfaces;
    using Utils;

    class DesEncryptor : DesEncryptionBase, IDesEncryptor
    {
        #region CONSTRUCTORS

        public DesEncryptor(byte[] data, byte[] key)
            : base(data, key) { }

        #endregion

        public byte[] EncryptData()
        {
            // bytes -> Bits
            keyOriginalBitArray = TransformToBitArray(key);
            Bit[] paddedBitArray = ToPaddedBitArray(data);

            // Phase I
            Bit[] permutedKeyByPc1 = permuteKeyByPC1();

            // Phase II
            Bit[][] subKeys = GenerateSubKeys(permutedKeyByPc1);

            // Phase III (Differs depending on selected mode)
            Bit[] encryptedDataBlocks = EncryptDataBlocks(subKeys, paddedBitArray);

            // Bits -> bytes
            byte[] encryptedBytes = TransformBitsToBytes(encryptedDataBlocks);

            return encryptedBytes;
        }


        private Bit[] EncryptDataBlocks(Bit[][] subKeys, Bit[] paddedBitArray)
        {
            TransformDataApplyingSubkeys(subKeys, paddedBitArray);

            return paddedBitArray;
        }


        protected override void ApplyDesOn64BitBlock(Bit[][] subKeys, Bit[] data, int @from, int count)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(count).ToArray();

            Bit[] ipDataVector = ComputeIpDataVector(currentDataBlock);

            Bit[] r16L16 = ComputeR16L16(subKeys, ipDataVector);

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


        private Bit[] ComputeR16L16(Bit[][] subKeys, Bit[] ipDataVector)
        {
            ApplyConsecutively16SubKeys(
                ipDataVector,
                subKeys,
                out Bit[] lhsN,
                out Bit[] rhsN);

            // rhsN == R16 and lhsN == L16
            Bit[] r16L16 = rhsN.Concat(lhsN).ToArray();

            return r16L16;
        }



    }
}