using System;
using System.Collections.Generic;
using System.Linq;

namespace Konata.Core.Utils.Extensions;

public static class ArrayExt
{
    /// <summary>
    /// Cut an array into pieces.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="blocksz"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T>[] Slices<T>(this IEnumerable<T> @this, int blocksz)
    {
        var array = @this.ToList();
        var alloc = (int) Math.Ceiling((double) array.Count / blocksz);
        var list = new IEnumerable<T>[alloc];

        // Create the slices
        for (var i = 0; i < alloc; i++)
        {
            var start = i * blocksz;
            var end = i + 1 == alloc ? array.Count : (i + 1) * blocksz;
            list[i] = array.ToArray()[start..end].ToList();
        }

        return list;
    }
}
