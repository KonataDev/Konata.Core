using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Attributes;

/// <summary>
/// Konata Component Attribute
/// </summary>
internal class ComponentAttribute : Attribute
{
    public string ComponentName { get; }

    public string Description { get; }

    public ComponentAttribute(string name, string description)
    {
        ComponentName = name;
        Description = description;
    }
}
