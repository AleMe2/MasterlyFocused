using Focused.Utils;
using System;

namespace Focused.BulletinBoard
{
    public abstract class BooleanBulletinNotice : BulletinNotice<bool>
    {
        private static readonly IDispatcherProxy DispatcherProxy = new DispatcherProxy();

        /// <summary>
        /// Executes the passed action using the passed dispatcher, if
        ///  - the bulletin board does not contain the specific bulletin notice
        ///  - OR the message for the bulletin notice within the board is false
        /// </summary>
        public void ExecuteActionIfBulletinAllowsIt(Action actionToExecute)
        {
            bool bulletinMessage;
            if (Bulletin.Board.TryRetrieveBulletinMessage(this, out bulletinMessage))
            {
                if (bulletinMessage)
                {
                    EventHandler<BulletinNotice> onWaitingIndicatorChanged = null;

                    onWaitingIndicatorChanged = (sender, notice) =>
                    {
                        var booleanBulletinNotice = notice as BooleanBulletinNotice;
                        if (booleanBulletinNotice == null)
                        {
                            return;
                        }

                        if (Bulletin.Board.TryRetrieveBulletinMessage(booleanBulletinNotice, out bulletinMessage))
                        {
                            if (!bulletinMessage)
                            {
                                Bulletin.Board.BulletinUpdated -= onWaitingIndicatorChanged;
                                DispatcherProxy.ExecuteInDispatcherThread(actionToExecute);
                            }
                        }
                    };

                    Bulletin.Board.BulletinUpdated += onWaitingIndicatorChanged;
                    return;
                }
            }

            DispatcherProxy.ExecuteInDispatcherThread(actionToExecute);
        }
    }
}
