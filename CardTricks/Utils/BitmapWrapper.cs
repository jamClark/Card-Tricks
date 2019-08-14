using CardTricks.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CardTricks.Utils
{
    //[Serializable]
    [DataContract]
    public class BitmapWrapper : INotifyPropertyChanged, IDeserializationCallback
    {
        #region Private Methods
        [DataMember]
        private string _FileName = "";
        [DataMember]
        private string ImageDirectory = null;

        [NonSerialized]
        private string _FullPath = "";
        //TODO: this should be a shared asset object
        [NonSerialized]
        private BitmapImage _Image = null;
        #endregion


        #region Public Properties
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public object Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (!_Image.Equals(value))
                {
                    _Image = (BitmapImage)value;
                    NotifyPropertyChanged("Image");
                }
            }
        }

        public string FullPath
        {
            get { return _FullPath; }
            private set
            {
                _FullPath = value;
                if (_Image == null) _Image = new BitmapImage();
                if (_FileName != null && _FileName.Length > 0)
                {
                    if (!File.Exists(_FullPath))
                    {
                        //default image
                    }
                    else
                    {
                        try
                        {
                            MemoryStream ms = new MemoryStream();
                            FileStream stream = new FileStream(_FullPath, FileMode.Open, FileAccess.Read);
                            ms.SetLength(stream.Length);
                            stream.Read(ms.GetBuffer(), 0, (int)stream.Length);

                            ms.Flush();
                            stream.Close();

                            _Image = new BitmapImage();
                            _Image.BeginInit();
                            _Image.StreamSource = ms;
                            _Image.EndInit();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                            _Image = null;//
                        }
                    }
                }


                NotifyPropertyChanged("Image");
                NotifyPropertyChanged("FullName");
            }
        }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (!_FileName.Equals(value))
                {
                    if (value == null) _FileName = "";
                    else _FileName = value;
                    if (_FileName != null && _FileName.Length > 0 && ImageDirectory != null)
                    {
                        FullPath = System.IO.Path.Combine(ImageDirectory, _FileName);
                    }

                    NotifyPropertyChanged("FileName");
                }
            }
        }

        #endregion


        #region Methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BitmapWrapper(string imageDirectory)
        {
            ImageDirectory = imageDirectory;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="target"></param>
        public BitmapWrapper(BitmapWrapper target)
        {
            //this will set the fullpath and load the image.
            target.FileName = FileName;
        }

        void IDeserializationCallback.OnDeserialization(Object sender)
        {
            // After being deserialized, initialize image
            //FullPath = System.IO.Path.Combine(ImageDirectory, _FileName);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            FullPath = System.IO.Path.Combine(ImageDirectory, _FileName);
        }
        #endregion

    }
}
