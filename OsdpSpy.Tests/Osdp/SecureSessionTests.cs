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
        
        Assert.IsNotNull(testObject.Smac1);
        Assert.IsNotNull(testObject.Smac2);
        Assert.IsNotNull(testObject.Enc);
        Assert.IsNotNull(testObject.Mac);
    }
}