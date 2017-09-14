namespace DESEncodeDecodeLib
{
    using Decryption;
    using Encryption;
    using Interfaces;

    public static class CryptoFactory
    {
        public static IEncryptor CreateEncryptor(byte[] data, byte[] key)
        {
            return new DesEncryptor(data, key);
        }

        public static IDecryptor CreateDecryptor(byte[] data, byte[] key)
        {
            return new DesDecryptor(data, key);
        }
    }
}