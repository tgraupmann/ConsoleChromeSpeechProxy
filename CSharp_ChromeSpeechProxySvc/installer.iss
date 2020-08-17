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

[Files]
Source: "bin\Debug\ChromeSpeechProxySvc.exe"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite
Source: "bin\Debug\ChromeSpeechProxySvc.exe.config"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxySvc"; CopyMode: alwaysoverwrite

[run]
Filename: {sys}\sc.exe; Parameters: "create ChromeSpeechProxySvc start=auto binPath=""{pf32}\TheyLoveGames\ChromeSpeechProxySvc\ChromeSpeechProxySvc.exe""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start ChromeSpeechProxySvc" ; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop ChromeSpeechProxySvc" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete ChromeSpeechProxySvc" ; Flags: runhidden