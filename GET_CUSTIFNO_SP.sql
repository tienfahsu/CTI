USE [COGDB2]
GO
/****** Object:  StoredProcedure [dbo].[GET_CUSTIFNO_SP]    Script Date: 2019/7/5 上午 09:42:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Dylan
-- Create date: 2018/08/02
-- Description:	Get Custom Information (check 10 days data)
--
-- =============================================
ALTER procedure [dbo].[GET_CUSTIFNO_SP]
	 (@PKey varchar(16))--傳入值
AS
BEGIN  
----------------------------------------------
--變數區//宣告在這個SP中會用到的變數
----------------------------------------------
--DEBUG
--DECLARE @PKey VARCHAR(16) = '557XXXXXX507'
--END DEBUG
	IF(OBJECT_ID('TEMPDB..#CUST_INFO')IS NOT NULL) 
	BEGIN 
		TRUNCATE TABLE #CUST_INFO 
	END
	ELSE
	BEGIN
		CREATE TABLE #CUST_INFO
		(
			IS_EXIST_FG VARCHAR(1)
			,CUST_ID varchar(10)
			,CUST_NAME Nvarchar(30)
			,CUST_ACCTNMBR VARCHAR(16)
			,CUST_CARDNMBR VARCHAR(16)
			,CUST_CACS_STATE VARCHAR(3)
			,CUST_CDC VARCHAR(2)
			,CUST_DUE_DAY VARCHAR(10)
			,CUST_LOC_CODE VARCHAR(6)
			,AC_TYPE VARCHAR(3)
			,CYCLEID INT
			,RESPONCOLL VARCHAR(8)
		)
	END

	 
	DECLARE @MESSAGE NVARCHAR(500) = '', @Status bit= 0, @RECNO as INT
	
	DECLARE @v_IS_EXIST_FG AS VARCHAR(1)

	DECLARE @CUST_ID as VARCHAR(10)
		,@CUST_NAME AS VARCHAR(30)
		,@CUST_ACCTNMBR AS VARCHAR(16)
		,@CUST_CARDNMBR AS VARCHAR(16)
		,@CUST_CACS_STATE AS VARCHAR(3)	
		,@CUST_CDC AS VARCHAR(2)
		,@CUST_DUE_DAY AS VARCHAR(10)
		,@CUST_LOC_CODE AS VARCHAR(6)
		,@AC_TYPE AS VARCHAR(3)
		,@CYCLEID INT
		,@RESPONCOLL VARCHAR(8)
	DECLARE @WHERE NVARCHAR(500) = ''
	DECLARE @V_MIS_DATE DATE, @V_CNT_DAY INT = 0

	IF OBJECT_ID('tempdb..#V_CARDS_FN') IS NOT NULL DROP TABLE #V_CARDS_FN	
	IF OBJECT_ID('tempdb..#V_BUDTRAN_FN') IS NOT NULL DROP TABLE #V_BUDTRAN_FN
	IF OBJECT_ID('tempdb..#V_REC_COMBO_FN') IS NOT NULL DROP TABLE #V_REC_COMBO_FN

	SELECT TOP 0 * INTO #V_CARDS_FN FROM dbo.DS_CARDS_VW
	SELECT TOP 0 * INTO #V_BUDTRAN_FN FROM dbo.DS_BUDTRAN_VW
	SELECT TOP 0 * INTO #V_REC_COMBO_FN FROM dbo.DS_REC_COMBO_VW

----------------------------------------------
--執行區----依據傳入CUST_ID 將查詢結果INSERT至@CUST_INFO
----------------------------------------------	
    SET @v_IS_EXIST_FG='N'	
	SET @PKey= LTRIM(RTRIM(@PKey))
	

	DECLARE @M_DATE VARCHAR(8)
	,@OUTPUT NVARCHAR(100) = ''
	,@G_MIS_DATE DATE
	DECLARE @DT_CNT DATE

    -- 10天內的檔案  (mis_date < GETDATE())
	--SET @v_IS_EXIST_FG = NULL
    SET @V_CNT_DAY = 1
    WHILE( @V_CNT_DAY <= 10 )
    BEGIN
        SET @DT_CNT = DATEADD(DAY, -@V_CNT_DAY, GETDATE())

		SET @WHERE = 'WHERE MIS_DATE = '''+ CONVERT(VARCHAR(10), @DT_CNT ,111)+''''
				+ ' AND ' + IIF(LEN(@Pkey)=10, 'CUSTID' ,  'CARDNMBR') + ' = '''+@PKEY+''''
		
		--檢查CARDS DataSet 是否Ready	
		--EXEC COGDB3.dbo.SP_GetTableName 'CARDS', @M_DATE ,@OUTPUT output

		--IF SUBSTRING(@OUTPUT,1,1) = 'S'
		--BEGIN
			INSERT INTO #V_CARDS_FN EXECUTE GET_DS_SP 'CARDS',@DT_CNT, @WHERE , @ORDER= 'MIS_DATE'

			IF EXISTS(SELECT 1 FROM #V_CARDS_FN)
			BEGIN
				SET @v_IS_EXIST_FG = 'C'
				SELECT TOP 1 @CUST_ID= LTRIM(RTRIM(CUSTID)) 
					,@CUST_NAME= LTRIM(RTRIM(CUSTNAME))
					,@CUST_ACCTNMBR= LTRIM(RTRIM(ACCTNMBR))
					,@CUST_CARDNMBR= LTRIM(RTRIM(CARDNMBR))
					,@CUST_CACS_STATE= LTRIM(RTRIM(CACS_STATE))
					,@CUST_CDC= STR(CDC,2)
					,@CUST_DUE_DAY= RIGHT('00'+LTRIM(RTRIM(STR(CYCLEID))),2)+'/'+RIGHT('00'+LTRIM(RTRIM(STR(day(date_due)))),2)
					,@CUST_LOC_CODE= LTRIM(RTRIM(LOCATION))
					,@AC_TYPE = dbo.ALLTRIM(logo)
					,@CYCLEID = IIF(CAST(CYCLEID As INT)<=2,1,0)
					,@RESPONCOLL = responcoll
				FROM #V_CARDS_FN
				BREAK
			END

		--END
		
		--檢查BUDTRAN DataSet 是否Ready	
		--EXEC COGDB3.dbo.SP_GetTableName 'BUDTRAN', @M_DATE ,@OUTPUT output
		
		--IF SUBSTRING(@OUTPUT,1,1) = 'S'
		--BEGIN
			INSERT INTO #V_BUDTRAN_FN EXECUTE GET_DS_SP 'BUDTRAN',@DT_CNT, @WHERE , @ORDER= 'MIS_DATE'

			IF  EXISTS (SELECT 1 FROM #V_BUDTRAN_FN) 
			BEGIN
				SET @v_IS_EXIST_FG = 'B'
				SELECT TOP 1 @CUST_ID= LTRIM(RTRIM(CUSTID))
					,@CUST_NAME= LTRIM(RTRIM(CUSTNAME))
					,@CUST_ACCTNMBR= LTRIM(RTRIM(ACCTNMBR))
					,@CUST_CARDNMBR= LTRIM(RTRIM(CARDNMBR))
					,@CUST_CACS_STATE= LTRIM(RTRIM(CACS_STATE))
					,@CUST_CDC= STR(CDC,2)
					,@CUST_DUE_DAY= RIGHT('00'+LTRIM(RTRIM(STR(CYCLEID))),2)+'/'+RIGHT('00'+LTRIM(RTRIM(STR(day(CYCLEID)))),2)
					,@CUST_LOC_CODE= LTRIM(RTRIM(LOCATION))	
					,@AC_TYPE = dbo.ALLTRIM(ACCTTYPE)	
					,@CYCLEID = IIF(CAST(CYCLEID As INT)<=2,1,0)
					,@RESPONCOLL = responcoll
				FROM #V_BUDTRAN_FN		
				BREAK
			END
		--END
		
		--檢查REC_COMBO DataSet 是否Ready	
		--EXEC COGDB3.dbo.SP_GetTableName 'REC_COMBO', @M_DATE ,@OUTPUT output

		--IF SUBSTRING(@OUTPUT,1,1) = 'S'
		--BEGIN
			INSERT INTO #V_REC_COMBO_FN EXECUTE GET_DS_SP 'REC_COMBO',@DT_CNT, @WHERE , @ORDER= 'MIS_DATE'
			IF EXISTS (SELECT 1 FROM #V_REC_COMBO_FN)
			BEGIN	
				SET @v_IS_EXIST_FG='R'
				SELECT TOP 1 @CUST_ID= LTRIM(RTRIM(CUSTID))
					,@CUST_NAME= LTRIM(RTRIM(CUSTNAME))
					,@CUST_ACCTNMBR= LTRIM(RTRIM(ACCTNMBR))
					,@CUST_CARDNMBR= LTRIM(RTRIM(CARDNMBR))
					,@CUST_CACS_STATE= LTRIM(RTRIM(CACS_STATE))
					,@CUST_CDC= '9'
					,@CUST_DUE_DAY= CAST(NULL AS VARCHAR(5))
					,@CUST_LOC_CODE= LTRIM(RTRIM(LOCATION))
					,@AC_TYPE = dbo.ALLTRIM(IIF(ISNULL(LOGO,'') = '',ACCTTYPE,LOGO))
					,@CYCLEID = 0
					,@RESPONCOLL = responcoll
				FROM #V_REC_COMBO_FN
				BREAK
			END
		--END

		 
        SET @V_CNT_DAY = (@V_CNT_DAY + 1)
    END -- WHILE

	
	 
	

	

----------------------------------------------
--回傳區
----------------------------------------------

	INSERT INTO #CUST_INFO(
		IS_EXIST_FG
		,CUST_ID
		,CUST_NAME
		,CUST_ACCTNMBR
		,CUST_CARDNMBR
		,CUST_CACS_STATE
		,CUST_CDC
		,CUST_DUE_DAY
		,CUST_LOC_CODE
		,AC_TYPE
		,CYCLEID
		,RESPONCOLL)
	VALUES(
		@v_IS_EXIST_FG
		,@CUST_ID
		,@CUST_NAME
		,@CUST_ACCTNMBR
		,@CUST_CARDNMBR
		,@CUST_CACS_STATE
		,@CUST_CDC
		,@CUST_DUE_DAY
		,@CUST_LOC_CODE
		,@AC_TYPE
		,@CYCLEID
		,@RESPONCOLL)

	--RAISERROR('TABLE NOT READY',16,1)
	--SET @STATUS =1
	--IF @v_IS_EXIST_FG='N'
	--BEGIN
	--	SET @MESSAGE= @MESSAGE+CHAR(13)+'查無此卡號資料在所有的DATA SET中'
	--END
	--ELSE
	--BEGIN		
	--	SET @MESSAGE = @MESSAGE + CHAR(13) +'Process Complete !'
	--	INSERT INTO @CUST_INFO(
	--		IS_EXIST_FG
	--		,CUST_ID
	--		,CUST_NAME
	--		,CUST_ACCTNMBR
	--		,CUST_CARDNMBR
	--		,CUST_CACS_STATE
	--		,CUST_CDC
	--		,CUST_DUE_DAY
	--		,CUST_LOC_CODE)
	--	VALUES(
	--		@v_IS_EXIST_FG
	--		,@CUST_ID
	--		,@CUST_NAME
	--		,@CUST_ACCTNMBR
	--		,@CUST_CARDNMBR
	--		,@CUST_CACS_STATE
	--		,@CUST_CDC
	--		,@CUST_DUE_DAY
	--		,@CUST_LOC_CODE)
	--END
	--INSERT INTO #CUST_INFO
 --   SELECT * FROM @CUST_INFO--回傳
END
 


