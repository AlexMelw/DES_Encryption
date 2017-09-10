namespace DES_Encoder_CLI
{
    using System;
    using DESEncodeDecodeLib;
    using DESEncodeDecodeLib.Interfaces;
    using DESEncodeDecodeLib.Utils;

    static class Program
    {
        static void Main(string[] args)
        {
            byte[] data = RandomNumbersUtil.GenerateRandomNumbers(7);
            byte[] key = RandomNumbersUtil.GenerateRandomNumbers(8);

            IDesEncryptor desEncryptor = CryptoFactory.CreateDesEncryptor(data, key);
            //IDesDecryptor desDecryptor = CryptoFactory.CreateDesDecryptor(data, key);

            byte[] encryptedData = desEncryptor.EncryptData();

            //Bit bit1 = 0;
            //Bit bit2 = 0;
            //Console.Out.WriteLine(bit1 ? $"Bit is 1 [{bit1}]" : $"Bit is 0 [{bit1}]");
            //Console.Out.WriteLine((byte)bit1);
            //Console.Out.WriteLine((byte)bit2);
            //Console.Out.WriteLine("bit1 == bit2 = {0}", bit1.Equals(bit2));
        }
    }
}