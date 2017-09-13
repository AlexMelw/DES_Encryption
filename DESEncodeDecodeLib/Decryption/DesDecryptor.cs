namespace DESEncodeDecodeLib.Decryption
{
    using System;
    using System.Linq;
    using Base;
    using Interfaces;
    using Utils;

    class DesDecryptor : DesEncryptionBase, IDesDecryptor
    {
        #region CONSTRUCTORS

        public DesDecryptor(byte[] data, byte[] key)
            : base(data, key) { }

        #endregion

        public byte[] DecryptData()
        {
            InitializeDesEngine(out Bit[] encryptedBitArray, out Bit[][] subKeys);

            // Phase III (Differs depending on selected mode)
            Bit[] decryptedDataBlocks = DecryptDataBlocks(subKeys, encryptedBitArray);

            // Bits -> bytes
            byte[] deccryptedBytes = BinaryUtil.TransformBitsToBytes(decryptedDataBlocks);

            return deccryptedBytes;
        }

        protected override void ApplyDesOn64BitBlock(Bit[][] subKeys, Bit[] data, int @from, int count)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(count).ToArray();

            Bit[] ipDataVector = ComputeIpDataVector(currentDataBlock);

            Bit[] r0L0 = ComputeL0R0(subKeys, ipDataVector);

            Bit[] finalIpPermutationBitArray = ComputeFinalIpBitsArray(r0L0);

            throw new NotImplementedException();
            //FillSourceDataBlockWithDesEncryptedData(data, finalIpPermutationBitArray, from, count);
        }

        private Bit[] DecryptDataBlocks(Bit[][] subKeys, Bit[] encryptedBitArray)
        {
            TransformDataApplyingSubkeys(subKeys, ref encryptedBitArray);

            return encryptedBitArray;
        }

        private Bit[] ComputeL0R0(Bit[][] subKeys, Bit[] ipDataVector)
        {
            ApplyConsecutively16SubKeys(
                ipDataVector,
                subKeys.Reverse().ToArray(), // Apply subkeys in reversed order for decrypting sake
                out Bit[] lhs0,
                out Bit[] rhs0);

            // rhsN == R16 and lhsN == L16
            Bit[] r0L0 = rhs0.Concat(lhs0).ToArray();

            return r0L0;
        }
    }
}