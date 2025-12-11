#define AppName "Viasoft.Licensing.LicenseServer"
#define Publisher "Korp"
#define AppUrl "https://www.korp.com.br/"
#define AppVersion GetVersionNumbersString('.\Viasoft.Licensing.LicenseServer.Host.exe')

[Setup]
AppId={{D5FD72AF-B4B2-4A00-8946-BA72C90CFC45}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#Publisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}
AppUpdatesURL={#AppUrl}
DefaultDirName=C:\Korp\ViasoftKorpServices\Services\LicenseServer
DisableDirPage=no
DefaultGroupName=Viasoft.Licensing.LicensingServer
DisableProgramGroupPage=yes
PrivilegesRequired=admin                                                                                       
PrivilegesRequiredOverridesAllowed=commandline
OutputBaseFilename=Viasoft_Licensing_LicensingServer
Compression=lzma                                                                                                   
SolidCompression=yes
WizardStyle=modern
CloseApplications=force
UsePreviousAppDir=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#SourcePath}Viasoft.Licensing.LicenseServer.Host\bin\Release\net5.0\win-x64\publish\*"; Excludes: "LicenseServerSettings.json"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Run]
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe install {param:serviceName|ViasoftKorpLicenseServer} ""{app}\Viasoft.Licensing.LicenseServer.Host.exe"" confirm"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} AppDirectory ""{app}"""; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} DisplayName {param:serviceName|ViasoftKorpLicenseServer}"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} AppExit Stop"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} Start SERVICE_AUTO_START"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} AppStdout {app}\ViasoftKorpLicenseServer.log"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} AppStderr {app}\ViasoftKorpLicenseServer.log"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe set {param:serviceName|ViasoftKorpLicenseServer} Type SERVICE_INTERACTIVE_PROCESS"; Flags: runhidden
Filename: {sys}\cmd.exe; Parameters: "/C {app}\nssm.exe start {param:serviceName|ViasoftKorpLicenseServer}"; Flags: runhidden

[Code]
function InitializeUninstall: Boolean;
var
  ResultCode: integer;
begin
  ShellExec('', ExpandConstant('{sys}\sc.exe'), ExpandConstant('stop {param:serviceName|ViasoftKorpLicenseServer}'), '', SW_SHOW, ewWaitUntilTerminated, ResultCode)
  ShellExec('', ExpandConstant('{sys}\sc.exe'), ExpandConstant('delete {param:serviceName|ViasoftKorpLicenseServer}'), '', SW_SHOW, ewWaitUntilTerminated, ResultCode)

  Result := True;
end;
