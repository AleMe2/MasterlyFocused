namespace Focused.BulletinBoard
{
    public class BusyIndicatorNotice : BooleanBulletinNotice
    {
        public override string Notice
        {
            get
            {
                return "IsBusyIndicatorShown";
            }
        }
    }
}
