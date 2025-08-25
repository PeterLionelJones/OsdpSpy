using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OsdpSpy.Listen;

namespace OsdpSpy.Tests.Listen;

internal class ThreadServiceTester : ThreadService
{
    public bool FailStart { get; set; }
    public bool ThrowException { get; set; }

    public bool WasStarted { get; private set; }
    public bool WasServicedAsynchronously { get; private set; }
    public bool WasServiced { get; private set; }
    public bool WasStopped { get; private set; }
    
    protected override bool OnStart()
    {
        if (FailStart) return false;
        WasStarted = true;
        return base.OnStart();
    }

    protected override Task OnServiceAsync()
    {
        WasServicedAsynchronously = true;
        return base.OnServiceAsync();
    }

    protected override void OnService()
    {
        WasServiced = true;

        if (ThrowException)
        {
            throw new Exception();
        }
        
        base.OnService();
    }

    protected override void OnStop()
    {
        WasStopped = true;
        base.OnStop();
    }
}

[TestFixture]
public class ThreadServiceTests
{
    private ThreadServiceTester _unit;

    [SetUp]
    public void SetUp()
    {
        _unit = new ThreadServiceTester();
    }

    [Test]
    public void Constructor_CheckStates_StatesValid()
    {
        Assert.IsFalse(_unit.FailStart);
        Assert.IsFalse(_unit.ThrowException);
        Assert.IsFalse(_unit.WasStarted);
        Assert.IsFalse(_unit.WasServicedAsynchronously);
        Assert.IsFalse(_unit.WasServiced);
        Assert.IsFalse(_unit.WasStopped);
        Assert.IsFalse(_unit.IsRunning);
    }

    [Test]
    public async Task Start_CheckStated_WasStartedAndRunning()
    {
        var tokenSource = new CancellationTokenSource();
        var stoppingToken = tokenSource.Token;
        
        _unit.Start(stoppingToken);
        await Task.Delay(100, stoppingToken);
        var isRunning = _unit.IsRunning;
        tokenSource.Cancel();
        await Task.Delay(100, CancellationToken.None);
        
        Assert.IsTrue(isRunning);
        Assert.IsTrue(_unit.WasStarted);
        Assert.IsTrue(_unit.WasServiced);
        Assert.IsTrue(_unit.WasServicedAsynchronously);
        Assert.IsTrue(_unit.WasStopped);
    }
    
    [Test]
    public async Task Start_ThrowExceptionInService_WasStartedAndRunning()
    {
        var tokenSource = new CancellationTokenSource();
        var stoppingToken = tokenSource.Token;
        
        _unit.ThrowException = true;
        _unit.Start(stoppingToken);
        await Task.Delay(100, stoppingToken);
        var isRunning = _unit.IsRunning;
        tokenSource.Cancel();
        await Task.Delay(100, CancellationToken.None);
        
        Assert.IsTrue(isRunning);
        Assert.IsTrue(_unit.WasStarted);
        Assert.IsTrue(_unit.WasServiced);
        Assert.IsTrue(_unit.WasServicedAsynchronously);
        Assert.IsTrue(_unit.WasStopped);
    }
    
    [Test]
    public void Start_FailStart_WasNotStarted()
    {
        var tokenSource = new CancellationTokenSource();
        var stoppingToken = tokenSource.Token;
        
        _unit.FailStart = true;
        _unit.Start(stoppingToken);
        
        Assert.IsFalse(_unit.IsRunning);
        Assert.IsFalse(_unit.WasStarted);
        Assert.IsFalse(_unit.WasServiced);
        Assert.IsFalse(_unit.WasServicedAsynchronously);
        Assert.IsFalse(_unit.WasStopped);
    }
}