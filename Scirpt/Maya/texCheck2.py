# -*- coding: utf-8 -*-
"""
Created on Sat Nov  6 14:47:25 2021

@author: yyj
"""
from maya import cmds
import os
#窗口类
class Window(object):
    #构造函数
    def __init__(self, name):
        if cmds.window(name, query=True, exists=True):
            cmds.deleteUI(name)
        cmds.window(name,height=1000)
        self.buildUI()
        cmds.showWindow()

    def buildUI(self):
        print ("no UI")

#具体UI类,继承Window
class TexUI(Window):
    def __init__(self, name='TexUI'):
        super(TexUI, self).__init__(name)
        self.badFilePath = []
        self.info = None
        

    def buildUI(self):
        column = cmds.columnLayout()

        cmds.columnLayout()
        frame = cmds.frameLayout(label="Check Texture Tool",width=200,backgroundColor=[0,0,0])
        #列出所有贴图
        cmds.rowLayout(numberOfColumns=2)
        cmds.button(label=u"列出所有贴图", command = self.onShowTexturesExistClick,width=100)
        cmds.button(label=u"选择错误路径贴图", command = self.onSelectTexturesNodes,width=100)
        cmds.setParent(frame)
        
        cmds.button(label=u"统计贴图数量", command = self.onCountTexturesSumClick,width=200)

        cmds.button(label=u"修改贴图文件路径", command = self.onEditTexturesPathClick,width=200)
        self.newPath = "newPath"
        cmds.textField(self.newPath,tx=u"请先选择一个物体再输入新路径(最后不要\\)")
        #debug用
        #cmds.textField(self.newPath,tx="D:\zark\Documents\maya\scripts")

        cmds.button(label=u"批量修改贴图文件路径", command = self.onBatchEditTexturesPathClick,width=200)
        self.batchNewPath = "batchNewPath"
        cmds.textField(self.batchNewPath,tx=u"请输入新路径(最后不要\\)")
        
        cmds.setParent(column)
        

        #消息文本框
        cmds.frameLayout(label=u"消息",width=200)
        

    def onShowTexturesExistClick(self, *args):
        '''列出MAYA中所有贴图并显示贴图是否存在'''

        textures=cmds.ls(type = "file")

        
        if len(textures)==0:
            cmds.confirmDialog( title=u'消息', message=u'该场景没有贴图', button=[u'确认'], defaultButton='Yes')
            return
        
        self.checkInfoExists()
        
        infoColumn = cmds.columnLayout()
        for tex in textures:
            cmds.text( label=u'节点名' )
            cmds.textField(tx=tex+'\n',width=200,edit=False)
            cmds.text( label=u'路径' )
            fp = cmds.getAttr((tex+".fileTextureName"))
            cmds.textField(tx=fp,width=200,edit=False)
            if not os.path.exists(fp):
                self.badFilePath.append(tex)
                cmds.text(label=u'↑路径不存在',backgroundColor=[1,0,0])


        cmds.setParent(infoColumn)
        self.info = infoColumn
        
        

    def onEditTexturesPathClick(self, *args):
        '''修改贴图文件路径'''
        obj = cmds.ls(selection=True)
        #print (obj)
        if obj == [] or len(obj)>1:
            cmds.confirmDialog( title=u'消息', message=u'请先选择一个物体', button=[u'确认'], defaultButton='Yes')
            return

        path = cmds.textField(self.newPath,q=True,tx=True)
        if not os.path.exists(path):
            cmds.confirmDialog( title=u'消息', message=u'路径不存在', button=[u'确认'], defaultButton='Yes')
            return
        
        shapesInSel = cmds.ls(dag=1,o=1,s=1,sl=1)
        shadingGrps = cmds.listConnections(shapesInSel,type='shadingEngine')
        shaders = cmds.ls(cmds.listConnections(shadingGrps),materials=1)
        fileNode = cmds.listConnections('%s.color' % (shaders[0]), type='file')
        try:
            currentFile = cmds.getAttr("%s.fileTextureName" % fileNode[0])
        except:
            cmds.confirmDialog( title=u'消息', message=u'该物体本身没有贴图\n不能修改路径!', button=[u'确认'], defaultButton='Yes')
            return
        nameList = currentFile.split('/')
        newPath = path + '\\' + nameList[len(nameList)-1]
        if not os.path.exists(newPath):
            cmds.confirmDialog( title=u'消息', message=u'路径不存在贴图文件', button=[u'确认'], defaultButton='Yes')
            return
        cmds.setAttr("%s.fileTextureName" % fileNode[0],newPath,type="string")
        cmds.confirmDialog( title=u'消息', message=u'修改成功', button=[u'确认'], defaultButton='Yes')


    def onSelectTexturesNodes(self, *args):
        '''选择相应的贴图节点'''
        if self.badFilePath == None or len(self.badFilePath)==0:
            cmds.confirmDialog( title=u'消息', message=u'没有路径错误，请重新检查', button=[u'确认'], defaultButton='Yes')
            return
        
        cmds.HypershadeWindow()
        cmds.select(cl=True)
        textures=cmds.ls(type = "file")
        for tex in self.badFilePath:
            cmds.select(tex,add=True,all=True)
        

    def onBatchEditTexturesPathClick(self, *args):
        '''批量修改贴图文件路径'''
        path = cmds.textField(self.batchNewPath,q=True,tx=True)
        if not os.path.exists(path):
            cmds.confirmDialog( title=u'消息', message=u'路径不存在', button=[u'确认'], defaultButton='Yes')
            return

        textures=cmds.ls(type = "file")
        for tex in textures:
            fp = cmds.getAttr((tex+".fileTextureName"))
            nameList = fp.split('/')
            newPath = path + '\\' + nameList[len(nameList)-1]
            cmds.setAttr("%s.fileTextureName" % tex,newPath,type="string")
        cmds.confirmDialog( title=u'消息', message=u'修改成功', button=[u'确认'], defaultButton='Yes')

    def onCountTexturesSumClick(self, *args):
        '''统计贴图数量'''
        textures=cmds.ls(type = "file")
        texNum = len(textures)

        self.checkInfoExists()
        
        infoColumn = cmds.columnLayout()
        cmds.text( label=u'贴图总数：%s'%texNum)
        cmds.setParent(infoColumn)
        self.info = infoColumn

    def checkInfoExists(self,*args):
        '''检查是否已有消息，有的话删除'''
        if self.info != None:
            cmds.deleteUI(self.info)

TexUI('TexUI')
