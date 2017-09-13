namespace DES_Encoder_CLI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using CommandLine;
    using CommandLine.Text;

    static class Program
    {
        static void Main(string[] args)
        {
            //EncryptVerbOptions encrypt = new EncryptVerbOptions
            //{
            //    KeyPath = @"""Lalalalla"""
            //};

            //string line = Parser.Default.FormatCommandLine(encrypt);

            //Console.Out.WriteLine("line = {0}", line);

            Parser.Default.ParseArguments<EncryptVerbOptions, DecryptVerbOptions>(args)
                .WithParsed<EncryptVerbOptions>(opts => { })
                .WithParsed<DecryptVerbOptions>(opts => { })
                .WithNotParsed(errors => { });
        }

        private static void ProcessCommand(string verb, object subOptions) { }
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
    class EncryptVerbOptions : CommonSubOptions
    {
        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the decryption operation.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "DesCLI")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Encryption", new EncryptVerbOptions
                {
                    KeyPath = "MyKey.txt",
                    InputFilePath = "ToBeEncrypted.txt",
                    OutputFilePath = "Encrypted.txt"
                });
            }
        }

    }

    [Verb("dec", HelpText = "Enforces file decrypt with the specified key.")]
    class DecryptVerbOptions : CommonSubOptions
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