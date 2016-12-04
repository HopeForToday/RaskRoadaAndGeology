; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "公路预警系统"
#define MyAppVerName "公路预警系统 1.0"
#define MyAppPublisher "成都理工大学"
#define MyAppURL "http://www.example.com/"
#define MyAppExeName "RoadRaskEvaltionSystem.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{70BAB8E4-D50E-4C17-B66D-FE147CA43B36}
AppName={#MyAppName}
AppVerName={#MyAppVerName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=C:\Users\Administrator\Desktop
OutputBaseFilename=公路预警安装包
SetupIconFile=E:\预警发布.png
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\CustomControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

