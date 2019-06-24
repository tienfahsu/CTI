USE MORTGAGE2
GO
-- Truncate the log by changing the database recovery model to SIMPLE.  
ALTER DATABASE MORTGAGE2
SET RECOVERY SIMPLE; 
GO
-- Shrink the truncated log file to 1 MB.  
DBCC SHRINKFILE (MORTGAGE2_log, 1);  
GO
-- Reset the database recovery model.  
ALTER DATABASE MORTGAGE2
SET RECOVERY FULL; 
GO	
