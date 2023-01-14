using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BroadVoicePOC.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
                    .Replace(toFind, ch => @"\" + ch)
                    .Replace('_', '.')
                    .Replace("%", ".*") + @"\z", RegexOptions.Singleline)
                .IsMatch(toSearch);
        }
    }
}
