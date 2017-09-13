namespace DESEncodeDecodeLib.Decryption
{
    using System;
    using System.Linq;
    using Base;
    using Interfaces;

    class DesDecryptor : DesEncryptionBase, IDesDecryptor
    {
        private readonly byte[] _data;
        private readonly byte[] _key;

        #region CONSTRUCTORS

        public DesDecryptor(byte[] data, byte[] key)
            : base(data, key) { }

        #endregion

        public byte[] DecryptData()
        {
            throw new System.NotImplementedException();
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

        protected override void ApplyDesOn64BitBlock(Bit[][] subKeys, Bit[] data, int @from, int count)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(count).ToArray();

            Bit[] ipDataVector = ComputeIpDataVector(currentDataBlock);

            Bit[] r16L16 = ComputeL0R0(subKeys, ipDataVector);

            Bit[] finalIpPermutationBitArray = ComputeFinalIpBitsArray(r16L16);

            throw new NotImplementedException();
            //FillSourceDataBlockWithDesEncryptedData(data, finalIpPermutationBitArray, from, count);
        }
    }
}