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
OutputBaseFilename=��·Ԥ����װ��
SetupIconFile=E:\Ԥ������.png
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\CustomControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\HtmlAgilityPack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\��Ŀ\temp2\RaskRoadaAndGeology\pixChange\bin\Debug\RoadRaskEvaltionSystem.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

