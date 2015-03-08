using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace loadify
{
    public static class Extensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string ValidateFileName(this string fileName)
        {
            return !String.IsNullOrEmpty(fileName) ? Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "-")) : "";
        }

        public static void Sleep(this TimeSpan timeSpan)
        {
            new ManualResetEvent(false).WaitOne(timeSpan.Milliseconds);
        }
    }
}
