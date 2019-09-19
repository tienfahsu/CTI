EXEC sp_MSForEachTable 'TRUNCATE TABLE ?'

EXEC sp_MSforeachtable 'if PARSENAME("?",1) like ''%CertainString%'' DROP TABLE ?'
