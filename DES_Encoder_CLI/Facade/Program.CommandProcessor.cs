namespace DEScli
{
    using System;
    using System.IO;
    using DESEncodeDecodeLib;
    using DESEncodeDecodeLib.Interfaces;
    using DESEncodeDecodeLib.Utils;

    static partial class Program
    {
        private static void ProcessDecryptCommand(DecryptVerbOptions options)
        {
            byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            byte[] keyByteArray = File.ReadAllBytes(options.KeyPath);

            IDecryptor desDecryptor = CryptoFactory.CreateDecryptor(inputByteArray, keyByteArray);
            byte[] decryptedData = desDecryptor.DecryptData();

            GenerateOutputFileNameIfNotSet(options);
            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(decryptedData, 0, decryptedData.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }

        private static void ProcessEncryptCommand(EncryptVerbOptions options)
        {
            byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            byte[] keyByteArray = File.ReadAllBytes(options.KeyPath);

            IEncryptor desEncryptor = CryptoFactory.CreateEncryptor(inputByteArray, keyByteArray);
            byte[] encryptedData = desEncryptor.EncryptData();

            GenerateOutputFileNameIfNotSet(options);
            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(encryptedData, 0, encryptedData.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }

        private static void ProcessGenerateSKCommand(GenerateStrongKeyVerbOptions options)
        {
            byte[] secureKey = RandomNumbersUtil.GenerateRandomNumbers(8);

            GenerateOutputFileNameIfNotSet(options);

            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(secureKey, 0, secureKey.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }
    }
}