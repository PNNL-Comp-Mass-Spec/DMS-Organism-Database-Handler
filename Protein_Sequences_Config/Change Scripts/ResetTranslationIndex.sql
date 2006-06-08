DELETE FROM T_DNA_Translation_Tables
GO

DBCC CHECKIDENT (T_DNA_Translation_Tables, RESEED, 0)
GO

DELETE FROM T_DNA_Translation_Table_Members
GO

DBCC CHECKIDENT (T_DNA_Translation_Table_Members, RESEED, 0)
GO
