namespace DES_Encoder_CLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("enc", HelpText = "Enforces file encryption with the specified key.")]
    class EncryptVerbOptions : CommonSubOptions, IOutputableOption
    {
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

        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the decryption operation.")]
        public string OutputFilePath { get; set; }
    }
}