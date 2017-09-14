namespace DES_Encoder_CLI
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("gsk", HelpText = "Generates a 64-bit Strong Key using RNGCryptoServiceProvider class " +
                            "that resides in System.Security.Cryptography namespace. (Recommended)")]
    class GenerateStrongKeyVerbOptions : IOutputableOption
    {
        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the generated 64-bit strong key.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "DEScli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Strong Key Generation", new GenerateStrongKeyVerbOptions
                {
                    OutputFilePath = "Strong.key"
                });
            }
        }
    }
}