namespace DESEncodeDecodeLib
{
    using Decryption;
    using Encryption;
    using Interfaces;

    public static class CryptoFactory
    {
        public static IDesEncryptor CreateDesEncryptor(byte[] data, byte[] key)
        {
            return new DesEncryptor(data, key);
        }

        public static IDesDecryptor CreateDesDecryptor(byte[] data, byte[] key)
        {
            return new DesDecryptor(data, key);
        }
    }
}