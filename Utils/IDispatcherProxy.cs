using System;
using System.Threading.Tasks;

namespace Focused.Utils
{
    public interface IDispatcherProxy
    {
        void ExecuteInDispatcherThread(Action action);

        Task ExecuteInDispatcherThreadAsync(Action action);
    }
}
