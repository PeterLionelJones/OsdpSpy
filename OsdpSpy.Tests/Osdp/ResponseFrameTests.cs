using System;
using System.Threading;
using NUnit.Framework;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests.Osdp;

[TestFixture]
public class ResponseFrameTests
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
    public void AddByte_AddExtraBytesAfterValidFrame_FrameIsStillComplete()
    {
        var testObject = new ResponseFrame();

        foreach (var inch in _tx)
        {
            testObject.AddByte(inch);
        }

        for (var i = 0; i < 4; ++i)
        {
            testObject.AddByte((byte)('0' + i));
        }
        
        Assert.IsTrue(testObject.IsComplete);
    }

    [Test]
    public void AddByte_FrameTooLong_FrameNotComplete()
    {
        var tx = new byte[272];
        Buffer.BlockCopy(_tx, 0, tx, 0, _tx.Length);
        tx[2] = (byte)(tx.Length & 0xFF);
        tx[3] = (byte)(tx.Length >> 8);

        var testObject = new ResponseFrame();

        foreach (var inch in tx)
        {
            testObject.AddByte(inch);
        }
        
        Assert.IsFalse(testObject.IsComplete);
    }
    
    [Test]
    public void AddByte_CorrupteFrameData_ThrowsExceptionInternally()
    {
        var testObject = new ResponseFrame();
        var clobbered = false;
        
        foreach (var inch in _tx)
        {
            testObject.AddByte(_tx[0]);
            if (!clobbered) testObject.KillFrame();
        }
        
        Assert.IsFalse(testObject.IsComplete);
    }

    [Test]
    public void AddByte_MeasureReceptionTime_MeasuresCorrectly()
    {
        var testObject = new ResponseFrame();

        foreach (var inch in _tx)
        {
            testObject.AddByte(inch);
            Thread.Sleep(1);
        }

        var elapsed = testObject.Elapsed;
        var precise = testObject.ElapsedWithPrecision;
        
        Assert.IsTrue(testObject.IsComplete);
        Assert.That(elapsed > 8);
        Assert.That(precise > 8.0);
    }

    [Test]
    public void TraceString_GetTxFrame_IsValid()
    {
        var testObject = new ResponseFrame();

        foreach (var inch in _tx)
        {
            testObject.AddByte(inch);
        }

        var traceString = testObject.TraceString;

        Assert.IsTrue(testObject.IsComplete);
        Assert.That(traceString.Contains("TX:"));
    }

    [Test]
    public void TraceString_GetRxFrame_IsValid()
    {
        var testObject = new ResponseFrame();

        foreach (var inch in _rx)
        {
            testObject.AddByte(inch);
        }

        var traceString = testObject.TraceString;

        Assert.IsTrue(testObject.IsComplete);
        Assert.That(traceString.Contains("RX:"));
    }

}