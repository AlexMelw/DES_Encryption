namespace DES_Encoder_CLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("dec", HelpText = "Enforces file decrypt with the specified key.")]
    class DecryptVerbOptions : CommonSubOptions, IOutputableOption
    {
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

        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the encryption operation.")]
        public string OutputFilePath { get; set; }
    }
}