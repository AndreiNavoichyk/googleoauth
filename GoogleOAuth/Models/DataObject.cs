using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoogleOAuth.Annotations;

namespace GoogleOAuth.Models
{
    public class DataObject : INotifyPropertyChanged
    {   
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnNotifyPropertyChange([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
