using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Focused
{
    public class MainWindowVm : INotifyPropertyChanged
    {
        private string lorem;

        private string ipsum;

        private string dolor;

        private string sit;

        private string amet;

        private int selectedRow = -1;

        private string rowToFocus;

        private bool isSecondRowFocused = true;

        public MainWindowVm()
        {
            this.SelecteableRows = new List<int> { 0, 1, 2, 3, 4 };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Lorem
        {
            get
            {
                return this.lorem;
            }
            set
            {
                if (this.lorem == value)
                {
                    return;
                }

                this.lorem = value;
                this.RaisePropertyChanged();
            }
        }

        public string Ipsum
        {
            get
            {
                return this.ipsum;
            }
            set
            {
                if (this.ipsum == value)
                {
                    return;
                }

                this.ipsum = value;
                this.RaisePropertyChanged();
            }
        }

        public string Dolor
        {
            get
            {
                return this.dolor;
            }
            set
            {
                if (this.dolor == value)
                {
                    return;
                }

                this.dolor = value;
                this.RaisePropertyChanged();
            }
        }

        public string Sit
        {
            get
            {
                return this.sit;
            }
            set
            {
                if (this.sit == value)
                {
                    return;
                }

                this.sit = value;
                this.RaisePropertyChanged();
            }
        }

        public string Amet
        {
            get
            {
                return this.amet;
            }
            set
            {
                if (this.amet == value)
                {
                    return;
                }

                this.amet = value;
                this.RaisePropertyChanged();
            }
        }

        public int SelectedRow
        {
            get
            {
                return this.selectedRow;
            }
            set
            {
                if (this.selectedRow == value)
                {
                    return;
                }

                this.selectedRow = value;
                this.SetRowToFocus();
                this.RaisePropertyChanged();
            }
        }

        public string RowToFocus
        {
            get
            {
                return this.rowToFocus;
            }
            set
            {
                if (this.rowToFocus == value)
                {
                    return;
                }

                this.rowToFocus = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsSecondRowFocused
        {
            get
            {
                return isSecondRowFocused;
            }
            set
            {
                if (this.isSecondRowFocused == value)
                {
                    return;
                }

                this.isSecondRowFocused = value;
                this.RaisePropertyChanged();
            }
        }


        public IList<int> SelecteableRows { get; private set; }

        private void SetRowToFocus()
        {
            switch (selectedRow)
            {
                case 0:
                    {
                        this.RowToFocus = "Lorem";
                        break;
                    }
                case 1:
                    {
                        this.RowToFocus = "Ipsum";
                        break;
                    }
                case 2:
                    {
                        this.RowToFocus = "Dolor";
                        break;
                    }
                case 3:
                    {
                        this.RowToFocus = "Sit";
                        break;
                    }
                case 4:
                    {
                        this.RowToFocus = "Amet";
                        break;
                    }
            }
        }

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)

            {
                propChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
