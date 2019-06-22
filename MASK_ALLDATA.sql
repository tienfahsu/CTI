ALTER PROCEDURE [dbo].[MASK_ALLDATA]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
		DECLARE @TABLE_CATALOG nvarchar(128),@TABLE_SCHEMA nvarchar(128),@TABLE_NAME nvarchar(128)
		DECLARE @COLUMN_NAME nvarchar(128),@CHARACTER_MAXIMUM_LENGTH INT
		DECLARE @sqlCommand nvarchar(MAX),@columnList nvarchar(MAX)

		DECLARE CUR_TABLES CURSOR FOR 
		SELECT TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME 
		FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'
		OPEN CUR_TABLES
		FETCH NEXT FROM CUR_TABLES INTO @TABLE_CATALOG, @TABLE_SCHEMA ,@TABLE_NAME 
		WHILE @@FETCH_STATUS = 0  
		BEGIN
			SET @columnList=''
			DECLARE CUR_COLUMNS CURSOR FOR  SELECT  COLUMN_NAME,CHARACTER_MAXIMUM_LENGTH 
			FROM INFORMATION_SCHEMA.COLUMNS
			WHERE TABLE_NAME=@TABLE_NAME AND TABLE_SCHEMA =@TABLE_SCHEMA AND DATA_TYPE IN ('varchar','char','nvarchar')
			OPEN CUR_COLUMNS
			FETCH NEXT FROM CUR_COLUMNS INTO @COLUMN_NAME, @CHARACTER_MAXIMUM_LENGTH
			WHILE @@FETCH_STATUS = 0  
			BEGIN
				IF @COLUMN_NAME IN ('acctnmbr','cardnmbr','cust_nbr','clnt_nbr','fmsacctno','reln_nbr','cardnmbr_o','RW_ACCTBR','RWACCT_B','RWACT_CARD','ACCT')
				BEGIN
					IF LEN(@columnList) >0 SET @columnList=@columnList+','
					SET @columnList=@columnList + @COLUMN_NAME +'=dbo.MASK_DATA('+@COLUMN_NAME+',3)'
				END
				ELSE IF @COLUMN_NAME IN ('custid','id_tag','IDN_BAN','idno','idn','IDNO_O')
				BEGIN
					IF LEN(@columnList) >0 SET @columnList=@columnList+','
					SET @columnList=@columnList + @COLUMN_NAME +'=dbo.MASK_DATA('+@COLUMN_NAME+',2)'
				END
				ELSE IF @COLUMN_NAME IN ('custname','cust_name','CNAME') OR ((@TABLE_NAME LIKE 'Payment_All_Detail%' OR @TABLE_NAME LIKE 'URL_Z%' OR @TABLE_NAME LIKE 'Z4%') AND @COLUMN_NAME='NAME')
				BEGIN
				   IF LEN(@columnList) >0 SET @columnList=@columnList+','
				   SET @columnList=@columnList + @COLUMN_NAME +'=dbo.MASK_DATA('+@COLUMN_NAME+',1)'
				END
				ELSE IF @COLUMN_NAME IN ('tel_buss','tel_home','tel_mobl','tel_othr','TEL') 
				BEGIN
				   IF LEN(@columnList) >0 SET @columnList=@columnList+','
				   SET @columnList=@columnList + @COLUMN_NAME +'=dbo.MASK_DATA('+@COLUMN_NAME+',4)'
				END
                                ELSE IF @COLUMN_NAME IN ('ADDR')
				BEGIN
				    IF LEN(@columnList) >0 SET @columnList=@columnList+','
				    SET @columnList=@columnList + @COLUMN_NAME +'=dbo.MASK_DATA('+@COLUMN_NAME+',5)'
				END
			 


				FETCH NEXT FROM CUR_COLUMNS INTO @COLUMN_NAME, @CHARACTER_MAXIMUM_LENGTH
			END
			CLOSE CUR_COLUMNS
			DEALLOCATE CUR_COLUMNS

			IF LEN(@columnList) > 0
			BEGIN
				SET @sqlCommand= 'UPDATE '+ @TABLE_SCHEMA+'.'+@TABLE_NAME + ' SET ' + @columnList
				BEGIN TRY
				    EXECUTE sp_executesql @sqlCommand 
				END TRY
				BEGIN CATCH  
				   PRINT @sqlCommand
				END CATCH
			END
  
			FETCH NEXT FROM CUR_TABLES INTO @TABLE_CATALOG, @TABLE_SCHEMA ,@TABLE_NAME 
		END
		CLOSE CUR_TABLES
		DEALLOCATE CUR_TABLES
END
