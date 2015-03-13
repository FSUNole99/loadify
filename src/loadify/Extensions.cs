using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace loadify
{
    public static class Extensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static void Sleep(this TimeSpan timeSpan)
        {
            new ManualResetEvent(false).WaitOne(timeSpan.Milliseconds);
        }

        public static string HTTPSToHTTP(string url)
        {
            return Regex.Replace(url, "https://", "http://");
        }
    }
}
