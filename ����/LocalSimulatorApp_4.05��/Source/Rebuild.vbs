Option Explicit

dim ConfigName, devenvPath
'�\�����[�V�����\��
ConfigName = "release"
'devenv�̃p�X
devenvPath = """C:\Program Files\" + _
    "Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"""

If WScript.Arguments.Length = 0 then
    Return
End If

dim WshShell, fso, objArgs, dic
Set WshShell = WScript.CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
Set dic = CreateObject("Scripting.Dictionary")

'�����擾
set objArgs = WScript.Arguments

GetAllSolutionFiles objArgs(0)

dim res, f
for each f in dic.Items
    'res = MsgBox(f, vbYesNoCancel, "build���܂����H")
     res = vbYes 
    if res = vbYes then
        WshShell.Run devenvPath + " /build " + ConfigName + _
            " """ + f + """",0,true
    elseif res = vbCancel then
        exit for
    end if
next
MsgBox CStr(dic.Count) + "��sln�t�@�C��������܂���"

sub GetAllSolutionFiles(strFolder)
    dim str, s
    
    if fso.FolderExists(strFolder) = false then exit sub
    
    GetSolutionFiles strFolder
    
    '�T�u�t�H���_�Ŏ��s
    dim folder, subfolders
    set folder = fso.GetFolder(strFolder)
    set subfolders = folder.SubFolders
    for each s in subfolders
        GetAllSolutionFiles s
    next
end sub

sub GetSolutionFiles(strFolder)
    dim folder, files, f
    
    if fso.FolderExists(strFolder) = false then exit sub
    
    '�t�H���_���̂��ׂẴt�@�C��
    set folder = fso.GetFolder(strFolder)
    set files = folder.Files
    
    for each f in files
        if StrComp(fso.GetExtensionName(f),"sln",vbTextCompare)=0 then
            dic.Add f, f
        end if
    next
end sub

