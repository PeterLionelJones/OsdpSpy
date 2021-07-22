using System;
using System.IO;

namespace ThirdMillennium.OsdpSpy
{
    public class FrameLogger : IFrameLogger
    {
        private IFrameProducer _input;
        private StreamWriter _output;

        private void OnFrame(object sender, IFrameProduct frame)
        {
            _output ??= new StreamWriter(
                DateTime.UtcNow.ToOsdpCaptureFileName(),
                append: true);

            _output.WriteLine(frame.ToJson());
        }
        
        public void Subscribe(IFrameProducer input)
        {
            _input = input;
            _input.FrameHandler += OnFrame;
        }

        public void Unsubscribe()
        {
            _input.FrameHandler -= OnFrame;
            _input = null;

            _output.Close();
            _output.Dispose();
            _output = null;
        }
    }
}