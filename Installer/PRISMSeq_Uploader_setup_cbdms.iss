; This is an Inno Setup configuration file
; http://www.jrsoftware.org/isinfo.php

#define ApplicationVersion GetFileVersion('..\Executables\Debug_CBDMS_GUI\PRISMSeq_Uploader.exe')

[CustomMessages]
AppName=PRISMSeq Uploader

[Messages]
; This message is shown when DisableWelcomePage is false
WelcomeLabel2=This will install [name/ver] on your computer. This version is customized to work with CBDMS.
; Example with multiple lines:
; WelcomeLabel2=Welcome message%n%nAdditional sentence

[Files]
Source: ..\Executables\Debug_CBDMS_GUI\PRISMSeq_Uploader.exe                                      ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\PRISMSeq_Uploader.exe.config                               ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Azure.Core.dll                                             ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Azure.Identity.dll                                         ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\CsvHelper.dll                                              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\FlexibleFileSortUtility.dll                                ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Bcl.AsyncInterfaces.dll                          ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Bcl.HashCode.dll                                 ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Data.SqlClient.dll                               ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Data.SqlClient.SNI.arm64.dll                     ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Data.SqlClient.SNI.x64.dll                       ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Data.SqlClient.SNI.x86.dll                       ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Extensions.DependencyInjection.Abstractions.dll  ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Extensions.Logging.Abstractions.dll              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Identity.Client.dll                              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.Identity.Client.Extensions.Msal.dll              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.IdentityModel.Abstractions.dll                   ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.IdentityModel.JsonWebTokens.dll                  ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.IdentityModel.Logging.dll                        ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.IdentityModel.Protocols.dll                      ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.IdentityModel.Protocols.OpenIdConnect.dll        ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Microsoft.IdentityModel.Tokens.dll                         ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Npgsql.dll                                                 ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\OrganismDatabaseHandler.dll                                ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\PRISM.dll                                                  ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\PRISMDatabaseUtils.dll                                     ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\PRISMWin.dll                                               ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\ProteinFileReader.dll                                      ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Raccoom.TreeViewFolderBrowser.DataProviders.dll            ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\Raccoom.TreeViewFolderBrowser.dll                          ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Buffers.dll                                         ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.ClientModel.dll                                     ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Collections.Immutable.dll                           ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Configuration.ConfigurationManager.dll              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Diagnostics.DiagnosticSource.dll                    ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.IdentityModel.Tokens.Jwt.dll                        ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.IO.FileSystem.AccessControl.dll                     ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Memory.Data.dll                                     ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Memory.dll                                          ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Numerics.Vectors.dll                                ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Runtime.CompilerServices.Unsafe.dll                 ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Security.AccessControl.dll                          ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Security.Cryptography.ProtectedData.dll             ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Security.Permissions.dll                            ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Security.Principal.Windows.dll                      ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Text.Encodings.Web.dll                              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Text.Json.dll                                       ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Threading.Channels.dll                              ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.Threading.Tasks.Extensions.dll                      ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\System.ValueTuple.dll                                      ; DestDir: {app}
Source: ..\Executables\Debug_CBDMS_GUI\ValidateFastaFile.dll                                      ; DestDir: {app}


Source: ..\Aux_Files\delete_16x.ico                                  ; DestDir: {app}
Source: ..\PRISMSeq_Uploader\PRISMSeq_Favicon.ico                    ; DestDir: {app}

Source: ..\RevisionHistory.txt                                       ; DestDir: {app}

[Dirs]
Name: {commonappdata}\PRISMSeq_Uploader_CBDMS; Flags: uninsalwaysuninstall

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
; Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Icons]
Name: {commondesktop}\PRISMSeq Uploader CBDMS; Filename: {app}\PRISMSeq_Uploader.exe; Tasks: desktopicon; Comment: PRISMSeq Uploader for CBDMS
Name: {group}\PRISMSeq Uploader CBDMS;         Filename: {app}\PRISMSeq_Uploader.exe; Comment: PRISMSeq Uploader for CBDMS

[Setup]
AppName=PRISMSeq_Uploader_for_CBDMS
AppVersion={#ApplicationVersion}
AppID=PRISMSeqUploaderCBDMSId
AppPublisher=Pacific Northwest National Laboratory
AppPublisherURL=http://omics.pnl.gov/software
AppSupportURL=http://omics.pnl.gov/software
AppUpdatesURL=http://omics.pnl.gov/software
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\PRISMSeq_Uploader_CBDMS
DefaultGroupName=PAST Toolkit
AppCopyright=� PNNL
;LicenseFile=.\License.rtf
PrivilegesRequired=poweruser
OutputBaseFilename=PRISMSeq_Uploader_CBDMS_Installer
VersionInfoVersion={#ApplicationVersion}
VersionInfoCompany=PNNL
VersionInfoDescription=PRISMSeq Uploader CBDMS
VersionInfoCopyright=PNNL
DisableWelcomePage=false
DisableFinishedPage=true
ShowLanguageDialog=no
ChangesAssociations=false
InfoAfterFile=.\postinstall.rtf
EnableDirDoesntExistWarning=false
AlwaysShowDirOnReadyPage=true
SetupIconFile=..\PRISMSeq_Uploader\PRISMSeq_Favicon.ico
UninstallDisplayIcon={app}\delete_16x.ico
ShowTasksTreeLines=true
OutputDir=.\Output

[Registry]
;Root: HKCR; Subkey: MageFile; ValueType: string; ValueName: ; ValueData:Mage File; Flags: uninsdeletekey
;Root: HKCR; Subkey: MageSetting\DefaultIcon; ValueType: string; ValueData: {app}\wand.ico,0; Flags: uninsdeletevalue

[UninstallDelete]
Name: {app}; Type: filesandordirs

