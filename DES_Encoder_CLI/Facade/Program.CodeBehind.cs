namespace DEScli
{
    using System;
    using System.IO;
    using DESEncodeDecodeLib;
    using DESEncodeDecodeLib.Interfaces;
    using DESEncodeDecodeLib.Utils;

    static partial class Program
    {
        private static void GenerateOutputFileNameIfNotSet(IOutputableOption options)
        {
            if (string.IsNullOrWhiteSpace(options.OutputFilePath))
            {
                string fileExtension = CreateFileExtension(options);
                string filePrefixName = CreateFilePrefixName(options, ref fileExtension);

                DateTime now = DateTime.Now;
                options.OutputFilePath =
                    $"{filePrefixName}_" +
                    $"{now.Year}-{now.Month}-{now.Day}_" +
                    $"{now.Hour}{now.Minute}{now.Second}{now.Millisecond}" +
                    $"{fileExtension}";
            }
        }

        private static string CreateFileExtension(IOutputableOption options)
        {
            string fileExtension = Path.HasExtension(options.OutputFilePath)
                ? $".{Path.GetExtension(options.OutputFilePath)}"
                : string.Empty;
            return fileExtension;
        }

        private static string CreateFilePrefixName(IOutputableOption options, ref string fileExtension)
        {
            string filePrefixName;

            switch (options)
            {
                case EncryptVerbOptions opts:
                    filePrefixName = "EncryptedData";
                    break;

                case DecryptVerbOptions opts:
                    filePrefixName = "DecryptedData";
                    break;

                case GenerateStrongKeyVerbOptions opts:
                    filePrefixName = "SK";
                    fileExtension = ".key";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
            return filePrefixName;
        }
    }
}