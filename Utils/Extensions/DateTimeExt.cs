using System;
using System.Runtime.CompilerServices;

namespace Konata.Core.Utils.Extensions;

public static class DateTimeExt
{
    /// <summary>
    /// Convert DateTime to unix time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Epoch(this DateTime time)
        => (time.Ticks - 621355968000000000) / 10000;
}
