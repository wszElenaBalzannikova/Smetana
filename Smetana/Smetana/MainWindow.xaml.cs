using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Smetana
{
    public class StringObject : INotifyPropertyChanged
    {
        private bool? _TagChecked;

        public string Value { get; set; }
        
        public bool? TagChecked
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

     public class BoolObject : INotifyPropertyChanged
     {
         private bool _TagChecked;
                 
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

		private string _EditedTag;
        private LoadDirectory LoadDirectoryDialog;
        private static MainWindow _Instance;
        private static string[] _AudioExtentions;
        private static string[] _VideoExtentions;
        private static string[] _ImageExtentions;
        private static string[] _DocsExtentions;

		private static List<CollectionFolder> selectedItems;
		private CollectionFolder _CollectionFolder;

        private List<TaggedFile> _FilesList;
        private List<string> _LabelsList;

        private bool _ShowTaggedFiles = true;
        private bool _ShowUntaggedFiles = true;
		static private CollectionFolder PrevSelectedItem;


        public MainWindow()
        {
			this.Closing += MainWindow_Closing;
            
            string SelectedCollectionName = AskCollectionName();

            // Если пользователь не выбрал коллекцию, выход
            if (SelectedCollectionName == null)
            {
                this.Close();
                return;
            }

            // Показать основное окно
            InitializeComponent();

			_CollectionFolder = new CollectionFolder();

            LoadDatabase(SelectedCollectionName);
            SetGropStyle();
			this.DataContext = this;
            _Instance = this;
            LoadDirectoryDialog = new LoadDirectory(LoadDirectoryToCollection);
            LoadDirectoryDialog.IsVisibleChanged += LoadDirectoryDialog_IsVisibleChanged;

			AllowMultiSelection(views);

            _AudioExtentions = Properties.Settings.Default.AudioFiles.Split(';');
            _VideoExtentions = Properties.Settings.Default.VideoFiles.Split(';');
            _ImageExtentions = Properties.Settings.Default.ImageFiles.Split(';');
            _DocsExtentions = Properties.Settings.Default.Documents.Split(';');

			selectedItems = new List<CollectionFolder>();
        }
               
        private void LoadDirectoryDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Разморозиться, когда диалог выбора каталога закроется
            if((bool)(e.NewValue) == false)
                this.IsEnabled = true;
        }

        private void SetGropStyle()
        {
            views.ItemsSource = Components;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(views.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("FolderName");
            view.GroupDescriptions.Add(groupDescription);
        }

		private  void OpenCollection(bool IsStart = false)
		{
            string SelectedCollectionName = AskCollectionName();
            						
			if(SelectedCollectionName != null)
			{
				// Загрузить коллекцию
				LoadDatabase(SelectedCollectionName);
			}
		}

        private string AskCollectionName()
        {
            // Показать диалог выбора коллекции
            CollectionsWindow cv = new CollectionsWindow(Engine.Get().GetCollectionsList());
            cv.ShowDialog();
            return cv.GetCollectionName();
        }

		void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			if (LoadDirectoryDialog != null)
			{
				LoadDirectoryDialog.Close();
			}
			
			Engine.Get().Close();
		}

        private void LoadTagsList()
        {
            _LabelsList = new List<string>();
            _LabelsList.AddRange(Engine.Get().GetTagsList());
            _LabelsList.Sort();
            UpdateTagsList();
        }

        private void UpdateTagsList()
        {
            LabelsCollection.Clear();
            foreach (string CurrentTag in _LabelsList)
            {
                LabelsCollection.Add(new StringObject { Value = CurrentTag });
            }
        }

        private void LoadFilesList()
        {
            _FilesList = new List<TaggedFile>();

            foreach(string CurrentName in Engine.Get().GetFilesList())
            {
                _FilesList.Add(new TaggedFile(CurrentName));
            }
            

           PrintFilesList();
        }

        private void PrintFilesList()
        {
            Components.Clear();
            
            foreach (var CurrentFile in _FilesList)
            {
                string[] FileTags = Engine.Get().GetFileTags(CurrentFile.FullName);
                CurrentFile.Labels = FileTags;
                _CollectionFolder.AddFile(CurrentFile);
            }
            UpdateCollectionTree();
        }

        private void UpdateFilesList()
        {
            if(_FilesList == null)
            {
                return;
            }

            foreach (var CurrentFile in _FilesList)
            {
                string[] FileTags = Engine.Get().GetFileTags(CurrentFile.FullName);

                // Файл без меток
                if (FileTags.Length == 0)
                {
                    if (_ShowUntaggedFiles)
                    {
                        CurrentFile.IsVisible = true;
                    }
                    else
                    {
                        CurrentFile.IsVisible = false;
                    }
                }
                // Файл с метками
                else
                {
                    CurrentFile.Labels = FileTags;

                    if (_ShowTaggedFiles)
                    {
                        CurrentFile.IsVisible = true;
                    }
                    else
                    {
                        CurrentFile.IsVisible = false;
                    }
                }                
            }
            UpdateCollectionTree();
        }

        private ObservableCollection<CollectionFolder> components;
        public ObservableCollection<CollectionFolder> Components
        {
            get
            {
                if (components == null)
                    components = new ObservableCollection<CollectionFolder>();
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
                AddFilesToColelction(ofd.FileNames);
            }
        }

        private void AddFilesToColelction(string[] FilesNames)
        {
            foreach (string FileName in FilesNames)
            {
				AddFileToColelction(FileName);                
            }

            UpdateCollectionTree();
        }

		private void AddFileToColelction(string FileName)
		{
			//добавить файл в базу
			if (Engine.Get().LoadFile(FileName))
			{
				// Если успешно, добавить в gui
                _CollectionFolder.AddFile(new TaggedFile(FileName));                
			}
		}

        private void UpdateCollectionTree()
        {
            //components = new ObservableCollection<CollectionFolder>();
			if(Components.Count == 0)
			{
				components.Add(_CollectionFolder);
			}
			else
			{
                components[0] = _CollectionFolder;			

			}
        }

        private ObservableCollection<StringObject> labelsCollection;
        public ObservableCollection<StringObject> LabelsCollection
        {
            get
            {
                if(labelsCollection == null)
                    labelsCollection = new ObservableCollection<StringObject>();

                return labelsCollection;
            }
        }

        private BoolObject _TagAssigmentActive;
        public BoolObject TagAssigmentActive 
        {
            get
            {
                if (_TagAssigmentActive == null)
                {
                    _TagAssigmentActive = new BoolObject() { TagChecked = false };
                }
                return _TagAssigmentActive;
            }

            set
            {
                _TagAssigmentActive = value;
            }
        }
        
        public void SelectedTag_MouseDown(object sender, MouseButtonEventArgs e)
        {
			ListViewItem SenderItem = sender as ListViewItem;
            if (SenderItem != null)
            {
                DragDrop.DoDragDrop(SenderItem, (SenderItem.Content as StringObject).Value, DragDropEffects.Copy);

                // Если активен режим выбора тегов, осуществить смену значения
				if (null == (SenderItem.Content as StringObject).TagChecked)
				{
					(SenderItem.Content as StringObject).TagChecked = true;
				}
				else
				{
					(SenderItem.Content as StringObject).TagChecked = !(SenderItem.Content as StringObject).TagChecked;
				}
            }
        }
		
        private void views_Drop(object sender, DragEventArgs e)
        {
			TreeViewItem SenderItem = sender as TreeViewItem;
            if (SenderItem != null)
            {
				// DROP "снаружи"
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    AddFilesToColelction(files);
                }
				// DROP из списка тэгов
                else
                {
                    string DroppedText = (string)e.Data.GetData(DataFormats.Text);
					CollectionFolder FileItem = SenderItem.DataContext as CollectionFolder;
					AssignTagToItem(FileItem, DroppedText);
					
					// обойти все выделенные файлы
					foreach (var SelectedItem in selectedItems)
					{
						if (SelectedItem == FileItem)
							continue;

						AssignTagToItem(SelectedItem, DroppedText);
					}                    
                }
            }
        }

		private void AssignTagToItem(CollectionFolder FileItem, string TagName)
		{
			if ((FileItem != null))
			{
				if (FileItem.File != null)
				{
					// Назначить тэг файлу
					if (Engine.Get().AssignTag(FileItem.File.FullName, TagName))
					{
						// Обновить список тэгов для файла
						FileItem.AssignTagsList(Engine.Get().GetFileTags(FileItem.File.FullName));
						UpdateCollectionTree();
					}
				}
				else
				{
					// Назначить тэг директории
					foreach (var CurrFile in FileItem.GetFileItemsList())
					{
						Engine.Get().AssignTag(CurrFile.FullName, TagName);
						CurrFile.AssignTagsList(Engine.Get().GetFileTags(CurrFile.FullName));
					}

					UpdateCollectionTree();
				}
			}
		}
        private void views_DropToList(object sender, DragEventArgs e)
        {
            ListView SenderItem = sender as ListView;
            if (SenderItem != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Note that you can have more than one file.
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    AddFilesToColelction(files);
                }
            }
        }

        private void views_DropEnter(object sender, DragEventArgs e)
        {
// 			TreeViewItem SenderItem = sender as TreeViewItem;
//              if (SenderItem != null)
//              {
//                  SenderItem.IsSelected = true;
//              }
        }

        private void views_DropLeave(object sender, DragEventArgs e)
        {
// 			TreeViewItem SenderItem = sender as TreeViewItem;
//             if (SenderItem != null)
//             {
//                 SenderItem.IsSelected = false;
//             }
        }

        private void views_KeyDown(object sender, KeyEventArgs e)
        {

            switch(e.Key)
            {
                case Key.Enter:
					OpenSelectedFiles();                    
                    break;

				case Key.Delete:
					DeleteSelectedFiles();
					break;
            }
        }

		private void OpenSelectedFiles()
		{
			// Открыть выделенные пункты
			string[] Parameters = new string[selectedItems.Count];
			int index = 0;

			foreach (var SelectedItem in selectedItems)
			{
				TaggedFile SelectedFile = SelectedItem.File;

				if (SelectedFile != null)
				{
					if (File.Exists(SelectedFile.FullName))
					{
						Parameters[index] = SelectedFile.FullName;
						index++;
					}
				}
			}

			CanOpenner.launchFile(Parameters);
		}

		private void DeleteSelectedFiles()
		{
			foreach (var SelectedItem in selectedItems)
			{
				TaggedFile SelectedFile = SelectedItem.File;

				if (SelectedFile != null)
				{
					Engine.Get().RemoveFile(SelectedFile.FullName);
				}
			}
			LoadFilesList();
		}
        		
        public void views_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			TreeViewItem SelectedListItem = sender as TreeViewItem;
            if (SelectedListItem != null)
            {
				CollectionFolder FileItem = SelectedListItem.DataContext as CollectionFolder;
                if((FileItem != null) && (FileItem.File != null))
                {
					if (File.Exists(FileItem.File.FullName))
                    {
						Process.Start(FileItem.File.FullName);
                    }
                    else
                    {
                        MessageBox.Show("Файл не найден!", "Открытие файла", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }                
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
                _LabelsList.Add(NewTag);
                _LabelsList.Sort();
                UpdateTagsList();
            }
            AddLabelPanel.Visibility = System.Windows.Visibility.Hidden;
            TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
        }

		private void RenameTag_Click(object sender, RoutedEventArgs e)
		{
			StringObject OldTagName = (TagsList.SelectedItem as StringObject);
			if (OldTagName != null)
			{
				_EditedTag = OldTagName.Value;
				tbNewLabelName.Text = _EditedTag;
				RenameLabelPanel.Visibility = System.Windows.Visibility.Visible;
				TagsButtonsPanel.Visibility = System.Windows.Visibility.Hidden;
				tbNewLabelName.Focus();
			}
		}

		private void btnCancelRename_Click(object sender, RoutedEventArgs e)
		{
			RenameLabelPanel.Visibility = System.Windows.Visibility.Hidden;
			TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
		}

		private void btnAcceptRename_Click(object sender, RoutedEventArgs e)
		{
			string NewTag = tbNewLabelName.Text;
			if (Engine.Get().RenameTag(_EditedTag, NewTag))
			{
				LoadTagsList();
				LoadFilesList();
			}
			RenameLabelPanel.Visibility = System.Windows.Visibility.Hidden;
			TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
		}

        private void AcceptChanges_Click(object sender, RoutedEventArgs e)
        {
            btnSetTags.IsChecked = false;
			bool fHasChanges = false;

			TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
			AcceptCancelPanel.Visibility = System.Windows.Visibility.Hidden;

			foreach (var SelectedItem in selectedItems)
			{
				TaggedFile SelectedFile = SelectedItem.File;
				if (SelectedFile != null)
				{
					foreach (StringObject CurrentTag in LabelsCollection)
					{
						if ((CurrentTag.TagChecked != null) && (CurrentTag.TagChecked.Value))
						{
							if(Engine.Get().AssignTag(SelectedFile.FullName, CurrentTag.Value))
							{
								// Обновить список тэгов для файла
								SelectedItem.AssignTagsList(Engine.Get().GetFileTags(SelectedFile.FullName));
								fHasChanges = true;
							}
						}
					}
				}
			}

 			if (fHasChanges)
 			{
 				UpdateCollectionTree();
 			}
        }

        private void CancelChanges_Click(object sender, RoutedEventArgs e)
        {
            btnSetTags.IsChecked = false;
			TagsButtonsPanel.Visibility = System.Windows.Visibility.Visible;
			AcceptCancelPanel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnAssignTags_Checked(object sender, RoutedEventArgs e)
        {
			TagsButtonsPanel.Visibility = System.Windows.Visibility.Hidden;
			AcceptCancelPanel.Visibility = System.Windows.Visibility.Visible;

            // Показать имеющиеся теги
			if (selectedItems != null)
			{
				List<string> IntersectedTags = new List<string>();
				List<string> UnintersectedTags = new List<string>();
				if(selectedItems.Count == 1)
				{
					string[] SelectedFileTags = Engine.Get().GetFileTags(selectedItems[0].File.FullName);
					IntersectedTags.AddRange(SelectedFileTags);
				}
				else
				{
					foreach (var SelectedItem in selectedItems)
					{
						TaggedFile SelectedFile = SelectedItem.File;
						if (SelectedFile != null)
						{
							string[] SelectedFileTags = Engine.Get().GetFileTags(SelectedFile.FullName);

							// Список пустой, заполнить его данными первого файла
							if (IntersectedTags.Count == 0)
							{
								IntersectedTags.AddRange(SelectedFileTags);
								UnintersectedTags.AddRange(SelectedFileTags);
							}
							else
							{
								// Создать временный список
								IntersectedTags = new List<string>(SelectedFileTags.Intersect(IntersectedTags));
								UnintersectedTags = new List<string>(SelectedFileTags.Union(UnintersectedTags).Except(SelectedFileTags.Intersect(UnintersectedTags)));
							}
						}
					}
				}

				// Отметить теги, которые есть у всех файлов
				foreach (StringObject ListedTag in LabelsCollection)
				{
					ListedTag.TagChecked = false;
					if (IntersectedTags.Contains(ListedTag.Value))
					{
						ListedTag.TagChecked = true;
					}			
				}

				foreach (StringObject ListedTag in LabelsCollection)
				{
					if (UnintersectedTags.Contains(ListedTag.Value))
					{
						ListedTag.TagChecked = null;
					}
				}
			}

        }

        private void btnOpen_Unchecked(object sender, RoutedEventArgs e)
        {
            TagAssigmentActive.TagChecked = false;
        }

        private void btnChangeCollection_Click(object sender, RoutedEventArgs e)
        {
			OpenCollection();
        }

        private void btnOpenCollection_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenColelctionDialog = new OpenFileDialog();
            OpenColelctionDialog.ShowDialog();
            if(OpenColelctionDialog.FileNames.Length == 1)
            {
				LoadDatabase(OpenColelctionDialog.FileNames[0]);
            }
        }

		private void LoadDatabase(string FileName)
		{
			if (Engine.Get().OpenCollection(FileName))
			{
				LoadFilesList();
				LoadTagsList();
                this.Title = FileName + " - Smetana";
			}
		}
        private void btnSaveCollection_Click(object sender, RoutedEventArgs e)
        {
			Engine.Get().Store();
        }

        private void btnRenameCollection_Click(object sender, RoutedEventArgs e)
        {
            //Engine.Get().Store();
        }

        private void TagSearchBox_KeyUp(object sender, KeyEventArgs e)
		{
            if (TagSearchBox.Text.Length != 0)
            {
                // По мере ввода искать файлы с тегами, разделенными запятой
                string[] TaggedFiles = Engine.Get().QueryFiles(TagSearchBox.Text);
                Components.Clear();

                if (TaggedFiles != null)
                {
                    foreach (var CurrentTaggedFile in _FilesList)
                    {
                        bool Found = false;
                        foreach (string CurrentFileName in TaggedFiles)
                        {
                            // Найти файл в списке                        
                            if (CurrentTaggedFile.FullName == CurrentFileName)
                            {
                                CurrentTaggedFile.IsVisible = true;
                                Found = true;
                            }
                        }

                        // Файл не найден в списке
                        if (!Found)
                        {
                            // Скрыть файл
                            CurrentTaggedFile.IsVisible = false;
                        }
                    }                   
                }
            }
            else
            {
                foreach (var CurrentTaggedFile in _FilesList)
                {
                    CurrentTaggedFile.IsVisible = true;
                }
            }

            UpdateCollectionTree();
        }

		private void Group1_MouseEnter(object sender, MouseEventArgs e)
		{
            if (sender as DockPanel != null)
                (sender as DockPanel).Visibility = System.Windows.Visibility.Hidden;
		}

        private void Gropu1_MouseLeave(object sender, MouseEventArgs e)
        {
            CollectionMenuPanelGeneral.Visibility = System.Windows.Visibility.Visible;
        }

        private void Group2_MouseEnter(object sender, MouseEventArgs e)
        {
            FilesMenuPanel.Visibility = System.Windows.Visibility.Hidden;
        }        

        private void Group2_MouseLeave(object sender, MouseEventArgs e)
        {
            FilesMenuPanel.Visibility = System.Windows.Visibility.Visible;
        }
               
        private void LoadDirectoryButton_Click(object sender, RoutedEventArgs e)
		{			
			LoadDirectoryDialog.Show();

            // Заморозиться, пока пользоватеь не примет решение
            this.IsEnabled = false;
		}

        private void LoadDirectoryToCollection(string SelectedDirectory, string Filters, bool IsRecursive)
        {
            if(SelectedDirectory == "")
            {
                return;
            }
            string[] Extentions = ConvertFileToExtentionsList(LoadDirectoryDialog.Filter);

            if (IsRecursive)
            {
                LoadDirectoryRecursive(SelectedDirectory, Extentions);
            }
            else
            {
                LoadDirectoryFiles(SelectedDirectory, Extentions);
            }   
         

        }

        private void LoadDirectoryFiles(string SelectedDirectory, string[] Extentions)
        {

            DirectoryInfo DirInfo = new DirectoryInfo(SelectedDirectory);
                        
            // Загрузить только выбранный каталог
            foreach (var CurrentFile in DirInfo.GetFiles())
            {
                // Фильтр не задан -> загружать все файлы
                if (Extentions.Length == 0)
                {
                    AddFileToColelction(CurrentFile.FullName);
                }
                else if (Extentions.Contains(Path.GetExtension(CurrentFile.FullName)))
                {
                    AddFileToColelction(CurrentFile.FullName);
                }                    
            }

			UpdateCollectionTree();
        }

        private void LoadDirectoryRecursive(string SelectedDirectory, string[] Extentions)
        {
            LoadDirectoryFiles(SelectedDirectory, Extentions);

            DirectoryInfo DirInfo = new DirectoryInfo(SelectedDirectory);
            foreach(var CurrentSubdirectory in DirInfo.GetDirectories())
            {
                LoadDirectoryRecursive(CurrentSubdirectory.FullName, Extentions);
            }
        }

		private string[] ConvertFileToExtentionsList(string Filter)
		{
			switch(Filter)
			{
				case "\"Аудио\"":
                    return _AudioExtentions;

				case "\"Видео\"":
					return _VideoExtentions;

                case "\"Изображения\"":
                    return _ImageExtentions;

                case "\"Документы\"":
                    return _DocsExtentions;

                case "Все файлы":
                    return new string[0];

					// Пользователь сам задал список расширений
					// Разобрать строку на блоки
				default:
                    return ParseUserExtentions(Filter);
			}
		}

        private string[] ParseUserExtentions(string Filter)
        {
            string[] Extentions = Filter.Split(';');
            List<string> Correctextentions = new List<string>();

            if (Extentions == null || Extentions.Length == 0)
            {
                return new string[] { GetCorrectExtention(Filter) };
            }

            foreach(string CurrentExt in Extentions)
            {
                if ((CurrentExt == " ") && (CurrentExt.Length == 0))
                {
                    continue;
                }

                Correctextentions.Add(GetCorrectExtention(CurrentExt));
            }
            return Correctextentions.ToArray();
        }

        private string GetCorrectExtention(string InputExtention)
        {
            // Оставить строку, начинающуюся с точки
            int LastPointIndex = InputExtention.LastIndexOf('.');

            if(LastPointIndex != -1)
            {
                InputExtention = InputExtention.Substring(LastPointIndex);
            }
            else{
                // Если пользователь не указал точку, добавить её
                InputExtention = '.' + InputExtention;
            }

            return InputExtention;
        }

		private static readonly PropertyInfo IsSelectionChangeActiveProperty
		  = typeof(TreeView).GetProperty
			(
			  "IsSelectionChangeActive",
			  BindingFlags.NonPublic | BindingFlags.Instance
			);

		public static void AllowMultiSelection(TreeView treeView)
		{
			if (IsSelectionChangeActiveProperty == null) return;

			
			treeView.SelectedItemChanged += (a, b) =>
			{
				var treeViewItem = treeView.SelectedItem as CollectionFolder;
				if (treeViewItem == null) return;
				
				// allow multiple selection
				// when control key is pressed
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					// suppress selection change notification
					// select all selected items
					// then restore selection change notifications
					var isSelectionChangeActive =
					  IsSelectionChangeActiveProperty.GetValue(treeView, null);

					IsSelectionChangeActiveProperty.SetValue(treeView, true, null);
					selectedItems.ForEach(item => item.IsSelected = true);

					IsSelectionChangeActiveProperty.SetValue
					(
					  treeView,
					  isSelectionChangeActive,
					  null
					);
				}
				else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					if (PrevSelectedItem == treeViewItem)
					{
						treeViewItem.IsSelected = !treeViewItem.IsSelected;
					}

					bool fSelect = false;
					foreach(var item in treeView.Items)
					{
						if((item == PrevSelectedItem) || (item == treeViewItem))
						{
							fSelect = !fSelect;
						}

						(item as CollectionFolder).IsSelected = fSelect;
					}
				}
				// TODO сделать множественное выделение для shift
				else
				{
					// deselect all selected items except the current one
					selectedItems.ForEach(item => item.IsSelected = (item == treeViewItem));
					selectedItems.Clear();
				}

				if (!selectedItems.Contains(treeViewItem))
				{
					treeViewItem.IsSelected = true;
					selectedItems.Add(treeViewItem);
				}
				else
				{
					// deselect if already selected
					treeViewItem.IsSelected = false;
					selectedItems.Remove(treeViewItem);
				}

				PrevSelectedItem = treeViewItem;
			};

		}

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveFilesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пока еще не готово :)");
        }

        private void MoveFilesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Скоро будет реализовано :Р");
        }

        private void LabelsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LabelsMenuPanelDetails_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteTagButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RenameTagButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearTagName_Click(object sender, RoutedEventArgs e)
        {
            FindTagTextBox.Text = "";
        }

        private void FindTagTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int LastSelectedIndex = TagsList.SelectedIndex;

            string Mask = FindTagTextBox.Text;
            int Index = 0;
            if(Mask.Length == 0)
            {
                TagsList.SelectedIndex = LastSelectedIndex;
                TagsList.ScrollIntoView(TagsList.SelectedItem);
                return;
            }

            // Искать метки в списке по мере набора имени
            foreach(var CurrentTag in LabelsCollection)
            {
                if(CurrentTag.Value.StartsWith(Mask))
                {
                    TagsList.SelectedIndex = Index;
                    TagsList.ScrollIntoView(TagsList.SelectedItem);
                    return;
                }
                Index++;
            }

            TagsList.SelectedIndex = LastSelectedIndex;
            TagsList.ScrollIntoView(TagsList.SelectedItem);
        }

        private void ShowTaggedFilesButton_Checked(object sender, RoutedEventArgs e)
        {
            // Показать файлы с метками
            _ShowTaggedFiles = true;
            UpdateFilesList();
        }

        private void ShowTaggedFilesButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Скрыть файлы с метками
            _ShowTaggedFiles = false;
            UpdateFilesList();
        }

        private void ShowUntaggedFilesButton_Checked(object sender, RoutedEventArgs e)
        {
            // Показывать файлы без меток
            _ShowUntaggedFiles = true;
            UpdateFilesList();
        }

        private void ShowUntaggedFilesButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Скрыть файлы без меток
            _ShowUntaggedFiles = false;
            UpdateFilesList();
        }

        public void FirstMethod(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

		private void views_MouseMove(object sender, MouseEventArgs e)
		{
			if(e.LeftButton == MouseButtonState.Pressed)
			{
				CollectionFolder FileItem = (sender as TreeViewItem).DataContext as CollectionFolder;

				DataObject DropObject = null;
				if (FileItem != null)
				{
					if (FileItem.File != null)
					{
						DropObject = new DataObject(DataFormats.FileDrop, new string[1] { FileItem.File.FullName });
					}
					else
					{
						DropObject = new DataObject(DataFormats.FileDrop, FileItem.GetFilesList().ToArray());
					}

					DragDrop.DoDragDrop(sender as TreeViewItem, DropObject, DragDropEffects.All);
				}				
			}
		}

        private void btnSetTags_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
