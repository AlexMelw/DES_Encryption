namespace DES_Encoder_CLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("dec", HelpText = "Enforces file decrypt with the specified key.")]
    class DecryptVerbOptions : IOutputableOption
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data to be decrypted with a given key.")]
        public string InputFilePath { get; set; }


        [Option('k', "key", Required = true,
            HelpText = "Path to file containing 64-bit key for data decryption from supplied file.")]
        public string KeyPath { get; set; }

        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the encryption operation.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "DEScli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Decryption", new EncryptVerbOptions
                {
                    KeyPath = "Key64bit.ext",
                    InputFilePath = "ToBeDecrypted.ext",
                    OutputFilePath = "Decrypted.ext"
                });
            }
        }
    }
}