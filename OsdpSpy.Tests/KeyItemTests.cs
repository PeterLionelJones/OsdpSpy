using System.Linq;
using NUnit.Framework;

namespace OsdpSpy.Tests;

[TestFixture]
public class KeyItemTests
{
    [Test]
    public void Constructor_Construct_FieldsLoaded()
    {
        var uid = new byte[] { 0x12, 0x34, 0x56, 0x78 };
        var key = new byte[] { 0x87, 0x65, 0x43, 0x21 };
        
        var keyItem = new KeyItem { Uid = uid, Key = key };
        
        Assert.That(keyItem.Uid.SequenceEqual(uid));
        Assert.That(keyItem.Key.SequenceEqual(key));
    }
}