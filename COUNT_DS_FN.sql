USE [COGDB2]
GO
/****** Object:  UserDefinedFunction [dbo].[COUNT_DS_FN]    Script Date: 2019/9/16 下午 06:02:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[COUNT_DS_FN]
(
	@TB NVARCHAR(100),@DATEAS DATE
)
RETURNS INT 
AS
BEGIN

	DECLARE  @CNT INT

	DECLARE @NEW_TB VARCHAR(100)
	DECLARE @DT DATE = GETDATE()-1
	SET @NEW_TB = @TB+CASE WHEN @DATEAS = @DT AND @TB NOT IN ('AS_CIR','CCH','GLDETAIL','IDRP_BASE','CLIENT_MASTER','PROD_CCH')  THEN '_NEW'
						WHEN MONTH(@DATEAS) = MONTH(@DT)  THEN ''
						WHEN MONTH(@DATEAS) <> MONTH(@DT) AND @TB NOT LIKE 'CUS_%'   THEN '_'+RIGHT('0'+CAST(MONTH(@DATEAS) AS VARCHAR(2)),2)						 
						ELSE ''
				END

	DECLARE @SQL NVARCHAR(4000) 
	
		SET @SQL = 'SELECT @retvalOUT = Count(*) FROM COGDB3.dbo.' + @NEW_TB +' DS '
					+ CASE  WHEN @TB NOT IN ('AS_CIR','CCH', 'GLDETAIL','IDRP_BASE','CLIENT_MASTER','PROD_CCH') THEN ' WHERE MIS_DATE ='''+CONVERT(VARCHAR(10),@DATEAS,120)+''' ' ELSE '' END 
		

		--PRINT @SQL
		EXECUTE SP_EXECUTESQL @SQL,N'@retvalOUT int OUTPUT',@retvalOUT=@CNT OUTPUT
		
	RETURN @CNT
END


