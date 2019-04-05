using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mobile.Models
{
    public class ViewPageViewModel: INotifyPropertyChanged
    {
        public ConfessLoader Confess { get; set; }

        public ViewPageViewModel()
        {

        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
       new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
