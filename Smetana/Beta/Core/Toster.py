#!/usr/bin/env python
# -*- coding: utf-8 -*-

import binascii

from  SmetanaCore import *

if __name__ == "__main__":

    FileName = 'hello world'
    crc = binascii.crc32(FileName.encode('utf-8'))

    appCore = ApplicationCore()
    appCore.CreateCollection("Collection")
  
    appCore.LoadFile("D:\\downloads\\Grizzly Knows No Remorse - дискография\\2009 Flashback 'N' Hangover (Demo)\\01 Walking Dead");
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

    print(appCore.QueryFiles("2"))
    print(appCore.QueryFiles("Tag3"))
    
    appCore.Store()

    #print(appCore.GetCollectionsList())

    #print(appCore.GetSimilarTags("1"))
    appCore.Close();
    
    appCore.OpenCollection("Collection")
    appCore.Restore()
    appCore.PrintFiles();

    appCore.LoadFile("File4.mp3");
    appCore.LoadFile("File5.mp3");
    appCore.LoadFile("File6.mp3");

    appCore.CreateTag("Special");

    appCore.AssignTag("File3.mp3", "Tag1")
    appCore.AssignTag("File4.mp3", "Tag3")
    appCore.AssignTag("File5.mp3", "Tag2")
    appCore.AssignTag("File6.mp3", "Special")

    appCore.PrintFiles();
   

