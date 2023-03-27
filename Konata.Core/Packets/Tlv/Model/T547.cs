namespace Konata.Core.Packets.Tlv.Model;

/// <summary>
/// Compose of T547 "Pow"
/// </summary>
internal class T547Body : TlvBody
{
    /// <summary>
    /// Constructor for composing of TLV547 (not parsing), should be directly powered from T546
    /// struct.pack("!H128sII", 128, dst, elp, cnt) // (big-endian) unsigned short, 128 bytes, unsigned int, unsigned int
    /// </summary>
    /// <param name="tlv">original T546</param>
    public T547Body(T546Body tlv) : base()
    {
        PutBytes(tlv.Parsed);
        PutShortBE(128); // length of dst
        PutBytes(tlv.Destination);
        PutUintBE(tlv.Elapsed);
        PutUintBE(tlv.Count);
    }
}