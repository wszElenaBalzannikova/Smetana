using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smetana
{
    class TaggeFilesCollection : ObservableCollection<TaggedFile>
    {
        //ObservableCollection<TaggedFile> sampleData;

        //public ObservableCollection<TaggedFile> FilesCollection
        //{
        //    get
        //    {
        //        sampleData = new ObservableCollection<TaggedFile>();
        //        sampleData.Add(new TaggedFile("sampleData one"));
        //        sampleData.Add(new TaggedFile("sampleData two"));
        //        sampleData.Add(new TaggedFile("sampleData 3"));
        //        sampleData.Add(new TaggedFile("File1232"));

        //        return sampleData;
        //    }

        //    set
        //    {
               
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void RaisePropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(name));
        //}
    }
}
