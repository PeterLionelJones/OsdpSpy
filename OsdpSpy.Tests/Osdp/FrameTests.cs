using NUnit.Framework;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests.Osdp;

[TestFixture]
public class FrameTests
{
    private readonly byte[] _tx =
    {
        0x53, 0x00, 0x09, 0x00, 0x07, 0x61, 0x00, 0x90, 0x3F
    };
    
    private readonly byte[] _rx =
    {
        0x53, 0x80, 0x14, 0x00, 0x07, 0x45, 0x6C, 0x4E, 
        0x86, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 
        0x00, 0x01, 0x02, 0x08
    };
    
    [Test]
    public void Constructor_ConstructFromByteArray_Verify()
    {
        var testObject = new Frame(_tx);
        
        Assert.IsFalse(testObject.IsSecure);
        Assert.That(testObject.Address != Frame.ConfigurationAddress);
        Assert.IsTrue(testObject.IsCp);
        Assert.IsFalse(testObject.IsPd);
        Assert.IsFalse(testObject.GoodMac);
        //Assert.IsTrue(testObject.GoodCheck);
        Assert.That(testObject.Sequence < 4);
        Assert.IsTrue(testObject.UseCRC16);
        Assert.IsFalse(testObject.IsSecure);
        Assert.IsNull(testObject.SecurityBlock);
        Assert.That(testObject.Command == Command.ID);
        Assert.IsFalse(testObject.HasMac);
   }
}