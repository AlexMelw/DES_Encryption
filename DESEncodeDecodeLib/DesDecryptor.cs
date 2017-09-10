namespace DESEncodeDecodeLib
{
    using Interfaces;

    class DesDecryptor : SymmetricEncryption, IDesDecryptor
    {
        private readonly byte[] _data;
        private readonly byte[] _key;

        #region CONSTRUCTORS

        public DesDecryptor(byte[] data, byte[] key)
        {
            _data = data;
            _key = key;
        }

        #endregion

        public byte[] DecryptData()
        {
            throw new System.NotImplementedException();
        }
    }
}