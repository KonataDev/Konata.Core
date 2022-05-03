using System;

namespace Konata.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateEventsAttribute : Attribute
{
    public GenerateEventsAttribute(params Type[] types)
    {

    }
}