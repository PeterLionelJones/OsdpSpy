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
        Assert.That(_unit.FailStart, Is.False);
        Assert.That(_unit.ThrowException, Is.False);
        Assert.That(_unit.WasStarted, Is.False);
        Assert.That(_unit.WasServicedAsynchronously, Is.False);
        Assert.That(_unit.WasServiced, Is.False);
        Assert.That(_unit.WasStopped, Is.False);
        Assert.That(_unit.IsRunning, Is.False);
    }

    [Test]
    public async Task Start_CheckStated_WasStartedAndRunning()
    {
        var tokenSource = new CancellationTokenSource();
        var stoppingToken = tokenSource.Token;
        
        _unit.Start(stoppingToken);
        await Task.Delay(100, stoppingToken);
        var isRunning = _unit.IsRunning;
        await tokenSource.CancelAsync();
        await Task.Delay(100, CancellationToken.None);
        
        Assert.That(isRunning, Is.True);
        Assert.That(_unit.WasStarted, Is.True);
        Assert.That(_unit.WasServiced, Is.True);
        Assert.That(_unit.WasServicedAsynchronously, Is.True);
        Assert.That(_unit.WasStopped, Is.True);
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
        await tokenSource.CancelAsync();
        await Task.Delay(100, CancellationToken.None);
        
        Assert.That(isRunning);
        Assert.That(_unit.WasStarted, Is.True);
        Assert.That(_unit.WasServiced, Is.True);
        Assert.That(_unit.WasServicedAsynchronously, Is.True);
        Assert.That(_unit.WasStopped, Is.True);
    }
    
    [Test]
    public void Start_FailStart_WasNotStarted()
    {
        var tokenSource = new CancellationTokenSource();
        var stoppingToken = tokenSource.Token;
        
        _unit.FailStart = true;
        _unit.Start(stoppingToken);
        
        Assert.That(_unit.IsRunning, Is.False);
        Assert.That(_unit.WasStarted, Is.False);
        Assert.That(_unit.WasServiced, Is.False);
        Assert.That(_unit.WasServicedAsynchronously, Is.False);
        Assert.That(_unit.WasStopped, Is.False);
    }
}