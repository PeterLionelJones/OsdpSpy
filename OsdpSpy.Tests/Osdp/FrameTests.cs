using System.Collections.Generic;
using NUnit.Framework;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests.Osdp;

[TestFixture]
public class FrameTests
{
    private readonly byte[] _plainTxWithChecksum =
    {
        0x53, 0x00, 0x15, 0x00, 0x01, 0x69, 0x00, 0x00, 0x02, 0x01, 
        0x05, 0x00, 0x02, 0x07, 0x00, 0x00, 0x32, 0x32, 0x04, 0x04, 
        0xB1
    };

    private readonly byte[] _plainRxWithChecksum =
    {
        0x53, 0x80, 0x0A, 0x00, 0x02, 0x53, 0x00, 0x01, 0x31, 0x9C
    };
    
    private readonly byte[] _plainTxWithCrC =
    {
        0x53, 0x00, 0x09, 0x00, 0x07, 0x61, 0x00, 0x90, 0x3F
    };

    private readonly byte[] _plainRxWithCrc =
    {
        0x53, 0x80, 0x14, 0x00, 0x07, 0x45, 0x6C, 0x4E, 
        0x86, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 
        0x00, 0x01, 0x02, 0x08
    };
    
    private readonly byte[] _secureTx =
    {
        0x53, 0x00, 0x1E, 0x00, 0x0E, 0x02, 0x17, 0x6A, 
        0x56, 0xD7, 0x13, 0x95, 0xD2, 0x21, 0xD7, 0xA0, 
        0x07, 0x51, 0xEB, 0x38, 0x10, 0x7A, 0xFC, 0x46, 
        0x8F, 0xE5, 0x30, 0x66, 0xE8, 0xFD
    };

    private readonly byte[] _secureRx =
    {
        0x53, 0x80, 0x1E, 0x00, 0x0D, 0x02, 0x18, 0x53, 
        0xCB, 0xAA, 0x84, 0xAD, 0xF0, 0xBF, 0x89, 0x52, 
        0xD1, 0x2D, 0xB8, 0xEC, 0x28, 0xD1, 0x60, 0x2A, 
        0x03, 0x19, 0xB3, 0x58, 0xCF, 0x31
    };
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    public void Constructor_ConstructFromByteArray_Verify(int testCase)
    {
        var testCases = new List<byte[]>
        {
            _plainTxWithChecksum, 
            _plainRxWithChecksum,
            _plainTxWithCrC, 
            _plainRxWithCrc, 
            _secureTx, 
            _secureRx
        }.ToArray();

        var frameData = testCases[testCase];
        
        var testObject = new Frame(frameData);
        
        Assert.That(testObject.Address != Frame.ConfigurationAddress);
        Assert.IsTrue(testObject.GoodCheck);
        Assert.That(testObject.Sequence < 4);
   }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void Constructor_VerifyAcuFrames_Succeeds(int testCase)
    {
        var testCases = new List<byte[]>
        {
            _plainTxWithChecksum, 
            _plainTxWithCrC, 
            _secureTx, 
        }.ToArray();

        var frameData = testCases[testCase];
        
        var testObject = new Frame(frameData);
        
        Assert.That(testObject.Address != Frame.ConfigurationAddress);
        Assert.IsTrue(testObject.IsAcu);
        Assert.IsFalse(testObject.IsPd);
        Assert.IsTrue(testObject.GoodCheck);
        Assert.That(testObject.Sequence < 4);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void Constructor_VerifyPdFrames_Succeeds(int testCase)
    {
        var testCases = new List<byte[]>
        {
            _plainRxWithChecksum, 
            _plainRxWithCrc, 
            _secureRx, 
        }.ToArray();

        var frameData = testCases[testCase];
        
        var testObject = new Frame(frameData);
        
        Assert.That(testObject.Address != Frame.ConfigurationAddress);
        Assert.IsFalse(testObject.IsAcu);
        Assert.IsTrue(testObject.IsPd);
        Assert.IsTrue(testObject.GoodCheck);
        Assert.That(testObject.Sequence < 4);
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void Constructor_VerifySecureFrames_Succeeds(int testCase)
    {
        var testCases = new List<byte[]>
        {
            _secureTx, 
            _secureRx
        }.ToArray();

        var frameData = testCases[testCase];
        
        var testObject = new Frame(frameData);
        
        Assert.IsTrue(testObject.IsSecure);
        Assert.That(testObject.Address != Frame.ConfigurationAddress);
        Assert.IsFalse(testObject.GoodMac);
        Assert.IsTrue(testObject.GoodCheck);
        Assert.That(testObject.Sequence < 4);
        Assert.IsTrue(testObject.UseCrc16);
        Assert.IsTrue(testObject.IsSecure);
        Assert.IsNotNull(testObject.SecurityBlock);
        Assert.IsTrue(testObject.HasMac);
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Constructor_VerifyPlainText_Succeeds(int testCase)
    {
        var testCases = new List<byte[]>
        {
            _plainTxWithChecksum, 
            _plainRxWithChecksum,
            _plainTxWithCrC, 
            _plainRxWithCrc 
        }.ToArray();

        var frameData = testCases[testCase];
        
        var testObject = new Frame(frameData);
        
        Assert.IsFalse(testObject.IsSecure);
        Assert.That(testObject.Address != Frame.ConfigurationAddress);
        Assert.IsTrue(testObject.GoodCheck);
        Assert.That(testObject.Sequence < 4);
        Assert.IsFalse(testObject.IsSecure);
        Assert.IsNull(testObject.SecurityBlock);
        Assert.IsFalse(testObject.HasMac);
    }
}