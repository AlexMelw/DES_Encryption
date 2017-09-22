namespace DES_Encoder_CLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("enc", HelpText = "Enforces file encryption with the specified key.")]
    class EncryptVerbOptions : IOutputableOption
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data to be encrypted with a given key.")]
        public string InputFilePath { get; set; }


        [Option('k', "key", Required = true,
            HelpText = "Path to file containing 64-bit key for data encryption from supplied file.")]
        public string KeyPath { get; set; }

        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the decryption operation.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "DEScli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Encryption", new EncryptVerbOptions
                {
                    KeyPath = "Key64bit.ext",
                    InputFilePath = "ToBeEncrypted.ext",
                    OutputFilePath = "Encrypted.ext"
                });
            }
        }
    }
}