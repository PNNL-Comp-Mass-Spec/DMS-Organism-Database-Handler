// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "RCS1075:Avoid empty catch clause that catches System.Exception.", Justification = "Ignore errors here", Scope = "member", Target = "~M:OrganismDatabaseHandler.ProteinExport.GetFASTAFromDMS.DeleteFASTAIndexFiles(System.IO.FileInfo)")]
[assembly: SuppressMessage("Design", "RCS1075:Avoid empty catch clause that catches System.Exception.", Justification = "Ignore errors here", Scope = "member", Target = "~M:OrganismDatabaseHandler.ProteinImport.ImportHandler.LoadFASTA(System.String)~OrganismDatabaseHandler.ProteinStorage.ProteinStorage")]
[assembly: SuppressMessage("Readability", "RCS1123:Add parentheses when necessary.", Justification = "Parentheses not needed", Scope = "member", Target = "~M:OrganismDatabaseHandler.ProteinExport.GetFASTAFromDMSForward.ExportFASTAFile(System.Collections.Generic.List{System.String},System.String,System.Int32,System.Boolean)~System.String")]
[assembly: SuppressMessage("Roslynator", "RCS1123:Add parentheses when necessary.", Justification = "Parentheses not needed", Scope = "member", Target = "~M:OrganismDatabaseHandler.ProteinExport.ArchiveToFile.DispositionFile(System.Int32,System.String,System.String,System.String,OrganismDatabaseHandler.ProteinExport.GetFASTAFromDMS.SequenceTypes,OrganismDatabaseHandler.ProteinExport.ArchiveOutputFilesBase.CollectionTypes,System.String)~System.Int32")]
[assembly: SuppressMessage("Simplification", "RCS1073:Convert 'if' to 'return' statement.", Justification = "Leave as-is", Scope = "member", Target = "~M:OrganismDatabaseHandler.ProteinExport.GetFASTAFromDMS.ExportLegacyFastaValidateHash(System.IO.FileSystemInfo,System.String@,System.Boolean)~System.Boolean")]
