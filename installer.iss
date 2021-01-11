[Setup]
AppName=Console Chrome Speech Proxy
AppVerName=Console Chrome Speech Proxy 1.3
AppPublisher=THEY LOVE GAMES
AppPublisherURL=https://theylovegames.com
AppSupportURL=https://theylovegames.com
AppUpdatesURL=https://theylovegames.com
DefaultDirName={pf32}\TheyLoveGames\ConsoleChromeSpeechProxy
DefaultGroupName=TheyLoveGames\ConsoleChromeSpeechProxy
OutputBaseFilename=ConsoleChromeSpeechProxySetup
SetupIconFile=release_icon.ico
UninstallDisplayIcon=release_icon.ico
Compression=lzma
SolidCompression=yes
InfoBeforeFile=eula.txt
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\Release\*"; DestDir: "{pf32}\TheyLoveGames\ConsoleChromeSpeechProxy"; Excludes: "*.pdb"; CopyMode: alwaysoverwrite

[Icons]
Name: "{group}\Console Chrome Speech Proxy"; Filename: "{pf32}\TheyLoveGames\ConsoleChromeSpeechProxy\ConsoleChromeSpeechProxy.exe"; WorkingDir: "{app}";
Name: "{commondesktop}\Console Chrome Speech Proxy"; Filename: "{pf32}\TheyLoveGames\ConsoleChromeSpeechProxy\ConsoleChromeSpeechProxy.exe"; WorkingDir: "{app}";
Name: "{group}\Uninstall Console Chrome Speech Proxy"; Filename: "{uninstallexe}"

[Run]
Filename: "{sys}\cmd.exe"; Description: "Download Chrome Browser"; Flags: postinstall skipifsilent; Parameters: "/c start """" ""https://google.com/chrome"""; WorkingDir: "{app}"
Filename: "{pf32}\TheyLoveGames\ConsoleChromeSpeechProxy\ConsoleChromeSpeechProxy.exe"; Description: "Launch Console Chrome Speech Proxy"; Flags: postinstall skipifsilent runascurrentuser; WorkingDir: "{app}"
