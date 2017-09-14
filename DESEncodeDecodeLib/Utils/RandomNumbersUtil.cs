namespace DESEncodeDecodeLib.Utils
{
    using System.Security.Cryptography;

    public static class RandomNumbersUtil
    {
        public static byte[] GenerateRandomNumbers(int byteLength)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumbers = new byte[byteLength];
                randomNumberGenerator.GetBytes(randomNumbers);

                return randomNumbers;
            }
        }
    }
}