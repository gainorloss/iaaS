using System;
using System.Text;

namespace Galosoft.IaaS.Nonce
{
    internal class NonceGenerator
        : INonceGenerator
    {
        public static string[] strs = new[]
          {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        public string Generate(int size = 6)
        {
            var len = strs.Length;
            var sb = new StringBuilder();

            var rd = new Random();
            for (int i = 0; i < size; i++)
            {
                var idx = rd.Next(0, len);
                sb.Append(strs[idx]);
            }

            return sb.ToString();
        }
    }
}
