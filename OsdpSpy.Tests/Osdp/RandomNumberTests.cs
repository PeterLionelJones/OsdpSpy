using NUnit.Framework;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests.Osdp;

[TestFixture]
public class RandomNumberTests
{
    [Test]
    public void Constructor_Construct_VerifyNumber()
    {
        var testObject = new RandomNumber();
        
        Assert.That(testObject.Data.Length == 8);
    }

    [Test]
    public void Constructor_Construct_VerifyString()
    {
        var testObject = new RandomNumber();
        
        Assert.That(testObject.ToString().Length == 23);
    }
}