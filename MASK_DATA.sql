  
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
    DECLARE @RETTMP VARCHAR(512),@CHARITEM INT	 
	DECLARE @ILEN INT,@EXIDX INT,@POS INT = 1
 
	SET @RETDATA = RTRIM(LTRIM(REPLACE(ISNULL(@PIIDATA,''),'　',' ')))
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
	    --ID
	    --CHAR(ASCII('0')+17)
		 
	   IF @DATALEN > 5
	   BEGIN
		 SET @EXIDX=0
	     SET @RETTMP = SUBSTRING(@RETDATA,4,@DATALEN-3)
		 SET @ILEN =LEN(@RETTMP)
		  
	     SET @RETDATA = LEFT(@RETDATA,3) --+ REPLICATE('*',@DATALEN-5) + RIGHT(@RETDATA,2)
		 WHILE @POS <= @ILEN - 2
		 BEGIN
			   SET @CHARITEM = ASCII(SUBSTRING(@RETTMP,@POS,1))
			   IF @CHARITEM >= 48 AND @CHARITEM <= 57
			       SET @RETDATA =@RETDATA + CHAR(ASCII(@POS)+49+@EXIDX*10)
			   ELSE 
				   SET @RETDATA =@RETDATA + CHAR(@CHARITEM);
			   SET @POS=@POS+1
			   
			   IF @EXIDX < 1 SET @EXIDX=@EXIDX+1
			   ELSE  SET @EXIDX=0
		 END		 
   	     SET @RETDATA = @RETDATA + RIGHT(@RETTMP,2) 
	   END
	   ELSE  
	     SET @RETDATA = @RETDATA + REPLICATE('*',10 - @DATALEN)

	END
	ELSE IF @PIITYPE = 3
	BEGIN
	    -- ACCOUNT 
	   IF @DATALEN > 10
	   BEGIN
	     SET @EXIDX=0
	     SET @RETTMP = SUBSTRING(@RETDATA,9,@DATALEN-8)
		 SET @ILEN =LEN(@RETTMP)		  
	     SET @RETDATA =LEFT(@RETDATA,8) --+ REPLICATE('*',@DATALEN-5) + RIGHT(@RETDATA,2)
		 WHILE @POS <= @ILEN - 2
		 BEGIN
			   SET @CHARITEM = ASCII(SUBSTRING(@RETTMP,@POS,1))
			   IF @CHARITEM >= 48 AND @CHARITEM <= 57
			       SET @RETDATA =@RETDATA + CHAR(ASCII(@POS)+49+@EXIDX*10)
			   ELSE 
				   SET @RETDATA =@RETDATA + CHAR(@CHARITEM);
			   SET @POS=@POS+1
			   IF @EXIDX < 1 SET @EXIDX=@EXIDX+1
			   ELSE  SET @EXIDX=0
		 END		 
   	     SET @RETDATA = @RETDATA + RIGHT(@RETTMP,2) 
	   END
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
	  IF @DATALEN > 6
		  SET @RETDATA = LEFT(@RETDATA,6) + REPLICATE('X',@DATALEN-7) + RIGHT(@RETDATA,1)
	  ELSE IF @DATALEN > 3
	      SET @RETDATA = LEFT(@RETDATA,3) + REPLICATE('X',@DATALEN-4) + RIGHT(@RETDATA,1)
	  ELSE 
	      SET @RETDATA =  REPLICATE('X',@DATALEN)
	END
	RETURN @RETDATA
END
GO
