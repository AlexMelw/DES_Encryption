namespace DESEncodeDecodeLib
{
    using System.Collections.Generic;
    using System.Linq;
    using AlgorithmTables;
    using Interfaces;

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
            for (int index = 0; index < paddedBitArray.Length - BlockSize; index += BlockSize)
            {
                Encrypt64BitBlock(paddedBitArray,
                    from: index,
                    to: index + BlockSize);
            }
        }

        private void Encrypt64BitBlock(Bit[] data, int from, int to)
        {
            Bit[] currentDataBlock = data.Skip(from).Take(to).ToArray();
            Bit[] lhs = currentDataBlock.Take(BlockSize / 2).ToArray();
            Bit[] rhs = currentDataBlock.Skip(BlockSize / 2).ToArray();

            LinkedList<Bit> ipDataVector = new LinkedList<Bit>(currentDataBlock);

            foreach (int currentIpPosition in InitialPermutation.Table)
            {
                ipDataVector.AddLast(currentDataBlock[currentIpPosition]);
            }
        }
    }
}