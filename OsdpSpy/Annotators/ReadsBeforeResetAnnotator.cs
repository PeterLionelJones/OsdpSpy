using System;
using System.Diagnostics;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Annotators;

public class ReadsBeforeResetAnnotator : AlertingAnnotator<IExchange>
{
    private readonly DateTime _start = DateTime.UtcNow;
    private DateTime _lastReport = DateTime.UtcNow;
    private int _cardsReadSinceLastReset = 0;
    private int _totalCardsRead = 0;
    private int _totalResets = 0;
    private double _averageReadsBeforeReset = 0.0;
        
    public ReadsBeforeResetAnnotator(IFactory<IAnnotation> factory) : base(factory) {}

    private void OnCardRead()
    {
        ++_totalCardsRead;
        ++_cardsReadSinceLastReset;
    }

    private void ReaderResetAlert(string headline)
    {
        var hasReset = _totalResets > 0;
        
        this.CreateOsdpAlert(headline)
            .AppendItem("TotalCardsRead", _totalCardsRead)
            .AppendItem("TotalResets", _totalResets)
            .AppendItem(hasReset, "AverageReadsBeforeReset", _averageReadsBeforeReset)
            .AppendItem(hasReset, "CardsReadSinceLastReset", _cardsReadSinceLastReset)
            .AndLogTo(this);
    }

    private void OnReaderReset()
    {
        ++_totalResets;
        _averageReadsBeforeReset = (double)_totalCardsRead / _totalResets;
        
        ReaderResetAlert("Reader Reset Detected");
            
        _cardsReadSinceLastReset = 0;
    }

    public override void Annotate(IExchange input, IAnnotation output)
    {
        try
        {
            if (input.Pd?.Frame != null)
            {
                switch (input.Pd.Frame.Reply)
                {
                    case Reply.RAW:
                        OnCardRead();
                        break;
                    
                    case Reply.LSTATR:
                        if (input.Pd?.Payload.Plain != null && input.Pd.Payload.Plain[1] == 1)
                        {
                            OnReaderReset();
                        }
                        break;
                }
            }

            if (DateTime.UtcNow - _lastReport > TimeSpan.FromMinutes(1))
            {
                _lastReport = DateTime.UtcNow;
                ReaderResetAlert("Reader Reset Update");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}