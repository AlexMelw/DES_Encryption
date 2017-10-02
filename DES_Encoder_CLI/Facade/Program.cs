namespace DEScli
{
    using CommandLine;

    static partial class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GenerateStrongKeyVerbOptions, EncryptVerbOptions, DecryptVerbOptions>(args)
                .WithParsed<EncryptVerbOptions>(ProcessEncryptCommand)
                .WithParsed<DecryptVerbOptions>(ProcessDecryptCommand)
                .WithParsed<GenerateStrongKeyVerbOptions>(ProcessGenerateSKCommand);
        }
    }
}