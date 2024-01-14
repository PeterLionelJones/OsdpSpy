using System;
using NUnit.Framework;
using OsdpSpy.Extensions;

namespace OsdpSpy.Tests.Extensions;

[TestFixture]
public class ToHexStringExtensionsTests
{
    [Test]
    public void ToHexString_SimpleConversion_ExpectedOutput()
    {
        var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90 };
        var expectedOutput = "12 34 56 78 90";

        var output = input.ToHexString();
        
        Assert.That(output.Equals(expectedOutput));
    }
    
    [Test]
    public void ToHexString_FromOffset_ExpectedOutput()
    {
        var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90 };
        var offset = 2;
        var expectedOutput = "56 78 90";

        var output = input.ToHexString(offset);
        
        Assert.That(output.Equals(expectedOutput));
    }
    
    [Test]
    public void ToHexString_FromOffsetWithLength_ExpectedOutput()
    {
        var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90 };
        var offset = 2;
        var length = 2;
        var expectedOutput = "56 78";

        var output = input.ToHexString(offset, length);
        
        Assert.That(output.Equals(expectedOutput));
    }

    [Test]
    public void ToHexString_NullInput_ThrowsExcepion()
    {
        byte[] input = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            input.ToHexString();
        });
    }

    [Test]
    public void ToHexString_NullInputWithLength_ThrowsExcepion()
    {
        byte[] input = null;
        var offset = 2;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            input.ToHexString(offset);
        });
    }

    [Test]
    public void ToHexString_NullInputWithLengthAndOffset_ThrowsExcepion()
    {
        byte[] input = null;
        var offset = 2;
        var length = 2;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            input.ToHexString(offset, length);
        });
    }
}