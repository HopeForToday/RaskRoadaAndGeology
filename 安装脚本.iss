; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "��·Ԥ��ϵͳ"
#define MyAppVerName "��·Ԥ��ϵͳ 1.0"
#define MyAppPublisher "�ɶ�����ѧ"
#define MyAppURL "http://www.example.com/"
#define MyAppExeName "RoadRaskEvaltionSystem.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{95D34EA0-3086-405C-978B-3446DCB2BC31}
AppName={#MyAppName}
AppVerName={#MyAppVerName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=C:\Users\Administrator\Desktop\����ļ�
OutputBaseFilename=��·Ԥ��ϵͳ��װ��
SetupIconFile=E:\favicon-20161204012616260.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\app.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\CustomControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\RoadRaskEvaltionSystem.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion

Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\������Ϣ.mdb"; DestDir: "{app}\Rources"; Flags: ignoreversion

Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\bin\Debug\Rources\*"; DestDir: "{app}\bin\Debug\Rources"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Images\*"; DestDir: "{app}\Images"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\��ͼ�ĵ�\*"; DestDir: "{app}\Resource\��ͼ�ĵ�"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\��������\*"; DestDir: "{app}\Resource\��������"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\��·\*"; DestDir: "{app}\Resource\��·"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\������·\*"; DestDir: "{app}\Resource\������·"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Resource\�������ݼ�\*"; DestDir: "{app}\Resource\�������ݼ�"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\BaseData\BaseRasterData\*"; DestDir: "{app}\Rources\BaseData\BaseRasterData"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\BaseData\BaseShapeData\*"; DestDir: "{app}\Rources\BaseData\BaseShapeData"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionAllData\ConditionAllRasterData\*"; DestDir: "{app}\Rources\ConditionAllData\ConditionAllRasterData"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionAllData\ConditionAllShapeData\*"; DestDir: "{app}\Rources\ConditionAllData\ConditionAllShapeData"; Flags: ignoreversion

Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionData\ConditionRasterData\*"; DestDir: "{app}\Rources\ConditionData\ConditionRasterData"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\ConditionData\ConditionShapeData\*"; DestDir: "{app}\Rources\ConditionData\ConditionShapeData"; Flags: ignoreversion

Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\Images\*"; DestDir: "{app}\Rources\Images"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\RoadData\CheckedRoad\*"; DestDir: "{app}\Rources\RoadData\CheckedRoad"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\RoadData\RoadRasterData\*"; DestDir: "{app}\Rources\RoadData\RoadRasterData"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Release\Rources\RoadData\RoadShapeData\*"; DestDir: "{app}\Rources\RoadData\RoadShapeData"; Flags: ignoreversion
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

