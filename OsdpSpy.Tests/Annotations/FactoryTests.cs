using System;
using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class FactoryTests
{
    private class Class
    {
        public string Field { get; init; }
    }
    
    [Test]
    public void Create_SpecifyType_ReturnsRequestedType()
    {
        var obj = new Class { Field = "Field" };
        var provider = new Mock<IServiceProvider>();
        var factory = new Factory<Class>(provider.Object);
        
        provider
            .Setup(o => o.GetService(typeof(Class)))
            .Returns(new Class { Field = "Field" });

        var returnedObj = factory.Create();

        Assert.IsNotNull(returnedObj);
        Assert.That(returnedObj.Field == obj.Field);
    }
}