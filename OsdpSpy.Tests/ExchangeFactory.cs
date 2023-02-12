using NUnit.Framework;

namespace OsdpSpy.Tests;

[TestFixture]
public class ExchangeFactory
{
    [Test]
    public void Create_ValidParameters_CreatedSuccessfully()
    {
        var _factory = new ExchangeFactory();
    }
}