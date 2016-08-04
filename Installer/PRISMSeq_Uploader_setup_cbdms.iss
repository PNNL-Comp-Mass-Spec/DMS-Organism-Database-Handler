; This is an Inno Setup configuration file
; http://www.jrsoftware.org/isinfo.php

#define ApplicationVersion GetFileVersion('..\Executables\Release_CBDMS_GUI\PRISMSeq Uploader.exe')

[CustomMessages]
AppName=PRISMSeq Uploader
[Messages]
; This message is shown when DisableWelcomePage is false
WelcomeLabel2=This will install [name/ver] on your computer. This version is customized to work with CBDMS.
; Example with multiple lines:
; WelcomeLabel2=Welcome message%n%nAdditional sentence
[Files]

Source: ..\Executables\Release_CBDMS_GUI\PRISMSeq Uploader.exe                 ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\PRISMSeq Uploader.exe.config          ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\ExpTreeLib.dll                        ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\ExtractAnnotationFromDescription.dll  ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\FlexibleFileSortUtility.dll           ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\NucleotideTranslator.dll              ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\PRISM.dll                             ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\Protein_Exporter.dll                  ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\Protein_Importer.dll                  ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\Protein_Storage.dll                   ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\Protein_Uploader.dll                  ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\ProteinFileReader.dll                 ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\SequenceInfoCalculator.dll            ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\Skybound.VisualStyles.dll             ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\TableManipulationBase.dll             ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\TranslationTableImport.dll            ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\UIControls.dll                        ; DestDir: {app}
Source: ..\Executables\Release_CBDMS_GUI\ValidateFastaFile.dll                 ; DestDir: {app}

Source: ..\Aux_Files\delete_16x.ico                                  ; DestDir: {app}
Source: ..\AppUI_OrfDBHandler\PRISMSeq_Favicon.ico                   ; DestDir: {app}

Source: ..\RevisionHistory.txt               ; DestDir: {app}

[Dirs]
Name: {commonappdata}\PRISMSeq_Uploader_CBDMS; Flags: uninsalwaysuninstall

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
; Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Icons]
Name: {commondesktop}\PRISMSeq Uploader CBDMS; Filename: {app}\PRISMSeq Uploader.exe; Tasks: desktopicon; Comment: PRISMSeq Uploader for CBDMS 
Name: {group}\PRISMSeq Uploader CBDMS;         Filename: {app}\PRISMSeq Uploader.exe; Comment: PRISMSeq Uploader for CBDMS 

[Setup]
AppName=PRISMSeq_Uploader_for_CBDMS
AppVersion={#ApplicationVersion}
AppID=PRISMSeqUploaderCBDMSId
AppPublisher=Pacific Northwest National Laboratory
AppPublisherURL=http://omics.pnl.gov/software
AppSupportURL=http://omics.pnl.gov/software
AppUpdatesURL=http://omics.pnl.gov/software
DefaultDirName={pf}\PRISMSeq_Uploader_CBDMS
DefaultGroupName=PAST Toolkit
AppCopyright=© PNNL
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
SetupIconFile=..\AppUI_OrfDBHandler\PRISMSeq_Favicon.ico
UninstallDisplayIcon={app}\delete_16x.ico
ShowTasksTreeLines=true
OutputDir=.\Output
[Registry]
;Root: HKCR; Subkey: MageFile; ValueType: string; ValueName: ; ValueData:Mage File; Flags: uninsdeletekey
;Root: HKCR; Subkey: MageSetting\DefaultIcon; ValueType: string; ValueData: {app}\wand.ico,0; Flags: uninsdeletevalue
[UninstallDelete]
Name: {app}; Type: filesandordirs

