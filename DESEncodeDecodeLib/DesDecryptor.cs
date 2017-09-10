namespace DESEncodeDecodeLib
{
    using Interfaces;

    class DesDecryptor : SymmetricEncryption, IDesDecryptor
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
    }
}