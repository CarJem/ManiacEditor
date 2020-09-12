using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Extensions
{
    public static class LinqExtensions
    {
        public static bool In<T>(this T o, params T[] values)
        {
            if (values == null) return false;

            return values.Contains(o);
        }
        public static bool In<T>(this T o, IEnumerable<T> values)
        {
            if (values == null) return false;

            return values.Contains(o);
        }
        public static IEnumerable<T> GetNth<T>(this IEnumerable<T> list, int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n");
            if (n > 0)
            {
                int c = 0;
                foreach (var e in list)
                {
                    if (c % n == 0)
                        yield return e;
                    c++;
                }
            }
        }
        public static IEnumerable<T> GetNth<T>(this IList<T> list, int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("n");
            if (n > 0)
                for (int c = 0; c < list.Count; c += n)
                    yield return list[c];
        }
    }
}
