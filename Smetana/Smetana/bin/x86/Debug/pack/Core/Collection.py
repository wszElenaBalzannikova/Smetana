from TaggedFile import TaggedFile
from FileTag import FileTag

class Collection:
    """description of class"""

    def __init__(self, CollectionName):        
        self.FilesList = {}
        self.TagsList = {}
        self.FreeFileId = 0
        self.FreeTagId = 0
        self._CollectionName = CollectionName        

    def GetCollectionName(self):
        return self._CollectionName


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

    def GetSimilarTags(self, TagName):
        return [x for x in self.TagsList if (self.TagsList[x].lower().find(TagName.lower()) != -1) ]

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

    def RenameTag(self, OldTagName, NewTagName):
       TagId = self.GetTagId(OldTagName)
       if TagId == 0:
            return False
       self.TagsList[TagId] = NewTagName
       return True

    def PrintFiles(self):
        for x in self.FilesList:
            print(self.FilesList[x].FileName, [self.TagsList[x] for x in self.FilesList[x].TagsList])
        return

    def QueryFiles(self, TagName):
        TagIds = self.GetSimilarTags(TagName)
        FileNames = []
        for i in TagIds:
            FileNames.extend([self.FilesList[x].FileName for x in self.FilesList if i in self.FilesList[x].TagsList])
        return FileNames

    def QueryAllFiles(self):
        return [self.FilesList[x].FileName for x in self.FilesList]

    def GetFileTags(self, FileName):
        for x in self.FilesList:
            if self.FilesList[x].FileName == FileName :
                return [self.TagsList[x] for x in self.FilesList[x].TagsList]

    def GetTagsList(self):
        return self.TagsList.values()
    
    # Serialize database
    def Store(self):

        # Create FileID -- FileName 
        FileNames = open(self._CollectionName + ".db", 'w')
        FileNames.write("FILES\n");
        for i in self.FilesList:
            FileNames.write(str(i) + " " + self.FilesList[i].FileName + "\n")
        
        FileNames.write("TAGS\n");
        # Create TagId -- TagName
        for i in self.TagsList:
            FileNames.write(str(i) + " " + self.TagsList[i] + "\n")
        
        FileNames.write("LINK\n");
        # Create FileId -- TagId
        for i in self.FilesList:
            for x in self.FilesList[i].TagsList:
                FileNames.write(str(i) + " " + str(x) + "\n")
        
        FileNames.close()

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
                        self.FilesList[FileId] = TaggedFile(CurrLine[len(row[0]) + 1:-1])
                        if FileId > self.FreeFileId:
                            self.FreeFileId = FileId

                    elif state == "TAGS":
                        row = CurrLine.split()
                        TagId = int(row[0])
                        self.TagsList[TagId] = CurrLine[len(row[0]) + 1:-1]
                        if TagId > self.FreeTagId:
                            self.FreeTagId = TagId

                    elif state == "LINKS":
                        row = CurrLine.split()
                        self.FilesList[int(row[0])].TagsList.append(int(row[1]));
        except:
            FileNames = open(self._CollectionName + ".db", 'w')

        FileNames.close()

    def Dispose(self):
        self.FilesList.clear()
        self.TagsList.clear()