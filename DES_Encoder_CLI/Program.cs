namespace DES_Encoder_CLI
{
    using System;
    using System.IO;
    using CommandLine;
    using DESEncodeDecodeLib;
    using DESEncodeDecodeLib.Interfaces;

    static class Program
    {
        static void Main(string[] args)
        {
            #region Test Data

            //ParserResult<object> parserResult =
            //    Parser.Default.ParseArguments<EncryptVerbOptions, DecryptVerbOptions>(
            //        new[] { "dec", "-i", "EncryptedData_2017-9-14_145348770", "-k", "Some.key" });

            //ParserResult<object> parserResult =
            //    Parser.Default.ParseArguments<EncryptVerbOptions, DecryptVerbOptions>(
            //        new[] { "enc", "-i", "OriginalData.txt", "-k", "Some.key" });

            #endregion

            ParserResult<object> parserResult =
                Parser.Default.ParseArguments<EncryptVerbOptions, DecryptVerbOptions>(args);

            parserResult.WithParsed<EncryptVerbOptions>(ProcessEncryptCommand)
                .WithParsed<DecryptVerbOptions>(ProcessDecryptCommand);
        }

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

        private static void GenerateOutputFileNameIfNotSet(IOutputableOption options)
        {
            if (string.IsNullOrWhiteSpace(options.OutputFilePath))
            {
                DateTime now = DateTime.Now;
                string fileExtension = Path.HasExtension(options.OutputFilePath)
                    ? $".{Path.GetExtension(options.OutputFilePath)}"
                    : string.Empty;

                string filePrefixName = options is EncryptVerbOptions
                    ? "EncryptedData"
                    : options is DecryptVerbOptions
                        ? "DecryptedData"
                        : "Output";

                options.OutputFilePath =
                    $"{filePrefixName}_{now.Year}-{now.Month}-{now.Day}_" +
                    $"{now.Hour}{now.Minute}{now.Second}{now.Millisecond}" +
                    $"{fileExtension}";
            }
        }
    }
}