using System;

namespace GoogleOAuth.Models
{
    public class PicasaItem : DataObject
    {
        private string _id;
        private DateTime _published;
        private DateTime _updated;
        private string _location;
        private int _numPhotos;
        private string _title;
        private string _coverUrl;
        private string _contentUrl;

        public string Etag { get; set; }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnNotifyPropertyChange();
            }
        }

        public DateTime Published
        {
            get { return _published; }
            set
            {
                _published = value;
                OnNotifyPropertyChange();
            }
        }

        public DateTime Updated
        {
            get { return _updated; }
            set
            {
                _updated = value;
                OnNotifyPropertyChange();
            }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnNotifyPropertyChange();
            }
        }

        public int NumPhotos
        {
            get { return _numPhotos; }
            set
            {
                _numPhotos = value;
                OnNotifyPropertyChange();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnNotifyPropertyChange();
            }
        }

        public string CoverUrl
        {
            get { return _coverUrl; }
            set
            {
                _coverUrl = value;
                OnNotifyPropertyChange();
            }
        }

        public string ContentUrl
        {
            get { return _contentUrl; }
            set
            {
                _contentUrl = value;
                OnNotifyPropertyChange();
            }
        }
    }
}
