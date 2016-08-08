using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using System.ComponentModel;

namespace Smetana
{
    public class StringObject : INotifyPropertyChanged
    {
        private bool _TagChecked;

        public string Value { get; set; }

        public bool TagChecked
        {
            get { return _TagChecked; }
            set
                {
                    _TagChecked = value;
                    OnPropertyChanged("TagChecked");
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            LoadFilesList();

            LoadTagsList();
        }

        private void LoadTagsList()
        {
            foreach (string CurrentTag in Engine.Get().GetTagsList())
            {
                LabelsCollection.Add(new StringObject { Value = CurrentTag });
            }
        }

        private void LoadFilesList()
        {
            foreach (string CurrentFileName in Engine.Get().GetFilesList())
            {
                Components.Add(new TaggedFile(CurrentFileName, Engine.Get().GetFileTags(CurrentFileName)));
            }
            
        }

        private ObservableCollection<TaggedFile> components;
        public ObservableCollection<TaggedFile> Components
        {
            get
            {
                if (components == null)
                    components = new ObservableCollection<TaggedFile>();
                return components;
            }
        }

        public void LoadFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.ShowDialog();
           

            if(ofd.FileNames.Length != 0)
            {
                foreach(string FileName in ofd.FileNames)
                {      
                    //Добвить файл в базу
                    if (Engine.Get().LoadFile(FileName))
                    {
                        // Если успешно, добавить в gui
                        Components.Add(new TaggedFile(FileName));
                    }
                }
            }
        }

        public ObservableCollection<StringObject> labelsCollection;
        public ObservableCollection<StringObject> LabelsCollection
        {
            get
            {
                if(labelsCollection == null)
                    labelsCollection = new ObservableCollection<StringObject>();

                return labelsCollection;
            }
        }
        

        public void SelectedTag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem SenderItem = sender as ListViewItem;
            if(SenderItem != null)
            DragDrop.DoDragDrop(SenderItem, (SenderItem.Content as StringObject).Value, DragDropEffects.Copy);
        }

        private void views_Drop(object sender, DragEventArgs e)
        {
            ListViewItem SenderItem = sender as ListViewItem;
            if (SenderItem != null)
            {
                string DroppedText = (string)e.Data.GetData(DataFormats.Text);
                TaggedFile FileItem = SenderItem.Content as TaggedFile;
                if (FileItem != null)
                {
                    if (Engine.Get().AssignTag(FileItem.FileName, DroppedText))
                    {
                        // Обновить список файлов
                        components.Clear();
                        LoadFilesList();
                    }
                }
            }
        }

        private void views_DropEnter(object sender, DragEventArgs e)
        {
             ListViewItem SenderItem = sender as ListViewItem;
             if (SenderItem != null)
             {
                 SenderItem.IsSelected = true;
             }
        }

        private void views_DropLeave(object sender, DragEventArgs e)
        {
            ListViewItem SenderItem = sender as ListViewItem;
            if (SenderItem != null)
            {
                SenderItem.IsSelected = false;
            }
        }

        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            tbNewLabel.Text = "";
            AddLabelPanel.Visibility = System.Windows.Visibility.Visible;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Hidden;
            tbNewLabel.Focus();
        }

        private void btnCancelNewTag_Click(object sender, RoutedEventArgs e)
        {
            AddLabelPanel.Visibility = System.Windows.Visibility.Hidden;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnAcceptNewTag_Click(object sender, RoutedEventArgs e)
        {
            string NewTag = tbNewLabel.Text;
            if(Engine.Get().CreateTag(NewTag))
            {
                LabelsCollection.Add(new StringObject() { Value = NewTag });
            }
            AddLabelPanel.Visibility = System.Windows.Visibility.Hidden;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void AcceptChanges_Click(object sender, RoutedEventArgs e)
        {
            AcceptCancelPanel.Visibility = System.Windows.Visibility.Hidden;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void CancelChanges_Click(object sender, RoutedEventArgs e)
        {
            AcceptCancelPanel.Visibility = System.Windows.Visibility.Hidden;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnOPen_Checked(object sender, RoutedEventArgs e)
        {           
            AcceptCancelPanel.Visibility = System.Windows.Visibility.Visible;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Hidden;

            // Показать имеющиеся теги
            if(views.SelectedItem != null)
            {
                TaggedFile SelectedFile = views.SelectedItem as TaggedFile;
                if(SelectedFile != null)
                {
                    string[] SelectedFileTags = Engine.Get().GetFileTags(SelectedFile.FileName);
                                       
                    foreach (StringObject ListedTag in LabelsCollection)
                    {
                        ListedTag.TagChecked = false;
                        for(int i = 0; i < SelectedFileTags.Length; i++)                   
                        {
                            if(ListedTag.Value == SelectedFileTags[i])
                            {
                                ListedTag.TagChecked = true;
                                break;
                            }
                        }
                        
                    }
                }

            }

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            StringObject ClickedTag = sender as StringObject;
            if(ClickedTag != null)
            {
                ClickedTag.TagChecked = !ClickedTag.TagChecked  ;
            }
        }


    }
}
