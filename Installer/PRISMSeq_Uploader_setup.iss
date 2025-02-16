; This is an Inno Setup configuration file
; http://www.jrsoftware.org/isinfo.php

#define ApplicationVersion GetFileVersion('..\Executables\Debug\PRISMSeq_Uploader.exe')

[CustomMessages]
AppName=PRISMSeq Uploader

[Messages]
; This message is shown when DisableWelcomePage is false
WelcomeLabel2=This will install [name/ver] on your computer.
; Example with multiple lines:
; WelcomeLabel2=Welcome message%n%nAdditional sentence

[Files]
Source: ..\Executables\Debug\PRISMSeq_Uploader.exe                                      ; DestDir: {app}
Source: ..\Executables\Debug\PRISMSeq_Uploader.exe.config                               ; DestDir: {app}
Source: ..\Executables\Debug\Azure.Core.dll                                             ; DestDir: {app}
Source: ..\Executables\Debug\Azure.Identity.dll                                         ; DestDir: {app}
Source: ..\Executables\Debug\CsvHelper.dll                                              ; DestDir: {app}
Source: ..\Executables\Debug\FlexibleFileSortUtility.dll                                ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Bcl.AsyncInterfaces.dll                          ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Bcl.HashCode.dll                                 ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Data.SqlClient.dll                               ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Data.SqlClient.SNI.arm64.dll                     ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Data.SqlClient.SNI.x64.dll                       ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Data.SqlClient.SNI.x86.dll                       ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Extensions.DependencyInjection.Abstractions.dll  ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Extensions.Logging.Abstractions.dll              ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Identity.Client.dll                              ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.Identity.Client.Extensions.Msal.dll              ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.IdentityModel.Abstractions.dll                   ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.IdentityModel.JsonWebTokens.dll                  ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.IdentityModel.Logging.dll                        ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.IdentityModel.Protocols.dll                      ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.IdentityModel.Protocols.OpenIdConnect.dll        ; DestDir: {app}
Source: ..\Executables\Debug\Microsoft.IdentityModel.Tokens.dll                         ; DestDir: {app}
Source: ..\Executables\Debug\Npgsql.dll                                                 ; DestDir: {app}
Source: ..\Executables\Debug\OrganismDatabaseHandler.dll                                ; DestDir: {app}
Source: ..\Executables\Debug\PRISM.dll                                                  ; DestDir: {app}
Source: ..\Executables\Debug\PRISMDatabaseUtils.dll                                     ; DestDir: {app}
Source: ..\Executables\Debug\PRISMWin.dll                                               ; DestDir: {app}
Source: ..\Executables\Debug\ProteinFileReader.dll                                      ; DestDir: {app}
Source: ..\Executables\Debug\Raccoom.TreeViewFolderBrowser.DataProviders.dll            ; DestDir: {app}
Source: ..\Executables\Debug\Raccoom.TreeViewFolderBrowser.dll                          ; DestDir: {app}
Source: ..\Executables\Debug\System.Buffers.dll                                         ; DestDir: {app}
Source: ..\Executables\Debug\System.ClientModel.dll                                     ; DestDir: {app}
Source: ..\Executables\Debug\System.Collections.Immutable.dll                           ; DestDir: {app}
Source: ..\Executables\Debug\System.Configuration.ConfigurationManager.dll              ; DestDir: {app}
Source: ..\Executables\Debug\System.Diagnostics.DiagnosticSource.dll                    ; DestDir: {app}
Source: ..\Executables\Debug\System.IdentityModel.Tokens.Jwt.dll                        ; DestDir: {app}
Source: ..\Executables\Debug\System.IO.FileSystem.AccessControl.dll                     ; DestDir: {app}
Source: ..\Executables\Debug\System.Memory.Data.dll                                     ; DestDir: {app}
Source: ..\Executables\Debug\System.Memory.dll                                          ; DestDir: {app}
Source: ..\Executables\Debug\System.Numerics.Vectors.dll                                ; DestDir: {app}
Source: ..\Executables\Debug\System.Runtime.CompilerServices.Unsafe.dll                 ; DestDir: {app}
Source: ..\Executables\Debug\System.Security.AccessControl.dll                          ; DestDir: {app}
Source: ..\Executables\Debug\System.Security.Cryptography.ProtectedData.dll             ; DestDir: {app}
Source: ..\Executables\Debug\System.Security.Permissions.dll                            ; DestDir: {app}
Source: ..\Executables\Debug\System.Security.Principal.Windows.dll                      ; DestDir: {app}
Source: ..\Executables\Debug\System.Text.Encodings.Web.dll                              ; DestDir: {app}
Source: ..\Executables\Debug\System.Text.Json.dll                                       ; DestDir: {app}
Source: ..\Executables\Debug\System.Threading.Channels.dll                              ; DestDir: {app}
Source: ..\Executables\Debug\System.Threading.Tasks.Extensions.dll                      ; DestDir: {app}
Source: ..\Executables\Debug\System.ValueTuple.dll                                      ; DestDir: {app}
Source: ..\Executables\Debug\ValidateFastaFile.dll                                      ; DestDir: {app}

Source: ..\Aux_Files\delete_16x.ico                                  ; DestDir: {app}
Source: ..\PRISMSeq_Uploader\PRISMSeq_Favicon.ico                    ; DestDir: {app}

Source: ..\RevisionHistory.txt                                       ; DestDir: {app}

[Dirs]
Name: {commonappdata}\PRISMSeq_Uploader; Flags: uninsalwaysuninstall

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
; Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Icons]
Name: {commondesktop}\PRISMSeq Uploader; Filename: {app}\PRISMSeq_Uploader.exe; Tasks: desktopicon; Comment: PRISMSeq Uploader
Name: {group}\PRISMSeq Uploader;         Filename: {app}\PRISMSeq_Uploader.exe; Comment: PRISMSeq Uploader

[Setup]
AppName=PRISMSeq_Uploader
AppVersion={#ApplicationVersion}
AppID=PRISMSeqUploaderId
AppPublisher=Pacific Northwest National Laboratory
AppPublisherURL=http://omics.pnl.gov/software
AppSupportURL=http://omics.pnl.gov/software
AppUpdatesURL=http://omics.pnl.gov/software
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\PRISMSeq_Uploader
DefaultGroupName=PAST Toolkit
AppCopyright=� PNNL
;LicenseFile=.\License.rtf
PrivilegesRequired=poweruser
OutputBaseFilename=PRISMSeq_Uploader_Installer
VersionInfoVersion={#ApplicationVersion}
VersionInfoCompany=PNNL
VersionInfoDescription=PRISMSeq Uploader
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

