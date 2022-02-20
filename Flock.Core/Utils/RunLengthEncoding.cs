using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;

namespace Flock.Core.Utils {

    internal static class RunLengthEncoding {

        // Algorithm: https://www.rosettacode.org/wiki/Run-length_encoding#Linq

        public static string Encode(string input) => input.Length == 0 ? "" : input.Skip(1)
            .Aggregate((t: input[0].ToString(), o: Empty<string>()),
                (a, c) => a.t[0] == c ? (a.t + c, a.o) : (c.ToString(), a.o.Append(a.t)),
                a => a.o.Append(a.t).Select(p => (key: p.Length, chr: p[0])))
            .Select(p => $"{p.key}{p.chr}")
            .StringConcat();
        
        public static string Decode(string input) => input
            .Aggregate((t: "", o: Empty<string>()), (a, c) => !char.IsDigit(c) ? ("", a.o.Append(a.t + c)) : (a.t + c, a.o)).o
            .Select(p => new string(p.Last(), int.Parse(string.Concat(p.Where(char.IsDigit)))))
            .StringConcat();

        private static string StringConcat(this IEnumerable<string> seq) => string.Concat(seq);
        
    }
}
