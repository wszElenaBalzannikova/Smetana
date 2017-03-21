#!/usr/bin/env python
# -*- coding: utf-8 -*-

import binascii
from TaggedFile import TaggedFile
from FileTag import FileTag

class Collection:
    """description of class"""

    def __init__(self, CollectionName):        
        self.FileBuckets = {}
        self.TagsList = {}
        self.FreeFileId = 0
        self.FreeTagId = 0
        self._CollectionName = CollectionName        

    def GetCollectionName(self):
        return self._CollectionName


    def LoadFile(self, FileName):
        crc = binascii.crc32(FileName.encode('utf-8'))
        if crc not in self.FileBuckets.keys():
            self.FileBuckets[crc] = {}

        FilesList = self.FileBuckets[crc]
        if FileName not in [FilesList[x].FileName for x in FilesList]:
            self.FreeFileId += 1
            FilesList[self.FreeFileId] = TaggedFile(FileName)
            return True

        return False

    def AddFile(self, FileId, FileName):
        crc = binascii.crc32(FileName.encode('utf-8'))
        if crc not in self.FileBuckets.keys():
            self.FileBuckets[crc] = {}

        FilesList = self.FileBuckets[crc]
        if FileName not in [FilesList[x].FileName for x in FilesList]:
            FilesList[FileId] = TaggedFile(FileName)
    
    def RemoveFile(self, FileName):
        crc = binascii.crc32(FileName.encode('utf-8'))
        if crc not in self.FileBuckets.keys():
            return False

        FilesList = self.FileBuckets[crc]
        for x in FilesList:
            if FileName == FilesList[x].FileName:
                del FilesList[x]
                return True
        return False

    def GetTagId(self, OtherTagName):
        for TagId, TagName in self.TagsList.items():
            if TagName == OtherTagName:
                return TagId;
        return 0

    def GetSimilarTags(self, TagName):
        return [x for x in self.TagsList if (self.TagsList[x].lower().find(TagName.lower()) != -1) ]

    def AssignTag(self, FileName, TagName):
        result = False

        #Find tag id
        TagId = self.GetTagId(TagName)
        if TagId == 0:
            return False
        else:
            crc = binascii.crc32(FileName.encode('utf-8'))
            if crc not in self.FileBuckets.keys():
                return False

            FilesList = self.FileBuckets[crc]
            for x in FilesList:
                if FilesList[x].FileName == FileName :
                    if TagId not in FilesList[x].TagsList :
                         FilesList[x].TagsList.append(TagId)
                         FilesList[x].TagsList.sort()
                         return True
                         
                    break
        return False

    def CreateTag(self, TagName):
        
        if(TagName not in self.TagsList):
            self.FreeTagId += 1
            self.TagsList[self.FreeTagId] = TagName
            return True;
        return False

    def RenameTag(self, OldTagName, NewTagName):
       TagId = self.GetTagId(OldTagName)
       if TagId == 0:
            return False
       self.TagsList[TagId] = NewTagName
       return True

    def RemoveTag(self, TagName):
       TagId = self.GetTagId(TagName)
       if TagId == 0:
            return False
       for Bucket in self.FileBuckets:
            for x in self.FileBuckets[Bucket]:
                if TagId in self.FileBuckets[Bucket][x].TagsList:
                    self.FileBuckets[Bucket][x].TagsList.remove(TagId)
       
       self.TagsList.pop(TagId)
       return True

    def PrintFiles(self):
        for Bucket in self.FileBuckets:
            for x in self.FileBuckets[Bucket]:
                print(self.FileBuckets[Bucket][x].FileName, [self.TagsList[x] for x in self.FileBuckets[Bucket][x].TagsList])
       

    def QueryFiles(self, TagName):
        TagIds = self.GetSimilarTags(TagName)
        FileNames = []
        for i in TagIds:
            for BKey, Bucket in self.FileBuckets.items():
                FileNames.extend([Bucket[x].FileName for x in Bucket if i in Bucket[x].TagsList])
        return FileNames

    def SortByAlphabet(self, Str):
        return Str.lower()

    def QueryAllFiles(self):
        FileNames = []
        for BKey, Bucket in self.FileBuckets.items():
            FileNames.extend([Bucket[x].FileName for x in Bucket])
        FileNames.sort(key = self.SortByAlphabet)
        return FileNames

    def GetFileTags(self, FileName):
        crc = binascii.crc32(FileName.encode('utf-8'))
        if crc not in self.FileBuckets.keys():
            return False

        Bucket = self.FileBuckets[crc]
        for x in Bucket:
            if Bucket[x].FileName == FileName :
                return [self.TagsList[x] for x in Bucket[x].TagsList]

    def GetTagsList(self):
        return self.TagsList.values()
    
    # Serialize database
    def Store(self):
        #try:
        # Create FileID -- FileName 
        FileNames = open(self._CollectionName + ".db", 'w')
        FileNames.write("FILES\n");

        for BKey, Bucket in self.FileBuckets.items():
            for i in Bucket:
                FileNames.write(str(i) + " " + Bucket[i].FileName.encode('utf-8') + "\n")
        
        
        FileNames.write("TAGS\n");
        # Create TagId -- TagName
        for i in self.TagsList:
            FileNames.write(str(i) + " " + self.TagsList[i].encode('utf-8') + "\n")
        
        FileNames.write("LINK\n");
        # Create FileId -- TagId
        for BKey, Bucket in self.FileBuckets.items():
            for i in Bucket:
                for x in Bucket[i].TagsList:
                    FileNames.write(str(i) + " " + str(x) + "\n")
        #except:
        #   FileNames.close()
        #    return

        FileNames.close()
        return

    def Restore(self):

        # Open FileID -- FileName 
        try:
            FileNames = open(self._CollectionName + ".db", 'r')
        
            state = ""
            for CurrLine in FileNames:
                if "FILES" in CurrLine:
                    state = "FILES"
                elif "TAGS" in CurrLine:
                    state = "TAGS"
                elif "LINK" in CurrLine:
                    state = "LINKS"
                else :
                    if state == "FILES":
                        row = CurrLine.split()
                        FileId = int(row[0])
                        TaggedFileName = CurrLine[len(row[0]) + 1:-1].decode('utf-8')
                        self.AddFile(FileId, TaggedFileName)
                        #self.FilesList[FileId] = TaggedFile(TaggedFileName)
                        if FileId > self.FreeFileId:
                            self.FreeFileId = FileId

                    elif state == "TAGS":
                        row = CurrLine.split()
                        TagId = int(row[0])
                        self.TagsList[TagId] = CurrLine[len(row[0]) + 1:-1].decode('utf-8')
                        if TagId > self.FreeTagId:
                            self.FreeTagId = TagId

                    elif state == "LINKS":
                        row = CurrLine.split()
                        FileTag = int(row[0])
                        for BKey, Bucket in self.FileBuckets.items():
                            if FileTag in Bucket.keys():
                                Bucket[FileTag].TagsList.append(int(row[1]));
        except:
            FileNames = open(self._CollectionName + ".db", 'w')

        FileNames.close()

    def Dispose(self):
        self.FileBuckets.clear()
        self.TagsList.clear()