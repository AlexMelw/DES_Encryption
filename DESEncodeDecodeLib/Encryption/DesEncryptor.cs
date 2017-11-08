namespace DESEncodeDecodeLib.Encryption
{
    using System.Linq;
    using Base;
    using Enums;
    using Interfaces;
    using Utils;

    class DesEncryptor : DesEncryptionBase, IEncryptor
    {
        #region CONSTRUCTORS

        public DesEncryptor(byte[] data, byte[] key) :
            base(data, key, OperationMode.Encryption) { }

        #endregion

        public byte[] EncryptData()
        {
            InitializeDesEngine(out Bit[][] subKeys, out Bit[] paddedBitArray);

            // Phase III (Differs depending on selected mode)
            Bit[] encryptedDataBlocks = EncryptDataBlocks(subKeys, paddedBitArray);

            // Bits -> bytes
            byte[] encryptedBytes = BinaryUtil.TransformBitsToBytes(encryptedDataBlocks);

            return encryptedBytes;
        }

        protected override Bit[] ApplyDesTransformationOn64BitBlock(Bit[][] subKeys, Bit[] data, int @from, int count)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(count).ToArray();

            Bit[] ipDataVector = ComputeIpDataVector(currentDataBlock);

            Bit[] r16L16 = ComputeR16L16(subKeys, ipDataVector);

            Bit[] finalIpPermutationBitArray = ComputeFinalIpBitsArray(r16L16);

            return finalIpPermutationBitArray;

            //FillSourceDataBlockWithDesEncryptedData(data, finalIpPermutationBitArray, from, count);
        }

        private Bit[] EncryptDataBlocks(Bit[][] subKeys, Bit[] plainBitArray)
        {
            Bit[] encryptedBitArray = TransformDataApplyingSubkeys(subKeys, plainBitArray);

            return encryptedBitArray;
        }

        //private void FillSourceDataBlockWithDesEncryptedData(
        //    Bit[] sourceDataBits,
        //    Bit[] encryptedBlock,
        //    int from, int count)
        //{
        //LinkedList<Bit> desEncryptedBits = new LinkedList<Bit>();

        //int index = 0;
        //for (int cursor = from; cursor < from + count; cursor++)
        //{
        //    sourceDataBits[cursor] = encryptedBlock[index++];
        //}

        //}

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