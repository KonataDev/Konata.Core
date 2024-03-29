﻿using System.Runtime.CompilerServices;
using Konata.Core.Utils.IO;

namespace Konata.Core.Utils.Extensions;

internal static class StringExt
{
    /// <summary>
    /// Convert hex string to bytes 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] UnHex(this string data)
        => ByteConverter.UnHex(data);

    /// <summary>
    /// Convert base64 string to bytes
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Atob(this string data)
        => ByteConverter.UnBase64(data);
    
    /// <summary>
    /// Convert base64 string to plain string
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Atoa(this string data)
        => ByteConverter.UnBase64String(data);
}
