namespace DESEncodeDecodeLib.Decryption
{
    using System.Linq;
    using Base;
    using Enums;
    using Interfaces;
    using Utils;

    class DesDecryptor : DesEncryptionBase, IDecryptor
    {
        #region CONSTRUCTORS

        public DesDecryptor(byte[] data, byte[] key)
            : base(data, key, OperationMode.Decryption) { }

        #endregion

        public byte[] DecryptData()
        {
            InitializeDesEngine(out Bit[][] subKeys, out Bit[] encryptedBitArray);

            // Phase III (Differs depending on selected mode)
            Bit[] decryptedDataBlocks = DecryptDataBlocks(subKeys, encryptedBitArray);

            // Bits -> bytes
            byte[] deccryptedBytes = BinaryUtil.TransformBitsToBytes(decryptedDataBlocks);

            return deccryptedBytes;
        }

        protected override Bit[] ApplyDesTransformationOn64BitBlock(Bit[][] subKeys, Bit[] data, int @from, int count)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(count).ToArray();

            Bit[] ipDataVector = ComputeIpDataVector(currentDataBlock);

            Bit[] r0L0 = ComputeL0R0(subKeys, ipDataVector);

            Bit[] finalIpPermutationBitArray = ComputeFinalIpBitsArray(r0L0);

            return finalIpPermutationBitArray;
        }

        private Bit[] DecryptDataBlocks(Bit[][] subKeys, Bit[] encryptedBitArray)
        {
            Bit[] plainBitArray = TransformDataApplyingSubkeys(subKeys, encryptedBitArray);

            Bit[] cutPlainBitArray = RemovePaddingBitsFromPlainBitArray(plainBitArray);

            //Bit[] cutPlainBitArray = plainBitArray;

            return cutPlainBitArray;
        }

        private static Bit[] RemovePaddingBitsFromPlainBitArray(Bit[] plainBitArray)
        {
            Bit[] lastByte = plainBitArray.Skip(plainBitArray.Length - 8).Take(8).ToArray();

            byte paddingBitsCount = BinaryUtil.TransformBitsToBytes(lastByte).Single();

            Bit[] cutPlainBitArray = plainBitArray.Take(plainBitArray.Length - paddingBitsCount).ToArray();
            return cutPlainBitArray;
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