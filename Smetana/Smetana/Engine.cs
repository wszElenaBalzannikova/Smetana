using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using System.IO;

namespace Smetana
{
    public class Engine
    {
        static private Engine _instance;

        private ScriptEngine engine;
        private ScriptScope scope;
        private dynamic appCore;

		private Engine()
		{
			this.engine = Python.CreateEngine();
            // d:\Smetana\Smetana\SmetanaCore\
			// @"f:\WORK\Smetana\Smetana\SmetanaCore"
            //  Directory.GetCurrentDirectory() + "\\Core"
			this.engine.SetSearchPaths(new[] { Directory.GetCurrentDirectory() + "\\Core" });
			this.scope = this.engine.CreateScope();

			this.scope.ImportModule("SmetanaCore");

			// appCore = ApplicationCore()
			this.engine.Execute("appCore = SmetanaCore.ApplicationCore()", this.scope);
			appCore = this.scope.GetVariable("appCore");
		}

        public static Engine Get()
        {
            if(_instance == null)
                _instance = new Engine();

            return _instance;
        }

		public string[] GetCollectionsList()
		{
			IronPython.Runtime.List CollectionsList = appCore.GetCollectionsList();

			string[] FileNames = new string[CollectionsList.__len__()];
			int i = 0;

			foreach (var CurrKey in CollectionsList)
			{
				FileNames[i] = CurrKey as string;
				i++;
			}

			return FileNames;

		}

        public string[] GetFilesList()
        {
            IronPython.Runtime.List FilesCollection = appCore.QueryAllFiles();

            string []FileNames = new string[FilesCollection.__len__()];
            int i = 0;

            foreach (var CurrKey in FilesCollection)
            {
                FileNames[i] = CurrKey as string;
                i++;
            }

            return FileNames;
        }

        public string[] GetFileTags(string FileName)
        {
            IronPython.Runtime.List TagsCollection = appCore.GetFileTags(FileName);

            string []FileTags = new string[TagsCollection.__len__()];
            int i = 0;

            foreach (var CurrKey in TagsCollection)
            {
                FileTags[i] = CurrKey as string;
                i++;
            }

            return FileTags;

        }

        public string[] GetTagsList()
        {
            IronPython.Runtime.List TagsCollection = appCore.GetTagsList();

            string[] FileTags = new string[TagsCollection.__len__()];
            int i = 0;

            foreach (var CurrKey in TagsCollection)
            {
                FileTags[i] = CurrKey as string;
                i++;
            }

            return FileTags;
        }

        public bool LoadFile(string FileName)
        {
           return appCore.LoadFile(FileName);
        }

        public bool CreateTag(string TagName)
        {
            return appCore.CreateTag(TagName);
        }

		public bool RenameTag(string OldTagName, string NewTagName)
		{
			return appCore.RenameTag(OldTagName, NewTagName);
		}

        public bool AssignTag(string FileName, string TagName)
        {
            return appCore.AssignTag(FileName, TagName);
        }

        public bool RenameLabel(string OldTagName, string NewTagName)
        {
            return appCore.RenameLabel(OldTagName, NewTagName);
        }

       

        public bool Store()
        {
            appCore.Store();
            return true;
        }

        public bool OpenCollection(string ProjectFileName)
        {
			appCore.OpenCollection(ProjectFileName);
            return true;
        }

		public string[] QueryFiles(string TagName)
		{
			IronPython.Runtime.List TagsCollection =  appCore.QueryFiles(TagName);
			if(TagsCollection == null)
			{
				return null;
			}

			string[] FileNames = new string[TagsCollection.__len__()];
			int i = 0;

			foreach (var CurrKey in TagsCollection)
			{
				FileNames[i] = CurrKey as string;
				i++;
			}

			return FileNames;
		}

		public void RemoveFile(string FileName)
		{
			appCore.RemoveFile(FileName);
		}

		public bool Close()
		{
			return appCore.Close();
		}
             
    }
}
