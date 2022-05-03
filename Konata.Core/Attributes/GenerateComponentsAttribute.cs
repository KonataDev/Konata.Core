using System;

namespace Konata.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateComponentsAttribute : Attribute
{
    public GenerateComponentsAttribute(string methodName, params Type[] types)
    {

    }
}