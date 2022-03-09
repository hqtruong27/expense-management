using System.Security.Cryptography;

namespace ExpenseManagement.Api.Common
{
    public class RandomStringGenerator
    {
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        /// <summary>
        /// Returns a random string.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string Next()
        {
            byte[] data = new byte[4 * 1];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }

            System.Text.StringBuilder result = new(1);
            {
                for (int i = 0; i < 1; i++)
                {
                    var rnd = BitConverter.ToUInt32(data, i * 4);
                    var idx = rnd % _chars.Length;

                    result.Append(_chars[idx]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns random string of the specified size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string Next(int size)
        {
            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            System.Text.StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % _chars.Length;

                result.Append(_chars[idx]);
            }

            return result.ToString();
        }

        public static void PerformTestRandomString(Func<int, string> generator)
        {
            const int REPETITIONS = 1000000;
            const int KEY_SIZE = 32;
            Dictionary<char, int> counts = new();
            foreach (var ch in _chars) counts.Add(ch, 0);

            for (int i = 0; i < REPETITIONS; i++)
            {
                var key = generator(KEY_SIZE);
                foreach (var ch in key) counts[ch]++;
            }

            int totalChars = counts.Values.Sum();
            foreach (var ch in _chars)
            {
                Console.WriteLine($"{ch}: {100.0 * counts[ch] / totalChars:#.000}%");
            }
        }
    }
}