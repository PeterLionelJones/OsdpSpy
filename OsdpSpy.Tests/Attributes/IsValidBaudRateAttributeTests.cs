using NUnit.Framework;
using OsdpSpy.Attributes;

namespace OsdpSpy.Tests.Attributes;

[TestFixture]
public class IsValidBaudRateAttributeTests
{
    private IsValidBaudRateAttribute _unit;
    
    [SetUp]
    public void SetUp()
    {
        _unit = new IsValidBaudRateAttribute();
    }
    
    [Test]
    [TestCase("auto")]
    [TestCase("AUTO")]
    [TestCase("Auto")]
    [TestCase("9600")]
    [TestCase("19200")]
    [TestCase("38400")]
    [TestCase("57600")]
    [TestCase("115200")]
    [TestCase("230400")]
    public void IsValid_ValidBaudRateString_ReturnsTrue(object input)
    {
        var isValid = _unit.IsValid(input);
        
        Assert.IsTrue(isValid);
    }

    [Test]
    [TestCase((int) 9600)]
    [TestCase((float) 9600.0)]
    [TestCase((double) 9600.0)]
    [TestCase("9601")]
    [TestCase(null)]
    public void IsValid_InvalidBaudRateObject_ReturnsFalse(object input)
    {
        var isValid = _unit.IsValid(input);
        
        Assert.IsFalse(isValid);
    }
}