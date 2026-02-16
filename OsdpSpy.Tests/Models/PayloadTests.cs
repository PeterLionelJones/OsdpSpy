using System.Linq;
using NUnit.Framework;
using OsdpSpy.Models;

namespace OsdpSpy.Tests.Models;

[TestFixture]
public class PayloadTests
{
    [Test]
    public void Constructor_Construct_FieldsLoaded()
    {
        var cipher = new byte[] { 0x12, 0x34, 0x56, 0x78 };
        var plain = new byte[] { 0x87, 0x65, 0x43, 0x21 };
        
        var testObject = new Payload { Cipher = cipher, Plain = plain };
        
        Assert.That(testObject.Cipher.SequenceEqual(cipher));
        Assert.That(testObject.Plain.SequenceEqual(plain));
    }
}