using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Focused.Utils
{
    public class DispatcherProxy : IDispatcherProxy
    {
        private bool IsCurrentlyInDispatcherThread()
        {
            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher != null)
            {
                return dispatcher == Application.Current.Dispatcher;
            }

            return false;
        }

        public void ExecuteInDispatcherThread(Action action)
        {
            if (this.IsCurrentlyInDispatcherThread())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        public async Task ExecuteInDispatcherThreadAsync(Action action)
        {
            if (this.IsCurrentlyInDispatcherThread())
            {
                action();
            }
            else
            {
                await Application.Current.Dispatcher.InvokeAsync(action);
            }
        }
    }
}
