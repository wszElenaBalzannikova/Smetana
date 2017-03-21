#!/usr/bin/env python
# -*- coding: utf-8 -*-

class TaggedFile(object):
    
    def __init__(self, FileName):
        self.FileName = FileName
        self.TagsList = []

    def AddTag(self, TagName):
        self.TagsList.append(TagName)
        
    def GetName(self):
        return self.FileName;