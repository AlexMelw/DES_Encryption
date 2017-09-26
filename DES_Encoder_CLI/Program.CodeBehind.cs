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
            byte[] secureKey = RandomNumbersUtil.GenerateRandomNumbers(byteLength: 8);

            GenerateOutputFileNameIfNotSet(options);

            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(secureKey, 0, secureKey.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }

        private static void GenerateOutputFileNameIfNotSet(IOutputableOption options)
        {
            if (string.IsNullOrWhiteSpace(options.OutputFilePath))
            {
                string fileExtension = CreateFileExtension(options);
                string filePrefixName = CreateFilePrefixName(options, ref fileExtension);

                DateTime now = DateTime.Now;
                options.OutputFilePath =
                    $"{filePrefixName}_" +
                    $"{now.Year}-{now.Month}-{now.Day}_" +
                    $"{now.Hour}{now.Minute}{now.Second}{now.Millisecond}" +
                    $"{fileExtension}";
            }
        }

        private static string CreateFileExtension(IOutputableOption options)
        {
            string fileExtension = Path.HasExtension(options.OutputFilePath)
                ? $".{Path.GetExtension(options.OutputFilePath)}"
                : string.Empty;
            return fileExtension;
        }

        private static string CreateFilePrefixName(IOutputableOption options, ref string fileExtension)
        {
            string filePrefixName;

            switch (options)
            {
                case EncryptVerbOptions opts:
                    filePrefixName = "EncryptedData";
                    break;

                case DecryptVerbOptions opts:
                    filePrefixName = "DecryptedData";
                    break;

                case GenerateStrongKeyVerbOptions opts:
                    filePrefixName = "SK";
                    fileExtension = ".key";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
            return filePrefixName;
        }
    }
}