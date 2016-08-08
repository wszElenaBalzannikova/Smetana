using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smetana
{
    public class TaggedFile : INotifyPropertyChanged
    {
        private string _FileName = string.Empty;
        private string[] _FileTagsList;

        public TaggedFile()
        { }

        
         public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                RaisePropertyChanged("FileName");
            }

        }

        public TaggedFile(string FileName)
         {
             _FileName = FileName;
         }

        public TaggedFile(string FileName, string[] FileTags)
        {
            _FileName = FileName;
            _FileTagsList = FileTags;
        }

         public ObservableCollection<StringObject> Labels
         {
             get
             {

                 ObservableCollection<StringObject> _Labels = new ObservableCollection<StringObject>();

                 if (_FileTagsList != null)
                 {
                     foreach (string CurrentTag in _FileTagsList)
                     {
                         _Labels.Add(new StringObject { Value = CurrentTag });
                     }
                 }

                 return _Labels;
             }
         }
         public class StringObject
         {
             public string Value { get; set; }
         }

         public event PropertyChangedEventHandler PropertyChanged;
         protected void RaisePropertyChanged(string name)
         {
             if (PropertyChanged != null)
                 PropertyChanged(this, new PropertyChangedEventArgs(name));
         }

    }
}
