[Setup]
AppName=Chrome Speech Proxy
AppVerName=1.0
AppPublisher=THEY LOVE GAMES
AppPublisherURL=https://theylovegames.com
AppSupportURL=https://theylovegames.com
AppUpdatesURL=https://theylovegames.com
DefaultDirName={pf32}\TheyLoveGames\ChromeSpeechProxy
DefaultGroupName=TheyLoveGames\ChromeSpeechProxy
OutputBaseFilename=ChromeSpeechProxySetup
SetupIconFile=release_icon.ico
UninstallDisplayIcon=release_icon.ico
Compression=lzma
SolidCompression=yes
InfoBeforeFile=eula.txt
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\Release\*"; DestDir: "{pf32}\TheyLoveGames\ChromeSpeechProxy"; Excludes: "*.pdb"; CopyMode: alwaysoverwrite

[Icons]
Name: "{group}\ChromeSpeechProxy"; Filename: "{pf32}\TheyLoveGames\ChromeSpeechProxy\ConsoleChromeSpeechProxy.exe"; WorkingDir: "{app}";
Name: "{commondesktop}\ChromeSpeechProxy"; Filename: "{pf32}\TheyLoveGames\ChromeSpeechProxy\ConsoleChromeSpeechProxy.exe"; WorkingDir: "{app}";
Name: "{group}\Uninstall ChromeSpeechProxy"; Filename: "{uninstallexe}"

[Run]
Filename: "{sys}\cmd.exe"; Description: "Download Chrome Browser"; Flags: postinstall skipifsilent; Parameters: "/c start """" ""https://google.com/chrome"""; WorkingDir: "{app}"
Filename: "{pf32}\TheyLoveGames\ChromeSpeechProxy\ConsoleChromeSpeechProxy.exe"; Description: "Launch Chrome Speech Proxy"; Flags: postinstall skipifsilent runascurrentuser; WorkingDir: "{app}"
