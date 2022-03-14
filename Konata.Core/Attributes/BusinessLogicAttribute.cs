using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Attributes;

/// <summary>
/// Business logic Attribute
/// </summary>
internal class BusinessLogicAttribute : Attribute
{
    public string LogicName { get; }

    public string Description { get; }

    public BusinessLogicAttribute(string name, string description)
    {
        LogicName = name;
        Description = description;
    }
}
