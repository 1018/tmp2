Option Explicit

dim ConfigName, devenvPath
'ソリューション構成
ConfigName = "release"
'devenvのパス
devenvPath = """C:\Program Files\" + _
    "Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"""

If WScript.Arguments.Length = 0 then
    Return
End If

dim WshShell, fso, objArgs, dic
Set WshShell = WScript.CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
Set dic = CreateObject("Scripting.Dictionary")

'引数取得
set objArgs = WScript.Arguments

GetAllSolutionFiles objArgs(0)

dim res, f
for each f in dic.Items
    'res = MsgBox(f, vbYesNoCancel, "buildしますか？")
     res = vbYes 
    if res = vbYes then
        WshShell.Run devenvPath + " /build " + ConfigName + _
            " """ + f + """",0,true
    elseif res = vbCancel then
        exit for
    end if
next
MsgBox CStr(dic.Count) + "個のslnファイルがありました"

sub GetAllSolutionFiles(strFolder)
    dim str, s
    
    if fso.FolderExists(strFolder) = false then exit sub
    
    GetSolutionFiles strFolder
    
    'サブフォルダで実行
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
    
    'フォルダ内のすべてのファイル
    set folder = fso.GetFolder(strFolder)
    set files = folder.Files
    
    for each f in files
        if StrComp(fso.GetExtensionName(f),"sln",vbTextCompare)=0 then
            dic.Add f, f
        end if
    next
end sub

