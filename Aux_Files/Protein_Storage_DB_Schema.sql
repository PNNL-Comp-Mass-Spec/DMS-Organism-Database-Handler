IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Protein_Storage_T3')
	DROP DATABASE [Protein_Storage_T3]
GO

CREATE DATABASE [Protein_Storage_T3]  ON (NAME = N'Protein_Storage_T3_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\Protein_Storage_T3_Data.MDF' , SIZE = 1545, FILEGROWTH = 10%) LOG ON (NAME = N'Protein_Storage_T3_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\Protein_Storage_T3_Log.LDF' , SIZE = 1744, FILEGROWTH = 10%)
 COLLATE SQL_Latin1_General_CP1_CI_AS
GO

exec sp_dboption N'Protein_Storage_T3', N'autoclose', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'bulkcopy', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'trunc. log', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'torn page detection', N'true'
GO

exec sp_dboption N'Protein_Storage_T3', N'read only', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'dbo use', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'single', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'autoshrink', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'ANSI null default', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'recursive triggers', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'ANSI nulls', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'concat null yields null', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'cursor close on commit', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'default to local cursor', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'quoted identifier', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'ANSI warnings', N'false'
GO

exec sp_dboption N'Protein_Storage_T3', N'auto create statistics', N'true'
GO

exec sp_dboption N'Protein_Storage_T3', N'auto update statistics', N'true'
GO

if( (@@microsoftversion / power(2, 24) = 8) and (@@microsoftversion & 0xffff >= 724) )
	exec sp_dboption N'Protein_Storage_T3', N'db chaining', N'false'
GO

use [Protein_Storage_T3]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_DNA_Structures_T_DNA_Structure_Types]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_DNA_Structures] DROP CONSTRAINT FK_T_DNA_Structures_T_DNA_Structure_Types
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Position_Info_T_DNA_Structures]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Position_Info] DROP CONSTRAINT FK_T_Position_Info_T_DNA_Structures
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_DNA_Structures_T_DNA_Translation_Table_Map]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_DNA_Structures] DROP CONSTRAINT FK_T_DNA_Structures_T_DNA_Translation_Table_Map
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_DNA_Translation_Table_Members_T_DNA_Translation_Table_Map]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_DNA_Translation_Table_Members] DROP CONSTRAINT FK_T_DNA_Translation_Table_Members_T_DNA_Translation_Table_Map
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_DNA_Translation_Tables_T_DNA_Translation_Table_Map]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_DNA_Translation_Tables] DROP CONSTRAINT FK_T_DNA_Translation_Tables_T_DNA_Translation_Table_Map
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_DNA_Structures_T_Genome_Assembly]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_DNA_Structures] DROP CONSTRAINT FK_T_DNA_Structures_T_Genome_Assembly
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Genome_Assembly_T_Naming_Authorities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Genome_Assembly] DROP CONSTRAINT FK_T_Genome_Assembly_T_Naming_Authorities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Protein_Names_T_Naming_Authorities]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Protein_Names] DROP CONSTRAINT FK_T_Protein_Names_T_Naming_Authorities
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Genome_Assembly_T_Organisms]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Genome_Assembly] DROP CONSTRAINT FK_T_Genome_Assembly_T_Organisms
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Protein_Names_T_Organisms]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Protein_Names] DROP CONSTRAINT FK_T_Protein_Names_T_Organisms
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Protein_Collections_T_Protein_Collection_States]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Protein_Collections] DROP CONSTRAINT FK_T_Protein_Collections_T_Protein_Collection_States
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Protein_Collections_T_Protein_Collection_Types]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Protein_Collections] DROP CONSTRAINT FK_T_Protein_Collections_T_Protein_Collection_Types
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Protein_Collection_Members_T_Protein_Names]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Protein_Collection_Members] DROP CONSTRAINT FK_T_Protein_Collection_Members_T_Protein_Names
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Position_Info_T_Proteins]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Position_Info] DROP CONSTRAINT FK_T_Position_Info_T_Proteins
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Protein_Collection_Members_T_Proteins]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Protein_Collection_Members] DROP CONSTRAINT FK_T_Protein_Collection_Members_T_Proteins
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_T_Position_Info_T_Reading_Frame_Types]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[T_Position_Info] DROP CONSTRAINT FK_T_Position_Info_T_Reading_Frame_Types
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[add_protein_reference]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[add_protein_reference]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[add_protein_sequence]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[add_protein_sequence]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[add_sha1_file_authentication]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[add_sha1_file_authentication]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[add_update_protein_collection]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[add_update_protein_collection]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[add_update_protein_collection_member]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[add_update_protein_collection_member]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[delete_protein_collection_members]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[delete_protein_collection_members]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[get_protein_collection_id]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[get_protein_collection_id]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[get_protein_collection_member_count]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[get_protein_collection_member_count]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[get_protein_collection_state]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[get_protein_collection_state]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[get_protein_id]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[get_protein_id]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[get_protein_reference_id]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[get_protein_reference_id]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[update_protein_collection_state]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[update_protein_collection_state]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UpdateProteinCollectionsByOrganism]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UpdateProteinCollectionsByOrganism]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Collections_By_Organism]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Collections_By_Organism]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Collection_Picker]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Collection_Picker]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Organism_Picker]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Organism_Picker]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Collection_Count_By_Organism]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Collection_Count_By_Organism]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Collections_Detail_Report]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Collections_Detail_Report]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Collections_List_Report]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Collections_List_Report]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Authority_Picker]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Authority_Picker]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Collection_Organism]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Collection_Organism]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Database_Export]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Database_Export]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Fingerprint_Export]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Fingerprint_Export]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Protein_Storage_Entry_Import]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Protein_Storage_Entry_Import]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Reference_Fingerprint_Export]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Reference_Fingerprint_Export]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V_Table_Row_Counts]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[V_Table_Row_Counts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_DNA_Structure_Types]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_DNA_Structure_Types]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_DNA_Structures]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_DNA_Structures]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_DNA_Translation_Table_Map]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_DNA_Translation_Table_Map]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_DNA_Translation_Table_Members]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_DNA_Translation_Table_Members]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_DNA_Translation_Tables]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_DNA_Translation_Tables]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_FASTA_Namesplit_Characters]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_FASTA_Namesplit_Characters]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Genome_Assembly]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Genome_Assembly]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Naming_Authorities]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Naming_Authorities]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Naming_Authority_Abbreviations]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Naming_Authority_Abbreviations]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Organisms]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Organisms]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Organisms_Ext]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Organisms_Ext]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Position_Info]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Position_Info]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Protein_Collection_Members]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Protein_Collection_Members]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Protein_Collection_States]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Protein_Collection_States]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Protein_Collection_Types]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Protein_Collection_Types]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Protein_Collections]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Protein_Collections]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Protein_Collections_By_Organism]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Protein_Collections_By_Organism]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Protein_Names]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Protein_Names]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Proteins]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Proteins]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[T_Reading_Frame_Types]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[T_Reading_Frame_Types]
GO

if not exists (select * from master.dbo.syslogins where loginname = N'DMSDeveloper')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'master', @loginlang = N'us_english'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'DMSDeveloper', null, @logindb, @loginlang
END
GO

if not exists (select * from master.dbo.syslogins where loginname = N'DMSReader')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'master', @loginlang = N'us_english'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'DMSReader', null, @logindb, @loginlang
END
GO

if not exists (select * from master.dbo.syslogins where loginname = N'DMSWebUser')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'master', @loginlang = N'us_english'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'DMSWebUser', null, @logindb, @loginlang
END
GO

if not exists (select * from master.dbo.syslogins where loginname = N'ERS_User')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'master', @loginlang = N'us_english'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'ERS_User', null, @logindb, @loginlang
END
GO

if not exists (select * from master.dbo.syslogins where loginname = N'LCMSReader')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'master', @loginlang = N'us_english'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'LCMSReader', null, @logindb, @loginlang
END
GO

if not exists (select * from master.dbo.syslogins where loginname = N'LCMSUser')
BEGIN
	declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'master', @loginlang = N'us_english'
	if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)
		select @logindb = N'master'
	if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')
		select @loginlang = @@language
	exec sp_addlogin N'LCMSUser', null, @logindb, @loginlang
END
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\D3E383')
	exec sp_grantlogin N'pnl\D3E383'
	exec sp_defaultdb N'pnl\D3E383', N'master'
	exec sp_defaultlanguage N'pnl\D3E383', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\d3j409')
	exec sp_grantlogin N'pnl\d3j409'
	exec sp_defaultdb N'pnl\d3j409', N'master'
	exec sp_defaultlanguage N'pnl\d3j409', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\d3j410')
	exec sp_grantlogin N'PNL\d3j410'
	exec sp_defaultdb N'PNL\d3j410', N'master'
	exec sp_defaultlanguage N'PNL\d3j410', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\D3J426')
	exec sp_grantlogin N'pnl\D3J426'
	exec sp_defaultdb N'pnl\D3J426', N'master'
	exec sp_defaultlanguage N'pnl\D3J426', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\D3K857')
	exec sp_grantlogin N'PNL\D3K857'
	exec sp_defaultdb N'PNL\D3K857', N'master'
	exec sp_defaultlanguage N'PNL\D3K857', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\d3k901')
	exec sp_grantlogin N'PNL\d3k901'
	exec sp_defaultdb N'PNL\d3k901', N'master'
	exec sp_defaultlanguage N'PNL\d3k901', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\d3L243')
	exec sp_grantlogin N'pnl\d3L243'
	exec sp_defaultdb N'pnl\d3L243', N'master'
	exec sp_defaultlanguage N'pnl\d3L243', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\D3M306')
	exec sp_grantlogin N'pnl\D3M306'
	exec sp_defaultdb N'pnl\D3M306', N'master'
	exec sp_defaultlanguage N'pnl\D3M306', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\d3m480')
	exec sp_grantlogin N'pnl\d3m480'
	exec sp_defaultdb N'pnl\d3m480', N'master'
	exec sp_defaultlanguage N'pnl\d3m480', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\d3m578')
	exec sp_grantlogin N'pnl\d3m578'
	exec sp_defaultdb N'pnl\d3m578', N'master'
	exec sp_defaultlanguage N'pnl\d3m578', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\D3M651')
	exec sp_grantlogin N'PNL\D3M651'
	exec sp_defaultdb N'PNL\D3M651', N'master'
	exec sp_defaultlanguage N'PNL\D3M651', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'pnl\ditsjobmgr')
	exec sp_grantlogin N'pnl\ditsjobmgr'
	exec sp_defaultdb N'pnl\ditsjobmgr', N'master'
	exec sp_defaultlanguage N'pnl\ditsjobmgr', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Archive_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_Archive_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_Archive_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Archive_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_DS_Entry')
	exec sp_grantlogin N'GIGASAX\DMS_DS_Entry'
	exec sp_defaultdb N'GIGASAX\DMS_DS_Entry', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_DS_Entry', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Experiment_Entry')
	exec sp_grantlogin N'GIGASAX\DMS_Experiment_Entry'
	exec sp_defaultdb N'GIGASAX\DMS_Experiment_Entry', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Experiment_Entry', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Instrument_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_Instrument_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_Instrument_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Instrument_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_LC_Column_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_LC_Column_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_LC_Column_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_LC_Column_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Ops_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_Ops_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_Ops_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Ops_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Org_Database_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_Org_Database_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_Org_Database_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Org_Database_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_ParamFile_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_ParamFile_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_ParamFile_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_ParamFile_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Sample_Prep_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_Sample_Prep_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_Sample_Prep_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Sample_Prep_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Storage_Admin')
	exec sp_grantlogin N'GIGASAX\DMS_Storage_Admin'
	exec sp_defaultdb N'GIGASAX\DMS_Storage_Admin', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Storage_Admin', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_SW_Testing')
	exec sp_grantlogin N'GIGASAX\DMS_SW_Testing'
	exec sp_defaultdb N'GIGASAX\DMS_SW_Testing', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_SW_Testing', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Web_DS_ExtendedOps')
	exec sp_grantlogin N'GIGASAX\DMS_Web_DS_ExtendedOps'
	exec sp_defaultdb N'GIGASAX\DMS_Web_DS_ExtendedOps', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Web_DS_ExtendedOps', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Web_DS_List')
	exec sp_grantlogin N'GIGASAX\DMS_Web_DS_List'
	exec sp_defaultdb N'GIGASAX\DMS_Web_DS_List', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Web_DS_List', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'GIGASAX\DMS_Web_Run_Scheduler')
	exec sp_grantlogin N'GIGASAX\DMS_Web_Run_Scheduler'
	exec sp_defaultdb N'GIGASAX\DMS_Web_Run_Scheduler', N'master'
	exec sp_defaultlanguage N'GIGASAX\DMS_Web_Run_Scheduler', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\EMSL-Prism.Users.DMS_Guest')
	exec sp_grantlogin N'PNL\EMSL-Prism.Users.DMS_Guest'
	exec sp_defaultdb N'PNL\EMSL-Prism.Users.DMS_Guest', N'master'
	exec sp_defaultlanguage N'PNL\EMSL-Prism.Users.DMS_Guest', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\EMSL-Prism.Users.DMS_JobRunner')
	exec sp_grantlogin N'PNL\EMSL-Prism.Users.DMS_JobRunner'
	exec sp_defaultdb N'PNL\EMSL-Prism.Users.DMS_JobRunner', N'master'
	exec sp_defaultlanguage N'PNL\EMSL-Prism.Users.DMS_JobRunner', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\EMSL-Prism.Users.DMS_User')
	exec sp_grantlogin N'PNL\EMSL-Prism.Users.DMS_User'
	exec sp_defaultdb N'PNL\EMSL-Prism.Users.DMS_User', N'master'
	exec sp_defaultlanguage N'PNL\EMSL-Prism.Users.DMS_User', N'us_english'
GO

if not exists (select * from master.dbo.syslogins where loginname = N'PNL\EMSL-Prism.Users.Web_Analysis')
	exec sp_grantlogin N'PNL\EMSL-Prism.Users.Web_Analysis'
	exec sp_defaultdb N'PNL\EMSL-Prism.Users.Web_Analysis', N'master'
	exec sp_defaultlanguage N'PNL\EMSL-Prism.Users.Web_Analysis', N'us_english'
GO

exec sp_addsrvrolemember N'PNL\d3j410', sysadmin
GO

exec sp_addsrvrolemember N'PNL\d3j410', securityadmin
GO

exec sp_addsrvrolemember N'PNL\d3j410', serveradmin
GO

exec sp_addsrvrolemember N'PNL\d3j410', setupadmin
GO

exec sp_addsrvrolemember N'PNL\d3j410', processadmin
GO

exec sp_addsrvrolemember N'PNL\d3j410', diskadmin
GO

exec sp_addsrvrolemember N'PNL\d3j410', dbcreator
GO

exec sp_addsrvrolemember N'PNL\d3j410', bulkadmin
GO

if not exists (select * from dbo.sysusers where name = N'BUILTIN\Administrators' and uid < 16382)
	EXEC sp_grantdbaccess N'BUILTIN\Administrators', N'BUILTIN\Administrators'
GO

if not exists (select * from dbo.sysusers where name = N'DMSReader' and uid < 16382)
	EXEC sp_grantdbaccess N'DMSReader', N'DMSReader'
GO

if not exists (select * from dbo.sysusers where name = N'DMSWebUser' and uid < 16382)
	EXEC sp_grantdbaccess N'DMSWebUser', N'DMSWebUser'
GO

if not exists (select * from dbo.sysusers where name = N'EMSL-Prism.Users.DMS_User' and uid < 16382)
	EXEC sp_grantdbaccess N'PNL\EMSL-Prism.Users.DMS_User', N'EMSL-Prism.Users.DMS_User'
GO

if not exists (select * from dbo.sysusers where name = N'pnl\D3E383' and uid < 16382)
	EXEC sp_grantdbaccess N'pnl\D3E383', N'pnl\D3E383'
GO

if not exists (select * from dbo.sysusers where name = N'pnl\d3l243' and uid < 16382)
	EXEC sp_grantdbaccess N'pnl\d3L243', N'pnl\d3l243'
GO

exec sp_addrolemember N'db_accessadmin', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_backupoperator', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_datareader', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_datareader', N'DMSReader'
GO

exec sp_addrolemember N'db_datareader', N'DMSWebUser'
GO

exec sp_addrolemember N'db_datareader', N'EMSL-Prism.Users.DMS_User'
GO

exec sp_addrolemember N'db_datareader', N'pnl\D3E383'
GO

exec sp_addrolemember N'db_datawriter', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_datawriter', N'DMSWebUser'
GO

exec sp_addrolemember N'db_ddladmin', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_denydatareader', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_denydatawriter', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_owner', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_owner', N'pnl\d3l243'
GO

exec sp_addrolemember N'db_securityadmin', N'BUILTIN\Administrators'
GO

CREATE TABLE [dbo].[T_DNA_Structure_Types] (
	[DNA_Structure_Type_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_DNA_Structures] (
	[DNA_Structure_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DNA_Structure_Type_ID] [int] NULL ,
	[DNA_Translation_Table_ID] [int] NULL ,
	[Assembly_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_DNA_Translation_Table_Map] (
	[DNA_Translation_Table_ID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_DNA_Translation_Table_Members] (
	[Translation_Entry_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Coded_AA] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Start_Sequence] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Base_1] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Base_2] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Base_3] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DNA_Translation_Table_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_DNA_Translation_Tables] (
	[Translation_Table_Name_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Translation_Table_Name] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DNA_Translation_Table_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_FASTA_Namesplit_Characters] (
	[Authority_ID] [int] NULL ,
	[Split_Characters] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Genome_Assembly] (
	[Assembly_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Source_File_Path] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Organism_ID] [int] NULL ,
	[Authority_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Naming_Authorities] (
	[Authority_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Description] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Web_Address] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Naming_Authority_Abbreviations] (
	[Authority_ID] [int] NULL ,
	[Xref_Authority_ID] [int] NOT NULL ,
	[Abbreviation] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Organisms] (
	[Organism_ID] [int] NOT NULL ,
	[Short_Name] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DMS_Name] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Storage_Location] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Kingdom] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Genus] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Species] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Strain] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Creation_Date] [datetime] NULL ,
	[DNA_Translation_Table_ID] [int] NULL ,
	[Mito_DNA_Translation_Table_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Organisms_Ext] (
	[Organism_ID] [int] NOT NULL ,
	[Short_Name] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DMS_Name] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Storage_Location] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Domain] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Kingdom] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Phylum] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Class] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Order] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Family] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Genus] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Species] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Strain] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Creation_Date] [datetime] NULL ,
	[DNA_Translation_Table_ID] [int] NULL ,
	[Mito_DNA_Translation_Table_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Position_Info] (
	[Protein_ID] [int] NOT NULL ,
	[Nucleotide_Start_Position] [int] NULL ,
	[Nucleotide_End_Position] [int] NULL ,
	[Reading_Frame_Type_ID] [tinyint] NULL ,
	[DNA_Structure_ID] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Collection_Members] (
	[Member_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Reference_ID] [int] NOT NULL ,
	[Protein_ID] [int] NOT NULL ,
	[Protein_Collection_ID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Collection_States] (
	[Collection_State_ID] [tinyint] NOT NULL ,
	[State] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Collection_Types] (
	[Collection_Type_ID] [tinyint] NOT NULL ,
	[Type] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Collections] (
	[Protein_Collection_ID] [int] IDENTITY (1000, 1) NOT NULL ,
	[FileName] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (900) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Collection_Type_ID] [tinyint] NULL ,
	[Collection_State_ID] [tinyint] NULL ,
	[NumProteins] [int] NULL ,
	[NumResidues] [int] NULL ,
	[DateCreated] [datetime] NULL ,
	[DateModified] [datetime] NULL ,
	[SHA1Authentication] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Collections_By_Organism] (
	[Protein_Collection_ID] [int] NOT NULL ,
	[Display] [varchar] (169) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (900) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Collection_State_ID] [tinyint] NULL ,
	[NumProteins] [int] NULL ,
	[SHA1Authentication] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[FileName] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Organism_ID] [int] NOT NULL ,
	[Authority_ID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Names] (
	[Reference_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Description] [varchar] (900) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Organism_ID] [int] NOT NULL ,
	[Authority_ID] [int] NOT NULL ,
	[Reference_Fingerprint] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[DateAdded] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Proteins] (
	[Protein_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Sequence] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Length] [int] NOT NULL ,
	[Molecular_Formula] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Monoisotopic_Mass] [float] NULL ,
	[Average_Mass] [float] NULL ,
	[SHA1_Hash] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[DateCreated] [datetime] NULL ,
	[DateModified] [datetime] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Reading_Frame_Types] (
	[Reading_Frame_Type_ID] [tinyint] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

 CREATE  CLUSTERED  INDEX [IX_T_Protein_Collection_Members_Coll_ID] ON [dbo].[T_Protein_Collection_Members]([Protein_Collection_ID]) ON [PRIMARY]
GO

 CREATE  CLUSTERED  INDEX [IX_T_Protein_Names_Org_ID] ON [dbo].[T_Protein_Names]([Organism_ID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_T_DNA_Translation_Tables] ON [dbo].[T_DNA_Translation_Tables]([Translation_Table_Name]) ON [PRIMARY]
GO

 CREATE  INDEX [T_Protein_Collection_Members8] ON [dbo].[T_Protein_Collection_Members]([Reference_ID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_T_Protein_Names_Name] ON [dbo].[T_Protein_Names]([Name]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_T_Protein_Names_Ref_Fingerprint] ON [dbo].[T_Protein_Names]([Reference_Fingerprint]) ON [PRIMARY]
GO

 CREATE  INDEX [T_Protein_Names9] ON [dbo].[T_Protein_Names]([Reference_ID], [Organism_ID], [Authority_ID]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_T_Proteins_SHA1_Hash] ON [dbo].[T_Proteins]([SHA1_Hash]) ON [PRIMARY]
GO

GRANT  SELECT  ON [dbo].[T_DNA_Structure_Types]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_DNA_Structures]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_DNA_Translation_Table_Map]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_DNA_Translation_Table_Members]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_DNA_Translation_Tables]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Genome_Assembly]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Naming_Authorities]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Organisms]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Organisms_Ext]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Position_Info]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Protein_Collection_Members]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Protein_Collection_States]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Protein_Collection_Types]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Protein_Collections]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Protein_Names]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Proteins]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[T_Reading_Frame_Types]  TO [DMSReader]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Authority_Picker
AS
SELECT     TOP 100 PERCENT Authority_ID AS ID, Name AS Display_Name, Description + ' <' + Web_Address + '>' AS Details
FROM         dbo.T_Naming_Authorities
ORDER BY Name

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Authority_Picker]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Authority_Picker]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collection_Organism
AS
SELECT     dbo.T_Protein_Collection_Members.Protein_Collection_ID, dbo.T_Protein_Names.Organism_ID, dbo.T_Protein_Names.Authority_ID
FROM         dbo.T_Protein_Collection_Members INNER JOIN
                      dbo.T_Protein_Names ON dbo.T_Protein_Collection_Members.Reference_ID = dbo.T_Protein_Names.Reference_ID
GROUP BY dbo.T_Protein_Names.Organism_ID, dbo.T_Protein_Collection_Members.Protein_Collection_ID, dbo.T_Protein_Names.Authority_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Protein_Collection_Organism]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Protein_Collection_Organism]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Database_Export
AS
SELECT     dbo.T_Protein_Names.Name, dbo.T_Protein_Names.Description, dbo.T_Proteins.Sequence, 
                      dbo.T_Protein_Collection_Members.Protein_Collection_ID
FROM         dbo.T_Protein_Collection_Members INNER JOIN
                      dbo.T_Protein_Names ON dbo.T_Protein_Collection_Members.Reference_ID = dbo.T_Protein_Names.Reference_ID INNER JOIN
                      dbo.T_Proteins ON dbo.T_Protein_Collection_Members.Protein_ID = dbo.T_Proteins.Protein_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Protein_Database_Export]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Protein_Database_Export]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Fingerprint_Export
AS
SELECT     TOP 100 PERCENT SHA1_Hash AS Fingerprint, Protein_ID, Length
FROM         dbo.T_Proteins
ORDER BY Protein_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Protein_Fingerprint_Export]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Protein_Fingerprint_Export]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Storage_Entry_Import
AS
SELECT     dbo.T_Protein_Names.Name, dbo.T_Protein_Names.Description, dbo.T_Proteins.Sequence, dbo.T_Proteins.Monoisotopic_Mass, 
                      dbo.T_Proteins.Average_Mass, dbo.T_Proteins.Length, dbo.T_Proteins.Molecular_Formula, dbo.T_Proteins.SHA1_Hash, dbo.T_Proteins.Protein_ID, 
                      dbo.T_Protein_Names.Reference_ID, dbo.T_Protein_Collection_Members.Protein_Collection_ID
FROM         dbo.T_Protein_Collection_Members INNER JOIN
                      dbo.T_Protein_Names ON dbo.T_Protein_Collection_Members.Reference_ID = dbo.T_Protein_Names.Reference_ID INNER JOIN
                      dbo.T_Proteins ON dbo.T_Protein_Collection_Members.Protein_ID = dbo.T_Proteins.Protein_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Protein_Storage_Entry_Import]  TO [DMSReader]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Reference_Fingerprint_Export
AS
SELECT     TOP 100 PERCENT Reference_Fingerprint AS Fingerprint, Reference_ID
FROM         dbo.T_Protein_Names
ORDER BY Reference_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Reference_Fingerprint_Export]  TO [DMSReader]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE VIEW dbo.V_Table_Row_Counts
AS
SELECT TOP 100 PERCENT o.name AS TableName, 
    i.rows AS TableRowCount
FROM dbo.sysobjects o INNER JOIN
    dbo.sysindexes i ON o.id = i.id
WHERE (o.type = 'u') AND (i.indid < 2) AND 
    (o.name <> 'dtproperties')
ORDER BY o.name



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Table_Row_Counts]  TO [DMSReader]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Organism_Picker
AS
SELECT     TOP 100 PERCENT Organism_ID AS ID, Short_Name, Short_Name + ' - ' + Description AS Display_Name, Storage_Location
FROM         dbo.T_Organisms_Ext
ORDER BY Short_Name

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Organism_Picker]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Organism_Picker]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collection_Count_By_Organism
AS
SELECT     TOP 100 PERCENT COUNT(dbo.V_Protein_Collection_Organism.Protein_Collection_ID) AS Collection_Count, 
                      dbo.T_Organisms_Ext.Organism_ID AS Expr1
FROM         dbo.V_Protein_Collection_Organism INNER JOIN
                      dbo.T_Organisms_Ext ON dbo.V_Protein_Collection_Organism.Organism_ID = dbo.T_Organisms_Ext.Organism_ID
GROUP BY dbo.T_Organisms_Ext.Organism_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Protein_Collection_Count_By_Organism]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Protein_Collection_Count_By_Organism]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collections_Detail_Report
AS
SELECT     dbo.T_Protein_Collections.Protein_Collection_ID AS [Collection ID], dbo.T_Protein_Collections.FileName AS Name, 
                      dbo.T_Protein_Collections.Description, dbo.T_Protein_Collections.NumProteins AS [Protein Count], 
                      dbo.T_Protein_Collections.NumResidues AS [Residue Count], dbo.T_Protein_Collections.DateCreated AS Created, 
                      dbo.T_Protein_Collections.DateModified AS [Last Modified], dbo.T_Protein_Collection_Types.Type, dbo.T_Protein_Collection_States.State, 
                      dbo.T_Protein_Collections.SHA1Authentication AS [SHA-1 Fingerprint]
FROM         dbo.T_Protein_Collections INNER JOIN
                      dbo.T_Protein_Collection_States ON dbo.T_Protein_Collections.Collection_State_ID = dbo.T_Protein_Collection_States.Collection_State_ID INNER JOIN
                      dbo.T_Protein_Collection_Types ON dbo.T_Protein_Collections.Collection_Type_ID = dbo.T_Protein_Collection_Types.Collection_Type_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collections_List_Report
AS
SELECT     dbo.T_Protein_Collections.Protein_Collection_ID AS [Collection ID], dbo.T_Protein_Collections.FileName AS Name, 
                      dbo.T_Protein_Collections.Description, dbo.T_Protein_Collection_States.State AS State, dbo.T_Protein_Collections.NumProteins AS [Protein Count], 
                      dbo.T_Protein_Collections.NumResidues AS [Residue Count], dbo.T_Protein_Collections.DateCreated AS Created, 
                      dbo.T_Protein_Collections.DateModified AS [Last Modified]
FROM         dbo.T_Protein_Collections INNER JOIN
                      dbo.T_Protein_Collection_States ON dbo.T_Protein_Collections.Collection_State_ID = dbo.T_Protein_Collection_States.Collection_State_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Collection_Picker
AS
SELECT     TOP 100 PERCENT dbo.T_Protein_Collections.Protein_Collection_ID AS ID, dbo.T_Protein_Collections.FileName AS Name, 
                      dbo.T_Protein_Collections.DateCreated AS Created, dbo.T_Protein_Collections.DateModified AS Modified, dbo.T_Protein_Collection_States.State
FROM         dbo.T_Protein_Collections INNER JOIN
                      dbo.T_Protein_Collection_States ON dbo.T_Protein_Collections.Collection_State_ID = dbo.T_Protein_Collection_States.Collection_State_ID
ORDER BY dbo.T_Protein_Collections.FileName

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collections_By_Organism
AS
SELECT DISTINCT 
                      TOP 100 PERCENT dbo.T_Protein_Collections.Protein_Collection_ID, 
                      dbo.T_Protein_Collections.FileName + ' (' + CAST(dbo.T_Protein_Collections.NumProteins AS varchar) + ' Entries)' AS Display, 
                      dbo.T_Protein_Collections.FileName, dbo.T_Protein_Names.Organism_ID, dbo.T_Protein_Names.Authority_ID
FROM         dbo.T_Protein_Collections INNER JOIN
                      dbo.T_Protein_Collection_Members ON 
                      dbo.T_Protein_Collections.Protein_Collection_ID = dbo.T_Protein_Collection_Members.Protein_Collection_ID INNER JOIN
                      dbo.T_Protein_Names ON dbo.T_Protein_Collection_Members.Reference_ID = dbo.T_Protein_Names.Reference_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  SELECT  ON [dbo].[V_Protein_Collections_By_Organism]  TO [DMSReader]
GO

GRANT  SELECT  ON [dbo].[V_Protein_Collections_By_Organism]  TO [pnl\d3l243]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE add_protein_reference

/****************************************************
**
**	Desc: Adds a new protein reference entry to T_Protein_Names
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 10/08/2004
**    
*****************************************************/

(
	@name varchar(128),
	@description varchar(900),
	@organism_ID int,
	@authority_ID int,
	@nameDescHash varchar(28),
	@message varchar(256) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)

	---------------------------------------------------
	-- Does entry already exist?
	---------------------------------------------------
	
	declare @Reference_ID int
	set @Reference_ID = 0
	
	execute @Reference_ID = get_protein_reference_id @name, @description, @nameDescHash
	
	if @Reference_ID > 0
	begin
		return @Reference_ID
	end
	

	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'add_protein_referenceEntry'
	begin transaction @transName

	INSERT INTO T_Protein_Names (
		[Name],
		Description,
		Organism_ID,
		Authority_ID,
		Reference_Fingerprint,
		DateAdded
	) VALUES (
		@name, 
		@description,
		@organism_ID, 
		@authority_ID,
		@nameDescHash,
		GETDATE()
	)
		
		
	--execute @Protein_ID = get_protein_id @length, @sha1_hash 		
	SELECT @Reference_ID = @@Identity
		
		--
		SELECT @myError = @@error, @myRowCount = @@rowcount
		--
		if @myError <> 0
		begin
			rollback transaction @transName
			set @msg = 'Insert operation failed!'
			RAISERROR (@msg, 10, 1)
			return 51007
		end
		
	commit transaction @transName

	return @Reference_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE add_protein_sequence

/****************************************************
**
**	Desc: Adds a new protein sequence entry to T_Proteins
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 10/06/2004
**    
*****************************************************/

(
	@sequence text,
	@length int,
	@molecular_formula varchar(128),
	@monoisotopic_mass float(8),
	@average_mass float(8),
	@sha1_hash varchar(28),
	@mode varchar(12) = 'add',
	@message varchar(512) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)

	---------------------------------------------------
	-- Does entry already exist?
	---------------------------------------------------
	
	declare @Protein_ID int
	set @Protein_ID = 0
	
	execute @Protein_ID = get_protein_id @length, @sha1_hash
	
	if @Protein_ID > 0 and @mode = 'add'
	begin
		return @Protein_ID
	end
			
	
	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'AddProteinCollectionEntry'
	begin transaction @transName


	---------------------------------------------------
	-- action for add mode
	---------------------------------------------------
	if @mode = 'add'
	begin

		INSERT INTO T_Proteins (
			[Sequence],
			Length,
			Molecular_Formula,
			Monoisotopic_Mass,
			Average_Mass,
			SHA1_Hash,
			DateCreated,
			DateModified
		) VALUES (
			@sequence, 
			@length, 
			@molecular_formula,
			@monoisotopic_mass,
			@average_mass, 
			@sha1_hash,
			GETDATE(),
			GETDATE()
		)
		
		
	SELECT @Protein_ID = @@Identity 		
		
		--
		SELECT @myError = @@error, @myRowCount = @@rowcount
		--
		if @myError <> 0
		begin
			rollback transaction @transName
			set @msg = 'Insert operation failed!'
			RAISERROR (@msg, 10, 1)
			return 51007
		end
	end
		
	commit transaction @transName

	return @Protein_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE add_sha1_file_authentication

/****************************************************
**
**	Desc: Adds a SHA1 fingerprint to a given Protein Collection Entry
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 04/15/2005
**    
*****************************************************/

(
	@Collection_ID int,
	@SHA1FileHash varchar(28),
	@message varchar(512) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)

	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'add_sha1_file_authentication'
	begin transaction @transName


	---------------------------------------------------
	-- action for add mode
	---------------------------------------------------
	begin

	UPDATE T_Protein_Collections
	SET 
		SHA1Authentication = @SHA1FileHash,
		DateModified = GETDATE()
		
	WHERE (Protein_Collection_ID = @Collection_ID)	
		
				
		--
		SELECT @myError = @@error, @myRowCount = @@rowcount
		--
		if @myError <> 0
		begin
			rollback transaction @transName
			set @msg = 'Update operation failed!'
			RAISERROR (@msg, 10, 1)
			return 51007
		end
	end
		
	commit transaction @transName
	
	return 0 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE add_update_protein_collection
/****************************************************
**
**	Desc: Adds a new protein collection entry
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 09/29/2004
**    
*****************************************************/
(
	@fileName varchar(128), 
	@Description varchar(900),
	@collection_type int = 1,
	@collection_state int,
	@numProteins int = 0,
	@numResidues int = 0,
	@active int = 1,
	@mode varchar(12) = 'add',
	@message varchar(512) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)

	---------------------------------------------------
	-- Validate input fields
	---------------------------------------------------

	set @myError = 0
	if LEN(@fileName) < 1
	begin
		set @myError = 51000
		RAISERROR ('fileName was blank',
			10, 1)
	end

	--
	
	if @myError <> 0
		return @myError
		
	
		
	---------------------------------------------------
	-- Does entry already exist?
	---------------------------------------------------
	
	declare @Collection_ID int
	set @Collection_ID = 0
	
	execute @Collection_ID = get_protein_collection_id @fileName
	
	if @Collection_ID > 0 and @mode = 'add'
	begin
		set @mode = 'update'
	end
	
	if @Collection_ID = 0 and @mode = 'update'
	begin
		set @mode = 'add'
	end
	
		
	
	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'AddProteinCollectionEntry'
	begin transaction @transName


	---------------------------------------------------
	-- action for add mode
	---------------------------------------------------
	if @mode = 'add'
	begin

		INSERT INTO T_Protein_Collections (
			FileName,
			Description,
			Collection_Type_ID,
			Collection_State_ID,
			NumProteins,
			NumResidues,
			DateCreated,
			DateModified
		) VALUES (
			@fileName, 
			@Description,
			@collection_type,
			@collection_state,
			@numProteins, 
			@numResidues,
			GETDATE(),
			GETDATE()
		)
		--
		SELECT @myError = @@error, @myRowCount = @@rowcount
		--
		if @myError <> 0
		begin
			rollback transaction @transName
			set @msg = 'Insert operation failed: "' + @filename + '"'
			RAISERROR (@msg, 10, 1)
			return 51007
		end
	end
	
	if @mode = 'update'
	begin
		
		UPDATE T_Protein_Collections
		SET
			Description = @Description,
			Collection_State_ID = @collection_state,
			Collection_Type_ID = @collection_type,
			NumProteins = @numProteins,
			NumResidues = @numResidues,
			DateModified = GETDATE()
		
		WHERE (FileName = @fileName)
		
			
		SELECT @myError = @@error, @myRowCount = @@rowcount
		--
		if @myError <> 0
		begin
			rollback transaction @transName
			set @msg = 'Update operation failed: "' + @filename + '"'
			RAISERROR (@msg, 10, 1)
			return 51002
		end
	end
	
		SELECT @Collection_ID = @@Identity

	commit transaction @transName
		
	return @Collection_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

GRANT  EXECUTE  ON [dbo].[add_update_protein_collection]  TO [DMSWebUser]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE add_update_protein_collection_member
/****************************************************
**
**	Desc: Adds a new protein collection member
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 10/06/2004
**    
*****************************************************/
(	
	@reference_ID int,
	@protein_ID int,
	@protein_collection_ID int,
	@message varchar(256) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)
	declare @member_ID int

	---------------------------------------------------
	-- Does entry already exist?
	---------------------------------------------------
	
--	declare @ID_Check int
--	set @ID_Check = 0
--	
--	SELECT @ID_Check = Protein_ID FROM T_Protein_Collection_Members
--	WHERE Protein_Collection_ID = @protein_collection_ID
--	
--	if @ID_Check > 0
--	begin
--		return 1  -- Entry already exists
--	end
		
	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'AddProteinCollectionMember'
	begin transaction @transName


	---------------------------------------------------
	-- action for add mode
	---------------------------------------------------
	INSERT INTO T_Protein_Collection_Members (
		Reference_ID,
		Protein_ID,
		Protein_Collection_ID
	) VALUES (
		@reference_ID,
		@protein_ID, 
		@protein_collection_ID
	)
	--
	
	SELECT @member_ID = @@Identity 		

	SELECT @myError = @@error, @myRowCount = @@rowcount
	--
	if @myError <> 0
	begin
		rollback transaction @transName
		set @msg = 'Insert operation failed: "' + @protein_ID + '"'
		RAISERROR (@msg, 10, 1)
		return 51007
	end
		
	commit transaction @transName
		
	return @member_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE delete_protein_collection_members
/****************************************************
**
**	Desc: Deletes Protein Collection Member Entries from a given Protein Collection ID
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 10/07/2004
**    
*****************************************************/
(
	@Collection_ID int,
	@message varchar(512) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	set @message = ''
	
	declare @msg varchar(256)
	
	declare @result int
	
	---------------------------------------------------
	-- Check if collection is OK to delete
	---------------------------------------------------
	
	declare @collectionState int
	
	SELECT @collectionState = Collection_State_ID
		FROM T_Protein_Collections
		WHERE Protein_Collection_ID = @Collection_ID
						
		declare @Collection_Name varchar(128)
		declare @State_Name varchar(64)
		
	SELECT @Collection_Name = FileName
		FROM T_Protein_Collections
		WHERE (Protein_Collection_ID = @Collection_ID)
	
	SELECT @State_Name = State
		FROM T_Protein_Collection_States
		WHERE (Collection_State_ID = @collectionState)					

	if @collectionState > 2	
	begin
		set @msg = 'Cannot Delete collection "' + @Collection_Name + '": ' + @State_Name + ' collections are protected'
		RAISERROR (@msg,10, 1)
			
		return 51140
	end
	
	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'delete_protein_collection_members'
	begin transaction @transName
--	print 'start transaction' -- debug only

	---------------------------------------------------
	-- delete any entries for the parameter file from the entries table
	---------------------------------------------------

	DELETE FROM T_Protein_Collection_Members 
	WHERE (Protein_Collection_ID = @Collection_ID)
	
	if @@error <> 0
	begin
		rollback transaction @transName
		RAISERROR ('Delete from entries table was unsuccessful for collection',
			10, 1)
		return 51130
	end
			
	commit transaction @transname
	
	return 0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE get_protein_collection_id
/****************************************************
**
**	Desc: Gets CollectionID for given FileName
**
**
**	Parameters: 
**
**		Auth: kja
**		Date: 9/29/2004
**    
*****************************************************/
(
	@fileName varchar(128)
)
As
	declare @Collection_ID int
	set @Collection_ID = 0
	
	SELECT @Collection_ID = Protein_Collection_ID FROM T_Protein_Collections
	 WHERE (FileName = @fileName)
	
	return @Collection_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE get_protein_collection_member_count
/****************************************************
**
**	Desc: Gets Collection Member count for given Collection_ID
**
**
**	Parameters: 
**
**		Auth: kja
**		Date: 10/07/2004
**    
*****************************************************/
(
	@Collection_ID int
)
As
	set nocount on
	
	declare @Collection_Member_Count int
	set @Collection_Member_Count = 0
	
SELECT @Collection_Member_Count = COUNT(*)
	FROM T_Protein_Collection_Members
	GROUP BY Protein_Collection_ID
	HAVING (Protein_Collection_ID = @Collection_ID)
	
	if @@rowcount = 0
	begin
		return 0
	end
	
	return(@Collection_Member_Count)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE get_protein_collection_state
/****************************************************
**
**	Desc: Gets Collection State Name for given CollectionID
**
**
**	Parameters: 
**
**		Auth: kja
**		Date: 08/04/2005
**    
*****************************************************/
(
	@Collection_ID int,
	@State_Name varchar(32) OUTPUT
)

As
	declare @State_ID int
	
	set @State_ID = 0
	set @State_Name = 'New'
	
	SELECT @State_ID = Collection_State_ID
	FROM T_Protein_Collections
	WHERE (Protein_Collection_ID = @Collection_ID)
	
	
	
	SELECT @State_Name = State
	FROM T_Protein_Collection_States
	WHERE (Collection_State_ID = @State_ID)
	
	return 0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE get_protein_id
/****************************************************
**
**	Desc: Gets ProteinID for given length and SHA-1 Hash
**
**
**	Parameters: 
**
**		Auth: kja
**		Date: 10/06/2004
**    
*****************************************************/
(
	@length int,
	@hash varchar(28)
)
As
	declare @Protein_ID int
	set @Protein_ID = 0
	
	SELECT @Protein_ID = Protein_ID FROM T_Proteins
	 WHERE (Length = @length AND SHA1_Hash = @hash)
	
	return @Protein_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE get_protein_reference_id
/****************************************************
**
**	Desc: Gets CollectionID for given FileName
**
**
**	Parameters: 
**
**		Auth: kja
**		Date: 10/08/2004
**    
*****************************************************/
(
	@name varchar(128),
	@description varchar(900),
	@nameDescHash varchar(28)
)
As
	declare @reference_ID int
	set @reference_ID = 0
	
	SELECT @reference_ID = Reference_ID FROM T_Protein_Names
	 WHERE (Reference_Fingerprint = @nameDescHash)
	 
	 if @@rowcount > 1
	 begin
		SELECT @reference_ID = Reference_ID FROM T_Protein_Names
	     WHERE ([Name] = @name and Description = @description)
	 end
	
	return @reference_ID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE update_protein_collection_state
/****************************************************
**
**	Desc: Adds a new protein collection member
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 07/28/2005
**    
*****************************************************/
(	
	@protein_collection_ID int,
	@state_ID int,
	@message varchar(256) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)
	
	---------------------------------------------------
	-- Make sure that the @state_ID value exists in
	-- T_Protein_Collection_States
	---------------------------------------------------

	declare @ID_Check int
	set @ID_Check = 0
	
	SELECT @ID_Check = Collection_State_ID FROM T_Protein_Collection_States
	WHERE Collection_State_ID = @state_ID
	
	if @ID_Check = 0
	begin
		set @message = 'Collection_State_ID: "' + @state_ID + '" does not exist'
		RAISERROR (@message, 10, 1)
		return 1  -- State Does not exist
	end

	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(32)
	set @transName = 'update_protein_collection_state'
	begin transaction @transName
	
	---------------------------------------------------
	-- action for update mode
	---------------------------------------------------
		UPDATE T_Protein_Collections
		SET
			Collection_State_ID = @state_ID, 
			DateModified = GETDATE()
		WHERE
			(Protein_Collection_ID = @protein_collection_ID)
	--
	
	SELECT @myError = @@error, @myRowCount = @@rowcount
	--
	if @myError <> 0
	begin
		rollback transaction @transName
		set @message = 'Update operation failed: The state of "' + @protein_collection_ID + '" could not be updated'
		RAISERROR (@message, 10, 1)
		return 51007
	end
	
	
	commit transaction @transName
		
	return 0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE UpdateProteinCollectionsByOrganism
/****************************************************
**
**	Desc: Refreshes the cached table of Collections
**        and their associated organisms
**
**	Return values: 0: success, otherwise, error code
**
**	Parameters: 
**
**	
**
**		Auth: kja
**		Date: 09/29/2004
**    
*****************************************************/
(
	@message varchar(512) output
)
As
	set nocount on

	declare @myError int
	set @myError = 0

	declare @myRowCount int
	set @myRowCount = 0
	
	declare @msg varchar(256)

	---------------------------------------------------
	-- Start transaction
	---------------------------------------------------

	declare @transName varchar(64)
	set @transName = 'UpdateProteinCollectionsByOrganism'
	begin transaction @transName

	begin
		DROP TABLE T_Protein_Collections_By_Organism

		SELECT  
			T_Protein_Collections.Protein_Collection_ID, 
			T_Protein_Collections.FileName + ' (' + CAST(dbo.T_Protein_Collections.NumProteins AS varchar)
                       + ' Entries)' AS Display, 
            T_Protein_Collections.Description, 
            T_Protein_Collections.Collection_State_ID, 
            T_Protein_Collections.NumProteins,
            T_Protein_Collections.SHA1Authentication, 
            T_Protein_Collections.FileName, 
            V_Protein_Collection_Organism.Organism_ID,
            V_Protein_Collection_Organism.Authority_ID
        INTO T_Protein_Collections_By_Organism
        FROM T_Protein_Collections INNER JOIN
                      V_Protein_Collection_Organism 
                      ON T_Protein_Collections.Protein_Collection_ID = V_Protein_Collection_Organism.Protein_Collection_ID
		--
		SELECT @myError = @@error, @myRowCount = @@rowcount
		--
		if @myError <> 0
		begin
			rollback transaction @transName
			set @msg = 'Insert operation failed'
			RAISERROR (@msg, 10, 1)
			return 51007
		end
	end
	
	commit transaction @transName

	
	return 0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

