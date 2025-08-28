using NUnit.Framework;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests.Osdp;

[TestFixture]
public class SecureSessionTests
{
    [Test]
    public void Constructor_Construct_Verify()
    {
        var testObject = new SecureSession();
        
        Assert.That(testObject.Smac1, Is.Not.Null);
        Assert.That(testObject.Smac2, Is.Not.Null);
        Assert.That(testObject.Enc, Is.Not.Null);
        Assert.That(testObject.Mac, Is.Not.Null);
    }
}