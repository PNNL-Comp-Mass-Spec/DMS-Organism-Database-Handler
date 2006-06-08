

DELETE FROM T_Archived_Output_File_Collections_XRef
go

dbcc checkident (T_Archived_Output_File_Collections_XRef, RESEED, 0)
go

DELETE FROM T_Archived_File_Creation_Options
go

dbcc checkident (T_Archived_File_Creation_Options, RESEED, 0)
go

Delete from T_Archived_Output_Files
go

dbcc checkident (T_Archived_Output_Files, RESEED, 1000)
go
