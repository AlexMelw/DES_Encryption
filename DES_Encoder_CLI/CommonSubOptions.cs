namespace DES_Encoder_CLI
{
    using CommandLine;

    abstract class CommonSubOptions
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data to be encrypted with a given key.")]
        public string InputFilePath { get; set; }


        [Option('k', "key", Required = true,
            HelpText = "Path to file containing 64-bit key for encryption of data from supplied file.")]
        public string KeyPath { get; set; }
    }
}