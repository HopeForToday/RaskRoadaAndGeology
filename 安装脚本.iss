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
AppId={{95D34EA0-3086-405C-978B-3446DCB2BC31}
AppName={#MyAppName}
AppVerName={#MyAppVerName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=C:\Users\Administrator\Desktop\打包文件
OutputBaseFilename=公路预警系统安装包
SetupIconFile=E:\favicon-20161204012616260.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\app.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\CustomControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion

Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\雨量信息.mdb"; DestDir: "{app}\Rources"; Flags: ignoreversion

Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\bin\Debug\Rources\*"; DestDir: "{app}\bin\Debug\Rources"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Images\*"; DestDir: "{app}\Images"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\地图文档\*"; DestDir: "{app}\Resource\地图文档"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\风险评价\*"; DestDir: "{app}\Resource\风险评价"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\公路\*"; DestDir: "{app}\Resource\公路"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\绕行线路\*"; DestDir: "{app}\Resource\绕行线路"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\网络数据集\*"; DestDir: "{app}\Resource\网络数据集"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\BaseData\BaseRasterData\*"; DestDir: "{app}\Rources\BaseData\BaseRasterData"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\BaseData\BaseShapeData\*"; DestDir: "{app}\Rources\BaseData\BaseShapeData"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionAllData\ConditionAllRasterData\*"; DestDir: "{app}\Rources\ConditionAllData\ConditionAllRasterData"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionAllData\ConditionAllShapeData\*"; DestDir: "{app}\Rources\ConditionAllData\ConditionAllShapeData"; Flags: ignoreversion

Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionData\ConditionRasterData\*"; DestDir: "{app}\Rources\ConditionData\ConditionRasterData"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionData\ConditionShapeData\*"; DestDir: "{app}\Rources\ConditionData\ConditionShapeData"; Flags: ignoreversion

Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\Images\*"; DestDir: "{app}\Rources\Images"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\RoadData\CheckedRoad\*"; DestDir: "{app}\Rources\RoadData\CheckedRoad"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\RoadData\RoadRasterData\*"; DestDir: "{app}\Rources\RoadData\RoadRasterData"; Flags: ignoreversion
Source: "E:\项目\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\RoadData\RoadShapeData\*"; DestDir: "{app}\Rources\RoadData\RoadShapeData"; Flags: ignoreversion
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

