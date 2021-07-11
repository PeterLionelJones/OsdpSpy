using System;
using System.Collections.Generic;
using System.Linq;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class ValidAccessAnnotator : ExchangeAnnotator
    {
        public ValidAccessAnnotator(IFactory<IAnnotation> factory, IDeferredLogger logger)
        {
            _factory = factory;
            _logger = logger;
            _list = new List<ValidAccessItem>();
        }

        private readonly IFactory<IAnnotation> _factory;
        private readonly IDeferredLogger _logger;
        private readonly List<ValidAccessItem> _list;

        private IAnnotation _annotation;

        private const int Green = 0x02; 
        private const int TempOnColorOffset = 5;
        private const int TempOffColorOffset = 6;
        private const int TenSecondWindow = 10;


        private void OnAccessGranted()
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
            
            _annotation = _factory.Create();

            _annotation.Append("**** VALID ACCESS DETECTED ****");

            _annotation.AppendNewLine();

            _annotation.AppendItem(
                "PreEventWindow", 
                TenSecondWindow, 
                "Seconds");
                
            if (keysBeforeCard.Any())
            {
                _annotation.AppendItem(
                    "KeysBeforeCard", 
                    keysBeforeCard.ToKeySummary());
            }

            _annotation.AppendItem("CardData", card.Payload.ToRawCardString());

            if (keysAfterCard.Any())
            {
                _annotation.AppendItem(
                    "KeysAfterCard", 
                    keysAfterCard.ToKeySummary());
            }
            
            _annotation.AppendNewLine();

            // The Annotation is logged when the ReportState method is called after all the
            // annotations have been run for a given IExchange.

            _list.Clear();
        }

        public override void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Pd.Frame == null) return;

            var acu = input.Acu.Payload.Plain;
            
            if (   acu != null &&
                   input.Acu.Frame.Command == Command.LED && 
                   (acu[TempOnColorOffset] == Green || acu[TempOffColorOffset] == Green))
            {
                OnAccessGranted();
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

        public override void ReportState()
        {
            _annotation?.Log();
            _annotation = null;
        }
    }
}