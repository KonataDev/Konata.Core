using System.Collections.Generic;
using Konata.Core.Utils.Extensions;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest;

public class ArrayExtTest
{
    [Test]
    public void CreateSlices()
    {
        var array = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
        var slices = array.Slices(2);

        Assert.AreEqual(slices.Length, 5);
    }

    [Test]
    public void CreateSlicesFromList()
    {
        var array = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
        var slices = array.Slices(2);

        Assert.AreEqual(slices.Length, 5);
    }
    
    [Test]
    public void CreateSlicesFromList2()
    {
        var array = new List<int> {1};
        var slices = array.Slices(2);

        Assert.AreEqual(slices.Length, 1);
    }
}
