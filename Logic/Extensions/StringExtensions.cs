using System;
using System.Linq;

namespace Logic.Extensions
{
    public static class StringExtensions
    {
        public static string TakeNLines(this string source, int count)
        {
            return string.Join(Environment.NewLine, source.Split(Environment.NewLine).Take(count));
        }
    }
}