using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace OsdpSpy.Listen
{
    public class ThreadService : IThreadService
    {
        private CancellationToken _token;

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
                        _token.ThrowIfCancellationRequested();
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
            _token = token;
            Task.Run(async () => await ServiceThreadAsync(), _token);
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
            await Task.Delay(1, _token);
        }
        
        protected virtual void OnStop() {}
    }
}