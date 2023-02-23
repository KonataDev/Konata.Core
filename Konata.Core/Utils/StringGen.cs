using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Utils;

internal static class StringGen
{
    private const string TemplateNum = "0123456789";
    private const string TemplateHex = "0123456789ABCDEF";

    private const string TemplateNormal = "0123456789" +
                                          "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                          "abcdefghijkimnopqrstuvwxyz";

    private const string TemplateNormalSym = "0123456789" +
                                             "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                             "abcdefghijkimnopqrstuvwxyz" +
                                             "!@#$%^&*()_+-=[]{},.<>/?|;:" +
                                             "\\\'\"";

    /// <summary>
    /// Get random hex string
    /// </summary>
    /// <param name="length">Required length</param>
    /// <param name="cases">Character cases, true is upper</param>
    /// <returns></returns>
    public static string GetRandHex(int length, bool cases = true)
        => GetRandStrFromTemplate(cases ? TemplateHex : TemplateHex.ToLower(), length);

    /// <summary>
    /// Get random number string
    /// </summary>
    /// <param name="length">Required length</param>
    /// <returns></returns>
    public static string GetRandNumber(int length)
        => GetRandStrFromTemplate(TemplateNum, length);

    /// <summary>
    /// Get random string
    /// </summary>
    /// <param name="length">Required length</param>
    /// <param name="symbol">Including symbols</param>
    /// <returns></returns>
    public static string GetRandString(int length, bool symbol = false)
        => GetRandStrFromTemplate(symbol ? TemplateNormalSym : TemplateNormal, length);

    /// <summary>
    /// Get random string from a template
    /// </summary>
    /// <param name="template">The template string</param>
    /// <param name="length">Required length</param>
    /// <returns></returns>
    public static string GetRandStrFromTemplate(string template, int length)
    {
        var rand = new Random();
        var chars = new char[length];

        // Generate random char from the template
        for (var i = 0; i < length; ++i)
        {
            var roll = rand.Next(0, template.Length);
            chars[i] = template[roll];
        }

        // Create a string
        return new string(chars);
    }
}
