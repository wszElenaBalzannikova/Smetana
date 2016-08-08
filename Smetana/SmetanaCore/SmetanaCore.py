from TaggedFile import TaggedFile
from FileTag import FileTag

class ApplicationCore(object):
    def __init__(self):
        self.FilesList = {}
        self.TagsList = {}
        self.FreeFileId = 0
        self.FreeTagId = 0

    #def LoadFolder(self, FolderName):

    def LoadFile(self, FileName):
                
        if FileName not in [self.FilesList[x].FileName for x in self.FilesList]:
            self.FreeFileId += 1
            self.FilesList[self.FreeFileId] = TaggedFile(FileName)
            return True

        return False

    def GetTagId(self, OtherTagName):
        for TagId, TagName in self.TagsList.items():
            if TagName == OtherTagName:
                return TagId;
        return 0

    def AssignTag(self, FileName, TagName):
        result = False

        #Find tag id
        TagId = self.GetTagId(TagName)
        if TagId == 0:
            result = False
        else:
            for x in self.FilesList:
                if self.FilesList[x].FileName == FileName :
                    if TagId not in self.FilesList[x].TagsList :
                         self.FilesList[x].TagsList.append(TagId)
                         result = True
                    break
        return result

    def CreateTag(self, TagName):
        
        if(TagName not in self.TagsList):
            self.FreeTagId += 1
            self.TagsList[self.FreeTagId] = TagName
            return True;
        return False

    def RenameLabel(self, OldTagName, NewTagName):
        if(OldTagName in self.TagsList and NewTagName not in self.TagsList and OldTagName != NewTagName):
            self.TagsList.remove(OldTagName)
            self.TagsList.append(NewTagName)
            return True
        return False

    def PrintFiles(self):
        for x in self.FilesList:
            print(self.FilesList[x].FileName, [self.TagsList[x] for x in self.FilesList[x].TagsList])
        return

    def QueryFiles(self, TagName):
        for i in self.TagsList:
            if self.TagsList[i] == TagName:
                return [self.FilesList[x].FileName for x in self.FilesList if i in self.FilesList[x].TagsList]

    def QueryAllFiles(self):
        return [self.FilesList[x].FileName for x in self.FilesList]

    def GetFileTags(self, FileName):
        for x in self.FilesList:
            if self.FilesList[x].FileName == FileName :
                return [self.TagsList[x] for x in self.FilesList[x].TagsList]
                 #return [self.TagsList[i] for i in self.FilesList[x].TagsList]

    def GetTagsList(self):
        return self.TagsList.values()

