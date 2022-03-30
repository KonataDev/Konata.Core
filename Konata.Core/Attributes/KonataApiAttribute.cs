using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class KonataApiAttribute : Attribute
{
    /// <summary>
    /// API version
    /// </summary>
    public uint Version { get; }

    /// <summary>
    /// Is an experimental API
    /// </summary>
    public bool Experimental { get; }
    
    public KonataApiAttribute(uint version, bool experimental = false)
    {
        Version = version;
        Experimental = experimental;
    }
}
