; This is an Inno Setup configuration file
; http://www.jrsoftware.org/isinfo.php

#define ApplicationVersion GetFileVersion('..\Executables\Release\PRISMSeq Uploader.exe')

[CustomMessages]
AppName=PRISMSeq Uploader
[Messages]
; This message is shown when DisableWelcomePage is false
WelcomeLabel2=This will install [name/ver] on your computer.
; Example with multiple lines:
; WelcomeLabel2=Welcome message%n%nAdditional sentence
[Files]

Source: ..\Executables\Release\PRISMSeq Uploader.exe                 ; DestDir: {app}
Source: ..\Executables\Release\PRISMSeq Uploader.exe.config          ; DestDir: {app}
Source: ..\Executables\Release\ExpTreeLib.dll                        ; DestDir: {app}
Source: ..\Executables\Release\ExtractAnnotationFromDescription.dll  ; DestDir: {app}
Source: ..\Executables\Release\FlexibleFileSortUtility.dll           ; DestDir: {app}
Source: ..\Executables\Release\NucleotideTranslator.dll              ; DestDir: {app}
Source: ..\Executables\Release\PRISM.dll                             ; DestDir: {app}
Source: ..\Executables\Release\Protein_Exporter.dll                  ; DestDir: {app}
Source: ..\Executables\Release\Protein_Importer.dll                  ; DestDir: {app}
Source: ..\Executables\Release\Protein_Storage.dll                   ; DestDir: {app}
Source: ..\Executables\Release\Protein_Uploader.dll                  ; DestDir: {app}
Source: ..\Executables\Release\ProteinFileReader.dll                 ; DestDir: {app}
Source: ..\Executables\Release\SequenceInfoCalculator.dll            ; DestDir: {app}
Source: ..\Executables\Release\Skybound.VisualStyles.dll             ; DestDir: {app}
Source: ..\Executables\Release\TableManipulationBase.dll             ; DestDir: {app}
Source: ..\Executables\Release\TranslationTableImport.dll            ; DestDir: {app}
Source: ..\Executables\Release\UIControls.dll                        ; DestDir: {app}
Source: ..\Executables\Release\ValidateFastaFile.dll                 ; DestDir: {app}

Source: ..\Aux_Files\delete_16x.ico                                  ; DestDir: {app}
Source: ..\AppUI_OrfDBHandler\PRISMSeq_Favicon.ico                   ; DestDir: {app}

Source: ..\RevisionHistory.txt               ; DestDir: {app}

[Dirs]
Name: {commonappdata}\PRISMSeq_Uploader; Flags: uninsalwaysuninstall

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
; Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Icons]
Name: {commondesktop}\PRISMSeq Uploader; Filename: {app}\PRISMSeq Uploader.exe; Tasks: desktopicon; Comment: PRISMSeq Uploader
Name: {group}\PRISMSeq Uploader;         Filename: {app}\PRISMSeq Uploader.exe; Comment: PRISMSeq Uploader

[Setup]
AppName=PRISMSeq_Uploader
AppVersion={#ApplicationVersion}
AppID=PRISMSeqUploaderId
AppPublisher=Pacific Northwest National Laboratory
AppPublisherURL=http://omics.pnl.gov/software
AppSupportURL=http://omics.pnl.gov/software
AppUpdatesURL=http://omics.pnl.gov/software
DefaultDirName={pf}\PRISMSeq_Uploader
DefaultGroupName=PAST Toolkit
AppCopyright=© PNNL
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
SetupIconFile=..\AppUI_OrfDBHandler\PRISMSeq_Favicon.ico
UninstallDisplayIcon={app}\delete_16x.ico
ShowTasksTreeLines=true
OutputDir=.\Output
[Registry]
;Root: HKCR; Subkey: MageFile; ValueType: string; ValueName: ; ValueData:Mage File; Flags: uninsdeletekey
;Root: HKCR; Subkey: MageSetting\DefaultIcon; ValueType: string; ValueData: {app}\wand.ico,0; Flags: uninsdeletevalue
[UninstallDelete]
Name: {app}; Type: filesandordirs

