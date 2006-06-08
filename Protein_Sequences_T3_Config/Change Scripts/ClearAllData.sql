
Delete from T_Protein_Collection_Members
go

dbcc checkident (T_Protein_Collection_Members, RESEED, 0)
go

DELETE FROM T_Protein_Names
go

dbcc CHECKIDENT (T_Protein_Names, RESEED, 1000)
go

DELETE FROM T_Annotation_Groups
GO

DBCC CHECKIDENT (T_Annotation_Groups, RESEED, 0)
go

DELETE FROM T_Proteins
GO

DBCC CHECKIDENT (T_Proteins, RESEED, 0)
GO

Delete from T_Protein_Collections
go

dbcc checkident (T_Protein_Collections, reseed, 1000)
GO



DELETE FROM T_Protein_Collections_By_Organism
go

DELETE FROM T_Archived_Output_File_Collections_XRef
go

dbcc checkident (T_Archived_Output_File_Collections_XRef, RESEED, 0)
go

DELETE FROM T_Archived_Output_Files
go

dbcc checkident (T_Archived_Output_Files, RESEED, 1000)
go


