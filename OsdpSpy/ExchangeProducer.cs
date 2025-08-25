using System;
using OsdpSpy.Abstractions;

namespace OsdpSpy;

public class ExchangeProducer : IExchangeProducer
{
    public ExchangeProducer(IExchangeFactory factory)
        => _factory = factory;

    private readonly IExchangeFactory _factory;
    private IFrameProducer _input;
    private IExchange _current;
    private long _sequence = 1;

    private void OnFrame(object sender, IFrameProduct product)
    {
        if (product.Frame.IsAcu)
        {
            // If we have an outstanding exchange then there was a timeout.
            if (_current != null)
            {
                // Send the FrameExchange with the missing response.
                ExchangeHandler?.Invoke(this, _current);
            }
                
            // Start a new exchange with a new sequence number.
            _current = _factory.Create(_sequence++, product);
        }
        else
        {
            // Make sure we have an exchange to complete.
            if (_current == null) return;
                
            // Complete the exchange.
            _current.AddReceived(product);
                    
            // The exchange has been completed.
            ExchangeHandler?.Invoke(this, _current);
            _current = null;
        }
    }

    public void Subscribe(IFrameProducer frameProducer)
    {
        if (frameProducer == null)
            throw new ArgumentNullException();
            
        Unsubscribe();
            
        _input = frameProducer;
        _input.FrameHandler += OnFrame;
    }

    public void Unsubscribe()
    {
        if (_input == null) return;

        _input.FrameHandler -= OnFrame;
        _input = null;
    }
        
    public EventHandler<IExchange> ExchangeHandler { get; set; }
}