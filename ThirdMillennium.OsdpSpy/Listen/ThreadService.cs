using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThirdMillennium.OsdpSpy
{
    public class ThreadService : IThreadService
    {
        protected CancellationToken Token { get; private set; }

        private async Task ServiceThreadAsync()
        {
            if (!OnStart()) return;

            try
            {
                // Poll until told otherwise!
                while (true)
                {
                    try
                    {
                        Token.ThrowIfCancellationRequested();
                        await OnServiceAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("ThreadService was cancelled");
            }

            OnStop();
        }

        public bool IsRunning { get; protected set; }

        public void Start(CancellationToken token)
        {
            Token = token;
            Task.Run(async () => await ServiceThreadAsync(), Token);
        }

        protected virtual bool OnStart()
        {
            IsRunning = true;
            return IsRunning;
        }

        protected virtual void OnService() {}

        protected virtual async Task OnServiceAsync()
        {
            OnService();
            await Task.Delay(1, Token);
        }
        
        protected virtual void OnStop() {}
    }
}