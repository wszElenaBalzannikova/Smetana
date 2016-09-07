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
            return False
        else:
            for x in self.FilesList:
                if self.FilesList[x].FileName == FileName :
                    if TagId not in self.FilesList[x].TagsList :
                         self.FilesList[x].TagsList.append(TagId)
                         self.FilesList[x].TagsList.sort()
                         return True
                         
                    break
        return False

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

    def GetTagsList(self):
        return self.TagsList.values()
    
    # Serialize database
    def Store(self, FileName):

        # Create FileID -- FileName 
        FileNames = open(FileName, 'w')
        FileNames.write("FILES\n");
        for i in self.FilesList:
            FileNames.write(str(i) + " " + self.FilesList[i].FileName + "\n")
        
        FileNames.write("TAGS\n");
        # Create TagId -- TagName
        for i in self.TagsList:
            FileNames.write(str(i) + " " + self.TagsList[i] + "\n")
        
        FileNames.write("LINKS\n");
        # Create FileId -- TagId
        for i in self.FilesList:
            for x in self.FilesList[i].TagsList:
                FileNames.write(str(i) + " " + str(x) + "\n")
        
        FileNames.close()

    def Restore(self, FileName):

        # Open FileID -- FileName 
        FileNames = open(FileName, 'r')
        
        state = ""
        for CurrLine in FileNames:
            if "FILES" in CurrLine:
                state = "FILES"
            elif "TAGS" in CurrLine:
                state = "TAGS"
            elif "LINKS" in CurrLine:
                state = "LINKS"
            else :
                if state == "FILES":
                    row = CurrLine.split()
                    self.FilesList[int(row[0])] = TaggedFile(row[1])
                elif state == "TAGS":
                    row = CurrLine.split()
                    self.TagsList[int(row[0])] = row[1]
                elif state == "LINKS":
                    row = CurrLine.split()
                    self.FilesList[int(row[0])].TagsList.append(int(row[1]));

        FileNames.close()

        
