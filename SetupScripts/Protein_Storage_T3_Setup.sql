CREATE DATABASE [Protein_Storage_T3]  ON (NAME = N'Protein_Storage_T3_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\Protein_Storage_T3_Data.MDF' , SIZE = 12, FILEGROWTH = 10%) LOG ON (NAME = N'Protein_Storage_T3_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\Protein_Storage_T3_Log.LDF' , SIZE = 38, FILEGROWTH = 10%)
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

use [Protein_Storage_T3]
GO

if not exists (select * from dbo.sysusers where name = N'BUILTIN\Administrators' and uid < 16382)
	EXEC sp_grantdbaccess N'BUILTIN\Administrators', N'BUILTIN\Administrators'
GO

if not exists (select * from dbo.sysusers where name = N'DMSReader' and uid < 16382)
	EXEC sp_grantdbaccess N'DMSReader', N'DMSReader'
GO

if not exists (select * from dbo.sysusers where name = N'pnl\D3E383' and uid < 16382)
	EXEC sp_grantdbaccess N'pnl\D3E383', N'pnl\D3E383'
GO

exec sp_addrolemember N'db_accessadmin', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_backupoperator', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_datareader', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_datareader', N'DMSReader'
GO

exec sp_addrolemember N'db_datareader', N'pnl\D3E383'
GO

exec sp_addrolemember N'db_datawriter', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_ddladmin', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_denydatareader', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_denydatawriter', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_owner', N'BUILTIN\Administrators'
GO

exec sp_addrolemember N'db_securityadmin', N'BUILTIN\Administrators'
GO

CREATE TABLE [dbo].[T_DNA_Structure_Types] (
	[DNA_Structure_Type_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_DNA_Translation_Table_Map] (
	[DNA_Translation_Table_ID] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Naming_Authorities] (
	[Authority_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Web_Address] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
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

CREATE TABLE [dbo].[T_Proteins] (
	[Protein_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Sequence] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Length] [int] NULL ,
	[Molecular_Formula] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Monoisotopic_Mass] [float] NULL ,
	[Average_Mass] [float] NULL ,
	[SHA1_Hash] [varchar] (28) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
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

CREATE TABLE [dbo].[T_Genome_Assembly] (
	[Assembly_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Source_File_Path] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Organism_ID] [int] NULL ,
	[Authority_ID] [int] NULL 
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
	[DateModified] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[T_Protein_Names] (
	[Reference_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Description] [varchar] (900) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Organism_ID] [int] NULL ,
	[Authority_ID] [int] NULL ,
	[DateAdded] [datetime] NULL 
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

CREATE TABLE [dbo].[T_Protein_Collection_Members] (
	[Member_ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Reference_ID] [int] NULL ,
	[Protein_ID] [int] NULL ,
	[Protein_Collection_ID] [int] NULL 
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

ALTER TABLE [dbo].[T_DNA_Structure_Types] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_DNA_Structure_Types] PRIMARY KEY  CLUSTERED 
	(
		[DNA_Structure_Type_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_DNA_Translation_Table_Map] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_DNA_Translation_Table_Map] PRIMARY KEY  CLUSTERED 
	(
		[DNA_Translation_Table_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Naming_Authorities] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Naming_Authorities] PRIMARY KEY  CLUSTERED 
	(
		[Authority_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Organisms] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Organisms] PRIMARY KEY  CLUSTERED 
	(
		[Organism_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Protein_Collection_States] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Protein_Collection_States] PRIMARY KEY  CLUSTERED 
	(
		[Collection_State_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Protein_Collection_Types] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Protein_Collection_Types] PRIMARY KEY  CLUSTERED 
	(
		[Collection_Type_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Proteins] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Proteins] PRIMARY KEY  CLUSTERED 
	(
		[Protein_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Reading_Frame_Types] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Reading_Frame_Types] PRIMARY KEY  CLUSTERED 
	(
		[Reading_Frame_Type_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_DNA_Translation_Table_Members] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Genetic_Code_Translation_Entries] PRIMARY KEY  CLUSTERED 
	(
		[Translation_Entry_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_DNA_Translation_Tables] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_DNA_Translation_Tables] PRIMARY KEY  CLUSTERED 
	(
		[Translation_Table_Name_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Genome_Assembly] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Genome_Assembly] PRIMARY KEY  CLUSTERED 
	(
		[Assembly_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Protein_Collections] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Protein_Collections] PRIMARY KEY  CLUSTERED 
	(
		[Protein_Collection_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Protein_Names] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Protein_Names] PRIMARY KEY  CLUSTERED 
	(
		[Reference_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_DNA_Structures] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_DNA_Structures] PRIMARY KEY  CLUSTERED 
	(
		[DNA_Structure_ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[T_Protein_Collection_Members] WITH NOCHECK ADD 
	CONSTRAINT [PK_T_Protein_Collection_Members] PRIMARY KEY  CLUSTERED 
	(
		[Member_ID]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_T_Proteins] ON [dbo].[T_Proteins]([Protein_ID]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_T_Proteins_Hash] ON [dbo].[T_Proteins]([SHA1_Hash]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_T_DNA_Translation_Tables] ON [dbo].[T_DNA_Translation_Tables]([Translation_Table_Name]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_DNA_Translation_Table_Members] ADD 
	CONSTRAINT [FK_T_DNA_Translation_Table_Members_T_DNA_Translation_Table_Map] FOREIGN KEY 
	(
		[DNA_Translation_Table_ID]
	) REFERENCES [dbo].[T_DNA_Translation_Table_Map] (
		[DNA_Translation_Table_ID]
	)
GO

ALTER TABLE [dbo].[T_DNA_Translation_Tables] ADD 
	CONSTRAINT [FK_T_DNA_Translation_Tables_T_DNA_Translation_Table_Map] FOREIGN KEY 
	(
		[DNA_Translation_Table_ID]
	) REFERENCES [dbo].[T_DNA_Translation_Table_Map] (
		[DNA_Translation_Table_ID]
	)
GO

ALTER TABLE [dbo].[T_Genome_Assembly] ADD 
	CONSTRAINT [FK_T_Genome_Assembly_T_Naming_Authorities] FOREIGN KEY 
	(
		[Authority_ID]
	) REFERENCES [dbo].[T_Naming_Authorities] (
		[Authority_ID]
	),
	CONSTRAINT [FK_T_Genome_Assembly_T_Organisms] FOREIGN KEY 
	(
		[Organism_ID]
	) REFERENCES [dbo].[T_Organisms] (
		[Organism_ID]
	)
GO

ALTER TABLE [dbo].[T_Protein_Collections] ADD 
	CONSTRAINT [FK_T_Protein_Collections_T_Protein_Collection_States] FOREIGN KEY 
	(
		[Collection_State_ID]
	) REFERENCES [dbo].[T_Protein_Collection_States] (
		[Collection_State_ID]
	),
	CONSTRAINT [FK_T_Protein_Collections_T_Protein_Collection_Types] FOREIGN KEY 
	(
		[Collection_Type_ID]
	) REFERENCES [dbo].[T_Protein_Collection_Types] (
		[Collection_Type_ID]
	)
GO

ALTER TABLE [dbo].[T_Protein_Names] ADD 
	CONSTRAINT [FK_T_Protein_Names_T_Naming_Authorities] FOREIGN KEY 
	(
		[Authority_ID]
	) REFERENCES [dbo].[T_Naming_Authorities] (
		[Authority_ID]
	),
	CONSTRAINT [FK_T_Protein_Names_T_Organisms] FOREIGN KEY 
	(
		[Organism_ID]
	) REFERENCES [dbo].[T_Organisms] (
		[Organism_ID]
	)
GO

ALTER TABLE [dbo].[T_DNA_Structures] ADD 
	CONSTRAINT [FK_T_DNA_Structures_T_DNA_Structure_Types] FOREIGN KEY 
	(
		[DNA_Structure_Type_ID]
	) REFERENCES [dbo].[T_DNA_Structure_Types] (
		[DNA_Structure_Type_ID]
	),
	CONSTRAINT [FK_T_DNA_Structures_T_DNA_Translation_Table_Map] FOREIGN KEY 
	(
		[DNA_Translation_Table_ID]
	) REFERENCES [dbo].[T_DNA_Translation_Table_Map] (
		[DNA_Translation_Table_ID]
	),
	CONSTRAINT [FK_T_DNA_Structures_T_Genome_Assembly] FOREIGN KEY 
	(
		[Assembly_ID]
	) REFERENCES [dbo].[T_Genome_Assembly] (
		[Assembly_ID]
	)
GO

ALTER TABLE [dbo].[T_Protein_Collection_Members] ADD 
	CONSTRAINT [FK_T_Protein_Collection_Map_T_Protein_Collections] FOREIGN KEY 
	(
		[Protein_Collection_ID]
	) REFERENCES [dbo].[T_Protein_Collections] (
		[Protein_Collection_ID]
	),
	CONSTRAINT [FK_T_Protein_Collection_Map_T_Proteins] FOREIGN KEY 
	(
		[Protein_ID]
	) REFERENCES [dbo].[T_Proteins] (
		[Protein_ID]
	),
	CONSTRAINT [FK_T_Protein_Collection_Members_T_Protein_Names] FOREIGN KEY 
	(
		[Reference_ID]
	) REFERENCES [dbo].[T_Protein_Names] (
		[Reference_ID]
	)
GO

ALTER TABLE [dbo].[T_Position_Info] ADD 
	CONSTRAINT [FK_T_Position_Info_T_DNA_Structures] FOREIGN KEY 
	(
		[DNA_Structure_ID]
	) REFERENCES [dbo].[T_DNA_Structures] (
		[DNA_Structure_ID]
	),
	CONSTRAINT [FK_T_Position_Info_T_Proteins] FOREIGN KEY 
	(
		[Protein_ID]
	) REFERENCES [dbo].[T_Proteins] (
		[Protein_ID]
	),
	CONSTRAINT [FK_T_Position_Info_T_Reading_Frame_Types] FOREIGN KEY 
	(
		[Reading_Frame_Type_ID]
	) REFERENCES [dbo].[T_Reading_Frame_Types] (
		[Reading_Frame_Type_ID]
	)
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Authority_Picker
AS
SELECT     Authority_ID, Name, Description + ' <' + Web_Address + '>' AS Details
FROM         dbo.T_Naming_Authorities

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Naming_Authorities"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 130
               Right = 190
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 4
         Width = 284
         Width = 1440
         Width = 1440
         Width = 6090
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4050
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Authority_Picker'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Authority_Picker'

GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Organism_Picker
AS
SELECT     Organism_ID, Short_Name, Short_Name + ' - ' + Description AS Display_Name
FROM         dbo.T_Organisms

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Organisms"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 121
               Right = 274
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2715
         Alias = 1155
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Organism_Picker'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Organism_Picker'

GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collections_By_Organism
AS
SELECT     dbo.T_Protein_Collections.Protein_Collection_ID, dbo.T_Protein_Collections.FileName, dbo.T_Protein_Collections.Description, 
                      dbo.V_Protein_Collection_Organism.Organism_ID, dbo.V_Protein_Collection_Organism.Authority_ID, dbo.T_Protein_Collections.Collection_State_ID, 
                      dbo.T_Protein_Collections.NumProteins
FROM         dbo.T_Protein_Collections INNER JOIN
                      dbo.V_Protein_Collection_Organism ON dbo.T_Protein_Collections.Protein_Collection_ID = dbo.V_Protein_Collection_Organism.Protein_Collection_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Protein_Collections"
            Begin Extent = 
               Top = 30
               Left = 204
               Bottom = 231
               Right = 427
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_Protein_Collection_Organism"
            Begin Extent = 
               Top = 9
               Left = 476
               Bottom = 118
               Right = 737
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 8
         Width = 284
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Protein_Collections_By_Organism'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Protein_Collections_By_Organism'

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


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Protein_Collection_Members"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 157
               Right = 246
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_Protein_Names"
            Begin Extent = 
               Top = 6
               Left = 276
               Bottom = 158
               Right = 494
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Protein_Collection_Organism'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Protein_Collection_Organism'

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


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Protein_Collection_Members"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 154
               Right = 293
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_Protein_Names"
            Begin Extent = 
               Top = 7
               Left = 424
               Bottom = 159
               Right = 604
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_Proteins"
            Begin Extent = 
               Top = 165
               Left = 424
               Bottom = 359
               Right = 605
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 6
         Width = 284
         Width = 1440
         Width = 3795
         Width = 5625
         Width = 1440
         Width = 1440
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Protein_Database_Export'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Protein_Database_Export'

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


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Protein_Collection_Members"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 121
               Right = 222
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_Protein_Names"
            Begin Extent = 
               Top = 2
               Left = 346
               Bottom = 154
               Right = 498
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_Proteins"
            Begin Extent = 
               Top = 164
               Left = 340
               Bottom = 360
               Right = 513
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 12
         Width = 284
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Protein_Storage_Entry_Import'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Protein_Storage_Entry_Import'

GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.V_Protein_Collection_Count_By_Organism
AS
SELECT     TOP 100 PERCENT dbo.T_Organisms.Organism_ID, dbo.V_Protein_Collection_Organism.Protein_Collection_ID
FROM         dbo.V_Protein_Collection_Organism INNER JOIN
                      dbo.T_Organisms ON dbo.V_Protein_Collection_Organism.Organism_ID = dbo.T_Organisms.Organism_ID
GROUP BY dbo.T_Organisms.Organism_ID, dbo.V_Protein_Collection_Organism.Protein_Collection_ID
ORDER BY dbo.T_Organisms.Organism_ID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


exec sp_addextendedproperty N'MS_DiagramPane1', N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_Organisms"
            Begin Extent = 
               Top = 52
               Left = 558
               Bottom = 167
               Right = 794
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_Protein_Collection_Organism"
            Begin Extent = 
               Top = 6
               Left = 312
               Bottom = 120
               Right = 496
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 3
         Width = 284
         Width = 1440
         Width = 1440
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', N'user', N'dbo', N'view', N'V_Protein_Collection_Count_By_Organism'
GO
exec sp_addextendedproperty N'MS_DiagramPaneCount', 1, N'user', N'dbo', N'view', N'V_Protein_Collection_Count_By_Organism'

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
			Collection_State_ID,
			NumProteins,
			NumResidues,
			DateCreated,
			DateModified
		) VALUES (
			@fileName, 
			@Description, 
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
			NumProteins = @numProteins,
			NumResidues = @numResidues,
			DateModified = GETDATE()
		
		WHERE (FileName = @fileName)
		
		SELECT @Collection_ID = @@Identity
			
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
	
	commit transaction @transName
		
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
	
	execute @Reference_ID = get_protein_reference_id @name, @description
	
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
		DateAdded
	) VALUES (
		@name, 
		@description,
		@organism_ID, 
		@authority_ID,
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
	
	if @Protein_ID = 0 and @mode = 'update'
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
	@description varchar(900)
)
As
	declare @reference_ID int
	set @reference_ID = 0
	
	SELECT @reference_ID = Reference_ID FROM T_Protein_Names
	 WHERE ([Name] = @name)
	 
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


exec sp_addextendedproperty N'MS_Description', N'The full, scientific name of the species considered (e.g. Escherichia coli)', N'user', N'dbo', N'table', N'T_Organisms', N'column', N'DMS_Name'
GO
exec sp_addextendedproperty N'MS_Description', N'Unique Organism ID', N'user', N'dbo', N'table', N'T_Organisms', N'column', N'Organism_ID'
GO
exec sp_addextendedproperty N'MS_Description', N'Shorter name used for internal naming,etc. (e.g. ''e_coli'')', N'user', N'dbo', N'table', N'T_Organisms', N'column', N'Short_Name'
GO
exec sp_addextendedproperty N'MS_Description', N'Gives the UNC path to the folder where archival versions of the database are stored', N'user', N'dbo', N'table', N'T_Organisms', N'column', N'Storage_Location'


GO


exec sp_addextendedproperty N'MS_Description', N'Number of Amino Acid Residues in this protein', N'user', N'dbo', N'table', N'T_Proteins', N'column', N'Length'


GO


exec sp_addextendedproperty N'MS_Description', N'Gene-containing molecules (generic term for Chromosome, Plasmid, etc.)', N'user', N'dbo', N'table', N'T_DNA_Structures', N'column', N'DNA_Structure_ID'


GO

