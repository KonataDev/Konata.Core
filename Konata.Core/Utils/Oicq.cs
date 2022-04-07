using System;

namespace Konata.Core.Utils;

/// <summary>
/// GUID Generator
/// </summary>
internal static class Oicq
{
    /// <summary>
    /// Group code 2 Group Uin (用户看到的群号)
    /// </summary>
    /// <returns></returns>
    public static uint GroupCode2GroupUin(uint groupCode)
    {
        var left = (uint) Math.Floor(groupCode / 1000000f);
        if (left >= 202 && left <= 212)
            left -= 202;
        else if (left >= 480 && left <= 488)
            left -= 469;
        else if (left >= 2100 && left <= 2146)
            left -= 2080;
        else if (left >= 2010 && left <= 2099)
            left -= 1943;
        else if (left >= 2147 && left <= 2199)
            left -= 1990;
        else if (left >= 2600 && left <= 2651)
            left -= 2265;
        else if (left >= 3800 && left <= 3989)
            left -= 3490;
        else if (left >= 4100 && left <= 4199)
            left -= 3890;
        return left * 1000000 + groupCode % 1000000;
    }
}
