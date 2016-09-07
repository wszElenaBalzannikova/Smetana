from  SmetanaCore import *

if __name__ == "__main__":

    appCore = ApplicationCore()
    """
    appCore.LoadFile("File1.mp3");
    appCore.LoadFile("File2.mp3");
    appCore.LoadFile("File3.mp3");
    appCore.LoadFile("File1.mp3");

    appCore.CreateTag("Tag1");
    appCore.CreateTag("Tag2");
    appCore.CreateTag("Tag3");

    appCore.AssignTag("File1.mp3", "Tag1")
    appCore.AssignTag("File1.mp3", "Tag1")
    appCore.AssignTag("File1.mp3", "Tag3")
    appCore.AssignTag("File1.mp3", "Tag2")

    appCore.AssignTag("File2.mp3", "Tag1")
    appCore.AssignTag("File2.mp3", "Tag2")
    appCore.AssignTag("File3.mp3", "Tag3")

    appCore.PrintFiles();

    print(appCore.QueryFiles("Tag2"))
    print(appCore.QueryFiles("Tag3"))
    
    appCore.Store("File1.db")
    """
    appCore.Restore("File1.db")