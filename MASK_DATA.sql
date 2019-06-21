-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		alpha hsu	
-- Create date: 2019/06/21
-- Description:	MASK DATA
-- =============================================
ALTER FUNCTION MASK_DATA
(
	-- Add the parameters for the function here
	 @PIIDATA varchar(512),
	 @PIITYPE INT

)
RETURNS VARCHAR(512)
AS
BEGIN
   
	-- Declare the return variable here
	DECLARE @RETDATA VARCHAR(512)	 
	DECLARE @DATALEN INT

	SET @RETDATA = REPLACE(RTRIM(LTRIM(@PIIDATA)),'　','')
	SET @DATALEN = LEN(@RETDATA)
	IF @PIITYPE =1
	BEGIN
	  -- NAME
	  IF @DATALEN > 2
	     SET @RETDATA =LEFT(@RETDATA,1) + REPLICATE('X',@DATALEN-2)+ RIGHT(@RETDATA,1)
	  ELSE
	     SET @RETDATA =LEFT(@RETDATA,1) + 'X'

	END
	ELSE IF @PIITYPE = 2
	BEGIN
	  
	    --CHAR(ASCII('0')+17)
		 
	   IF @DATALEN > 5
	   BEGIN
	     DECLARE @RETTMP VARCHAR(512)	 
		 DECLARE @ILEN INT,@POS INT = 1
	     SET @RETTMP =SUBSTRING(@RETDATA,4,@DATALEN-3)
		 SET @ILEN =LEN(@RETTMP)
		  
	     SET @RETDATA =LEFT(@RETDATA,3) --+ REPLICATE('*',@DATALEN-5) + RIGHT(@RETDATA,2)
		 WHILE @POS < @ILEN - 2
		 BEGIN
			   SET @RETDATA =@RETDATA + CHAR(ASCII(@POS)+49)
			   SET @POS=@POS+1
		 END		 
   	     SET @RETDATA = @RETDATA + RIGHT(@RETTMP,2) 
	   END
	   ELSE  IF @DATALEN > 2	     
	     SET @RETDATA =LEFT(@RETDATA,1) + REPLICATE('*',@DATALEN-2) + RIGHT(@RETDATA,1)
	   ELSE 
	     SET @RETDATA = @RETDATA +  REPLICATE('*',10 - @DATALEN)

	END
	ELSE IF @PIITYPE = 3
	BEGIN
	    -- ACCOUNT 
	   IF @DATALEN > 7
	     SET @RETDATA =LEFT(@RETDATA,3) + REPLICATE('*',@DATALEN-7) + RIGHT(@RETDATA,4)
	   ELSE  IF @DATALEN > 5
	     SET @RETDATA =LEFT(@RETDATA,2) + REPLICATE('*',@DATALEN-5) + RIGHT(@RETDATA,3)
	   ELSE 
	     SET @RETDATA = @RETDATA + REPLICATE('*',16 - @DATALEN)
	END
	ELSE IF @PIITYPE = 4
	BEGIN
	    -- TEL  MOBILE
	   IF @DATALEN > 5
	      SET @RETDATA = LEFT(@RETDATA,@DATALEN - 5) + REPLICATE('*',4) +  RIGHT(@RETDATA,1)	 
	   ELSE 
	     SET @RETDATA = @RETDATA + REPLICATE('*',4)
	END
	ELSE 
	BEGIN
	  IF @DATALEN > 2
		  SET @RETDATA = LEFT(@RETDATA,1) + REPLICATE('X',@DATALEN-2) + RIGHT(@RETDATA,1)
	  ELSE
	      SET @RETDATA =REPLICATE('X',2)
	END
	RETURN @RETDATA
END
GO

