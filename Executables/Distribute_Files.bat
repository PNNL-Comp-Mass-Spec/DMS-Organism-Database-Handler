@echo off

set TargetBase=\\proto-2\past\Software\PrismSeqUploader
set Iteration=1

:Loop
echo Copying to %TargetBase%
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\Executables\Debug\*" %TargetBase% /d /y
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\Bulk_Fasta_Importer\bin\Debug\*" %TargetBase%\Bulk_Fasta_Importer /d /y
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\FastaFileMaker_Exe\bin\*.exe" %TargetBase%\FASTAFileMaker /d /y
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\FastaFileMaker_Exe\bin\*.dll" %TargetBase%\FASTAFileMaker /d /y
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\FastaFileMaker_Exe\bin\*.pdb" %TargetBase%\FASTAFileMaker /d /y
echo Copying the installer to %TargetBase%\Installer
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\Installer\Output\PRISMSeq_Uploader_Installer.exe" %TargetBase%\Installer /d /y
@echo off

if %Iteration%==2 Goto Done

echo.

set Iteration=2
set TargetBase=\\floyd\software\PrismSeq_Uploader

goto Loop

:Done

echo.
echo Copying the CBDMS installer to \\cbdms\DMS_Programs
xcopy "F:\My Documents\Projects\KenAuberry\Organism_Database_Handler\Installer\Output\PRISMSeq_Uploader_CBDMS_Installer.exe" \\cbdms\DMS_Programs\_Installers /d /y

pause