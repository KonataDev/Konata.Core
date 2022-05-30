namespace Konata.Core.Utils;

/// <summary>
/// GUID Generator
/// </summary>
internal static class Guid
{
    /// <summary>
    /// Generate a guid as bytes
    /// </summary>
    /// <returns></returns>
    public static byte[] GenerateBytes()
        => System.Guid.NewGuid().ToByteArray();

    /// <summary>
    /// Generate a guid as a string
    /// </summary>
    /// <returns></returns>
    public static string GenerateString()
        => System.Guid.NewGuid().ToString();
}
