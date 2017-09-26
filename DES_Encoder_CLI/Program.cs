namespace DEScli
{
    using CommandLine;

    static partial class Program
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


            //parserResult.WithParsed<EncryptVerbOptions>(ProcessEncryptCommand)
            //    .WithParsed<DecryptVerbOptions>(ProcessDecryptCommand)
            //    .WithParsed<GenerateStrongKeyVerbOptions>(ProcessGenerateSKCommand);

            #endregion

            Parser.Default.ParseArguments<GenerateStrongKeyVerbOptions, EncryptVerbOptions, DecryptVerbOptions>(args)
                .WithParsed<EncryptVerbOptions>(ProcessEncryptCommand)
                .WithParsed<DecryptVerbOptions>(ProcessDecryptCommand)
                .WithParsed<GenerateStrongKeyVerbOptions>(ProcessGenerateSKCommand);
        }
    }
}