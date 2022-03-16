using System;
using Konata.Core.Events.Model;
using Konata.Core.Utils.Extensions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message;

internal class SliceControl
{
    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Total slice count <br/>
    /// </summary>
    public uint Total { get; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Current slice id <br/>
    /// </summary>
    public uint Index { get; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Slice flags <br/>
    /// </summary>
    public uint Id { get; }

    private SliceControl (uint total, uint index, uint id)
    {
        Id = id;
        Total = total;
        Index = index;
    }

    public static SliceControl Create(uint total, uint index, uint id)
        => new(total, index, id);
}
