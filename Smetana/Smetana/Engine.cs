using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;

namespace Smetana
{
    public class Engine
    {
        static private Engine _instance;

        private ScriptEngine engine;
        private ScriptScope scope;
        private dynamic appCore;

        public static Engine Get()
        {
            if(_instance == null)
                _instance = new Engine();

            return _instance;
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

        public bool AssignTag(string FileName, string TagName)
        {
            return appCore.AssignTag(FileName, TagName);
        }

        public bool RenameLabel(string OldTagName, string NewTagName)
        {
            return appCore.RenameLabel(OldTagName, NewTagName);
        }

        private Engine()
        {
            this.engine = Python.CreateEngine();
            this.engine.SetSearchPaths(new[] { @"C:\Users\Elena\Documents\Visual Studio 2013\Projects\Smetana\SmetanaCore" });
            this.scope = this.engine.CreateScope();

            this.scope.ImportModule("SmetanaCore");

            // appCore = ApplicationCore()
            this.engine.Execute("appCore = SmetanaCore.ApplicationCore()", this.scope);
            appCore = this.scope.GetVariable("appCore");

                        appCore.LoadFile("File1.ogg");
            appCore.PrintFiles();

            appCore.CreateTag("Rock");
            appCore.CreateTag("Jazz");
            appCore.CreateTag("Metal");

            appCore.LoadFile("File1.mp3");

            appCore.LoadFile("File2.mp3");
            appCore.LoadFile("File3.mp3");
            appCore.LoadFile("File1.mp3");

            appCore.AssignTag("File1.mp3", "Rock");
            appCore.AssignTag("File1.mp3", "Jazz");
            appCore.AssignTag("File1.mp3", "Metal");

            appCore.AssignTag("File2.mp3", "Rock");
            appCore.AssignTag("File2.mp3", "Jazz");
            appCore.AssignTag("File3.mp3", "Metal");

            IronPython.Runtime.List q = appCore.QueryFiles("Jazz");
            string[] AAA = q.Cast<string>().ToArray();

            q = appCore.GetTagsList();
            AAA = q.Cast<string>().ToArray();
            

        }
             
    }
}
