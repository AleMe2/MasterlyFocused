using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Focused.BulletinBoard
{
    public class Bulletin
    {
        private static Bulletin instance;

        private readonly Dictionary<BulletinNotice, object> board;

        private Bulletin()
        {
            this.board = new Dictionary<BulletinNotice, object>();
        }

        public event EventHandler<BulletinNotice> BulletinUpdated;

        public static Bulletin Board
        {
            get
            {
                if (instance == null)
                {
                    instance = new Bulletin();
                }

                return instance;
            }
        }

        public void Update<T>(BulletinNotice<T> notice, T message)
        {
            if (!this.board.ContainsKey(notice))
            {
                this.board.Add(notice, message);
            }
            else
            {
                this.board[notice] = message;
            }

            this.RaiseBulletinUpdated(notice);
        }

        public void Remove<T>(BulletinNotice<T> notice)
        {
            if (this.board.ContainsKey(notice))
            {
                this.board.Remove(notice);
            }

            this.RaiseBulletinUpdated(notice);
        }

        public bool TryRetrieveBulletinMessage<T>(BulletinNotice<T> notice, out T message)
        {
            if (this.board.ContainsKey(notice))
            {
                message = (T)this.board[notice];
                return true;
            }

            message = default(T);
            return false;
        }

        private void RaiseBulletinUpdated<T>(BulletinNotice<T> notice)
        {
            var bulletingUpdated = this.BulletinUpdated;
            if (bulletingUpdated != null)
            {
                bulletingUpdated(this, notice);
            }
        }
    }
}
