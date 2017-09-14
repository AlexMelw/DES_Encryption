namespace DES_Encoder_CLI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CommandLine;
    using CommandLine.Text;
    using DESEncodeDecodeLib;
    using DESEncodeDecodeLib.Interfaces;
    using EasySharp.NHelpers.CustomExMethods;

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

            //Console.Out.WriteLine(inputByteArray.ToUtf8String());

            byte[] keyByteArray = File.ReadAllBytes(options.KeyPath);

            //Console.Out.WriteLine(keyByteArray.ToUtf8String());

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

    abstract class CommonSubOptions
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data to be encrypted with a given key.")]
        public string InputFilePath { get; set; }


        [Option('k', "key", Required = true,
            HelpText = "Path to file containing 64-bit key for encryption of data from supplied file.")]
        public string KeyPath { get; set; }
    }

    [Verb("enc", HelpText = "Enforces file encryption with the specified key.")]
    class EncryptVerbOptions : CommonSubOptions, IOutputableOption
    {
        [Usage(ApplicationAlias = "DesCLI")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Encryption", new EncryptVerbOptions
                {
                    KeyPath = "MyKey.txt",
                    InputFilePath = "ToBeEncrypted.txt",
                    OutputFilePath = "Encrypted.txt"
                });
            }
        }

        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the decryption operation.")]
        public string OutputFilePath { get; set; }
    }

    internal interface IOutputableOption
    {
        string OutputFilePath { get; set; }
    }

    [Verb("dec", HelpText = "Enforces file decrypt with the specified key.")]
    class DecryptVerbOptions : CommonSubOptions, IOutputableOption
    {
        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the encryption operation.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "DesCLI")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Decryption", new EncryptVerbOptions
                {
                    KeyPath = "MyKey.txt",
                    InputFilePath = "ToBeDecrypted.txt",
                    OutputFilePath = "Decrypted.txt"
                });
            }
        }
    }
}