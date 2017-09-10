namespace DESEncodeDecodeLib.Utils
{
    using System.Security.Cryptography;

    public static class RandomNumbersUtil
    {
        public static byte[] GenerateRandomNumbers(int length)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumbers = new byte[length];
                randomNumberGenerator.GetBytes(randomNumbers);

                return randomNumbers;
            }
        }
    }
}