using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smetana
{
    public class TaggedFile : INotifyPropertyChanged
    {
        private string _FileName = string.Empty;
        private string[] _FileTagsList;
        private string _FolderName;
		private bool _IsSelected = false;
        private bool _IsVisible = true;

        public TaggedFile()
        { }

		public string FullName
		{
			get { return _FileName; }		

		}

         public string FileName
        {
            get { return Path.GetFileName(_FileName); }
            set
            {
                _FileName = value;
                RaisePropertyChanged("FileName");
            }

        }

        public string FolderName
         {
             get { return _FolderName; }
             set
             {
                 _FolderName = value;
                 RaisePropertyChanged("FolderName");
             }
         }

        public TaggedFile(string FileName)
         {
             _FileName = FileName;
             _FolderName = Path.GetDirectoryName(_FileName);
         }

        public TaggedFile(string FileName, string[] FileTags)
        {
            _FileName = FileName;
            _FileTagsList = FileTags;
            _FolderName = Path.GetDirectoryName(_FileName);
        }

		public void AssignTagsList(string[] FileTags)
		{
			_FileTagsList = FileTags;
		}

		public bool IsSelected
		{
			get { return _IsSelected; }
			set
			{ 
				if(value != _IsSelected)
				{
					_IsSelected = value;
					RaisePropertyChanged("IsSelected");
				}
			}
		}
                
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                _IsVisible = value;
            }
        }

        public string[] Labels
		{
			get
			{
				return _FileTagsList;
			}

            set
            {
                _FileTagsList = value;
            }
		}

         public class StringObject
         {
             public string Value { get; set; }
         }

		public bool IsADirectory
		 {
			 get { return false; }
		 }
        
         public event PropertyChangedEventHandler PropertyChanged;
         protected void RaisePropertyChanged(string name)
         {
             if (PropertyChanged != null)
                 PropertyChanged(this, new PropertyChangedEventArgs(name));
         }

    }
}
