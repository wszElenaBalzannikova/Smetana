#!/usr/bin/env python
# -*- coding: utf-8 -*-


from Collection import Collection


class ApplicationCore(object):
    def __init__(self):
        self.CollectionsList = []
        self.CurrentCollection = 0

        # Open maindatabase file
        try:
            SmetanaDatabase = open("Smetana.db", "r")
            for CollectionName in SmetanaDatabase:
                self.CollectionsList.append(Collection(CollectionName[:-1]))
                            
        except:
            SmetanaDatabase = open("Smetana.db", "w")

        SmetanaDatabase.close()

    def CreateCollection(self, CollectionName):
        # Create new collection
        self.CurrentCollection = Collection(CollectionName)

        # Put new collection into list
        self.CollectionsList.append(self.CurrentCollection)
        return

    def OpenCollection(self, CollectionName):
        for CurCollection in self.CollectionsList:
            if CurCollection._CollectionName == CollectionName:

                if self.CurrentCollection != 0:
                    self.CurrentCollection.Store()
                    self.CurrentCollection.Dispose()
                self.CurrentCollection = CurCollection
                self.CurrentCollection.Restore()
                return

        self.CreateCollection(CollectionName)
        return

    def GetCollectionsList(self):
        return [x._CollectionName for x in self.CollectionsList]

    def Close(self):
        try:

            self.CurrentCollection.Store()

            SmetanaDatabase = open("Smetana.db", "w")
            for CollectionName in self.CollectionsList:
                SmetanaDatabase.write(CollectionName._CollectionName + "\n")
            SmetanaDatabase.close()
            return True                
        except:
            return False


    def LoadFile(self, FileName):                
        return self.CurrentCollection.LoadFile(FileName)

    def AssignTag(self, FileName, TagName):
        return self.CurrentCollection.AssignTag(FileName, TagName)

    def CreateTag(self, TagName):        
        return self.CurrentCollection.CreateTag(TagName)

    def RenameTag(self, OldTagName, NewTagName):
       return self.CurrentCollection.RenameTag(OldTagName, NewTagName)

    def PrintFiles(self):
         return self.CurrentCollection.PrintFiles()

    def QueryFiles(self, TagName):
         return self.CurrentCollection.QueryFiles(TagName)

    def QueryAllFiles(self):
        return self.CurrentCollection.QueryAllFiles()

    def GetFileTags(self, FileName):
        return self.CurrentCollection.GetFileTags(FileName)

    def GetTagsList(self):
        return self.CurrentCollection.GetTagsList()
    
    def GetSimilarTags(self, TagName):
         return self.CurrentCollection.GetSimilarTags(TagName)

    def RemoveFile(self, FileName):
         return self.CurrentCollection.RemoveFile(FileName)

    # Serialize database
    def Store(self):
        return self.CurrentCollection.Store()

    def Restore(self):
        return self.CurrentCollection.Restore()