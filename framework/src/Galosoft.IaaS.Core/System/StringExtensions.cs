using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Extensions of <see cref="String"/>
    /// </summary>
    public static class StringExtensions
    {
        // 控制字符
        private static readonly Regex ControlCharRegex = new Regex(@"[\p{C}]", RegexOptions.Compiled);

        /// <summary>
        /// 移除控制字符
        /// </summary>
        public static string RemoveControlChars(this string txt)
        {
            return ControlCharRegex.Replace(txt, string.Empty);
        }
    }
}
