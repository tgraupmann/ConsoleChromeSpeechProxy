[Setup]
AppName=Chrome Speech Proxy Service
AppVerName=Chrome Speech Proxy Service 1.0
AppPublisher=THEY LOVE GAMES
AppPublisherURL=https://theylovegames.com
AppSupportURL=https://theylovegames.com
AppUpdatesURL=https://theylovegames.com
DefaultDirName={pf32}\TheyLoveGames\ChromeSpeechProxySvc
DefaultGroupName=TheyLoveGames\ChromeSpeechProxySvc
OutputBaseFilename=ChromeSpeechProxySvcSetup
SetupIconFile=release_icon.ico
UninstallDisplayIcon=release_icon.ico
Compression=lzma
SolidCompression=yes
InfoBeforeFile=eula.txt
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[code]
procedure StopService;
var
  ResultCode: Integer;
begin

Exec('cmd.exe', '/c net stop ChromeSpeechProxySvc', 'C:\Windows\System32', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
end;



[Files]
;Source: "bin\Debug\ChromeSpeechProxySvc.exe.config"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite; BeforeInstall: StopService();
;Source: "bin\Debug\ChromeSpeechProxySvc.exe"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
;Source: "bin\Debug\Newtonsoft.Json.dll"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
;Source: "bin\Debug\proxy.html"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
;Source: "bin\Debug\proxy.zip"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
Source: "bin\Release\ChromeSpeechProxySvc.exe.config"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite; BeforeInstall: StopService();
Source: "bin\Release\ChromeSpeechProxySvc.exe"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
Source: "bin\Release\Newtonsoft.Json.dll"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
Source: "bin\Release\Newtonsoft.Json.dll"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
Source: "bin\Release\proxy.html"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
Source: "bin\Release\proxy.zip"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite

[run]
Filename: {sys}\sc.exe; Parameters: "create ChromeSpeechProxySvc start=auto binPath=""{pf32}\TheyLoveGames\ChromeSpeechProxySvc\ChromeSpeechProxySvc.exe""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start ChromeSpeechProxySvc" ; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop ChromeSpeechProxySvc" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete ChromeSpeechProxySvc" ; Flags: runhidden