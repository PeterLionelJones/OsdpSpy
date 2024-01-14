using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Decoders;
using OsdpSpy.Osdp;

namespace OsdpSpy.Annotators;

public class ValidAccessAnnotator : AlertingAnnotator<IExchange>
{
    public ValidAccessAnnotator(IFactory<IAnnotation> factory) 
        : base(factory)
    {
        _list = new List<ValidAccessItem>();
    }

    private readonly List<ValidAccessItem> _list;

    private const int Green = 0x02;

    private const int TempSet = 0x02;
    private const int TempControlCodeOffset = 2;
    private const int TempOnColorOffset = 5;
    private const int TempOffColorOffset = 6;
        
    private const int PermSet = 0x01;
    private const int PermControlCodeOffset = 10;
    private const int PermOnColorOffset = 12;
    private const int PermOffColorOffset = 13;
        
    private const int TenSecondWindow = 10;


    private void OnAccessGranted(IExchange input)
    {
        var tenSecondsAgo = DateTime.UtcNow - TimeSpan.FromSeconds(TenSecondWindow);
            
        var recentCards = _list
            .CardsSince(tenSecondsAgo)
            .ToList();

        if (recentCards.Count == 0) return;

        var timeWindowOfInterest = recentCards.Count > 1 
            ? recentCards[1].Timestamp 
            : tenSecondsAgo;

        var card = recentCards[0];

        var keysBeforeCard = _list
            .KeysSince(timeWindowOfInterest)
            .Until(card.Timestamp)
            .ToKeyArray();

        var keysAfterCard = _list
            .KeysSince(card.Timestamp)
            .ToKeyArray();
            
        var alert = this.CreateOsdpAlert("Valid Access Detected")
            .AppendItem("TriggeredBy", input.Sequence)
            .AppendItem(
                "PreEventWindow", 
                TenSecondWindow, 
                "Seconds");
                
        if (keysBeforeCard.Any())
        {
            alert.AppendItem(
                "KeysBeforeCard", 
                keysBeforeCard.ToKeySummary());
        }

        alert.AppendItem("CardData", card.Payload.ToRawCardString());

        if (keysAfterCard.Any())
        {
            alert.AppendItem(
                "KeysAfterCard", 
                keysAfterCard.ToKeySummary());
        }
            
        alert.AndLogTo(this);

        // The alert Annotation is logged when the ReportState method is called in the base
        // class after all the annotations have been run for a given IExchange.

        _list.Clear();
    }

    public override void Annotate(IExchange input, IAnnotation output)
    {
        try
        {
            if (input.Pd?.Frame == null) return;

            var acu = input.Acu.Payload.Plain;
            if (acu != null && input.Acu.Frame.Command == Command.LED)
            {
                var tempGreen = 
                    acu[TempControlCodeOffset] == TempSet &&
                    (acu[TempOnColorOffset] == Green || acu[TempOffColorOffset] == Green);

                var permGreen = 
                    acu[PermControlCodeOffset] == PermSet &&
                    (acu[PermOnColorOffset] == Green || acu[PermOffColorOffset] == Green);
            
                if (tempGreen || permGreen)
                {
                    OnAccessGranted(input);
                }
            }
                
            var pd = input.Pd.Payload.Plain;
            if (pd == null) return;
            
            switch (input.Pd.Frame.Reply)
            {
                case Reply.RAW:
                    _list.Add(new ValidAccessItem(true, pd));
                    break;
                
                case Reply.KEYPAD:
                    _list.Add(new ValidAccessItem(false, pd));
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}