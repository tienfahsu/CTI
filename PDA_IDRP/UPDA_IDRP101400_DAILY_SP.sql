-- =====================================================================================================
-- Author        : alpha hsu
-- Create date   : 2019/06/17
-- SYSCode       : PDA_IDRP
-- Function Name : 確認案件是否符合提前結清
-- SA/SD Document:  
-- Description   :確認案件是否符合提前結清
-- =====================================================================================================
ALTER PROCEDURE [dbo].[UPDA_IDRP101400_DAILY_SP] 
-------------------------------------
---- 以下的變數是由外面的程式傳進來的
-------------------------------------
AS
BEGIN

----------------------------------------------
--變數區//宣告在這個SP中會用到的變數
----------------------------------------------
DECLARE @MESSAGE NVARCHAR(500) = '', @STATUS BIT=	 0
DECLARE @SEQNO INT,
		@CUSTID VARCHAR(10),
		@SETTLE_AMT numeric(18, 0),
		@COLLECTOR_ID varchar(10), 
		@ACCTNMBR varchar(16),
		@TOT_AMT  numeric(18, 0),
		@CCH_RECORD varchar(254)

IF OBJECT_ID('tempdb..#PDA_IDRP_SETTLE') IS NOT NULL DROP TABLE #PDA_IDRP_SETTLE
 
----------------------------------------------
--執行區
----------------------------------------------	
--檢核區間 申請日 ~ 結清日+30天 +1 AND 還未符合提前結清
DECLARE ST_PAYREC CURSOR FOR 
				WITH SETTLE_TOT_AMT
				AS
				(
					SELECT A.SEQNO,A.CUSTID,A.SETTLE_AMT,A.COLLECTOR_ID,A.ACCTNMBR,  
					ISNULL((SELECT SUM(P.rem_amt) FROM COGDB3.dbo.CUS_IDRP_PAYMENTREC P WHERE P.CUSTID=A.CUSTID AND CAST(P.rec_date AS date) BETWEEN cast(A.CREATE_DATE as date) AND CAST(DATEADD(d,31,A.SETTLE_DATE) AS DATE)),0) AS TOT_AMT
					FROM [dbo].[UPDA_IDRP_SETTLE] A WHERE A.IF_MATCH=0-- AND A.SETTLE_AMT > 0
				)
				SELECT SEQNO,CUSTID,SETTLE_AMT,COLLECTOR_ID,ACCTNMBR,TOT_AMT FROM SETTLE_TOT_AMT WHERE (TOT_AMT BETWEEN SETTLE_AMT * 0.9 AND SETTLE_AMT * 1.1)

--UPDATE A SET A.IF_MATCH =1 FROM [dbo].[UPDA_IDRP_SETTLE] A INNER JOIN SETTLE_TOT_AMT B ON A.SEQNO=B.SEQNO AND (B.TOT_AMT BETWEEN B.SETTLE_AMT * 0.9 AND B.SETTLE_AMT * 1.1)
OPEN ST_PAYREC
FETCH NEXT FROM ST_PAYREC INTO @SEQNO ,@CUSTID, @SETTLE_AMT, @COLLECTOR_ID,@ACCTNMBR,@TOT_AMT
	                             
WHILE(  @@FETCH_STATUS = 0 )
BEGIN
	-- UPDATE IF_MATCH 符合提前結清
	UPDATE [dbo].[UPDA_IDRP_SETTLE] SET IF_MATCH =1 WHERE SEQNO = @SEQNO

	SET @CCH_RECORD='CHECK CUS ' +CONVERT(varchar(10),GETDATE(),111) + ' (CUS IBRS date) '+ FORMAT(@TOT_AMT,'$##,##0') + ' (CUS payment)' 

	--UPDATE CCH record
	INSERT INTO [COGDB3].[dbo].[UPLOAD_CACS](MIS_DATE,[TIMESTAMP],BATCH,[USER_ID],CARDNMBR,TXN_CODE,TXTLINE1,TXTLINE2)
	VALUES(GETDATE(),GETDATE(),'PDA',@COLLECTOR_ID,@ACCTNMBR,'PDA',@CCH_RECORD,'')

	FETCH NEXT FROM ST_PAYREC INTO @SEQNO ,@CUSTID, @SETTLE_AMT, @COLLECTOR_ID,@ACCTNMBR,@TOT_AMT
END
CLOSE ST_PAYREC
DEALLOCATE ST_PAYREC

-- Tracer reports
--1)	捉Supervisor 確認OK後的三天內的CCH 中是否有OC/IC ?
-- Check OC/IC SET IF_OCIC=1 WHERE IF_OCIC = 0
UPDATE A SET IF_OCIC = 1
FROM [dbo].[UPDA_IDRP_SETTLE] A
INNER JOIN [dbo].[DS_CCH_VW] B ON B.CUSTID =A.CUSTID AND B.ac_cch IN ('IC','OC') AND CAST(B.date_cch AS DATE) > CAST(A.SUBMIT_DATE AS DATE) AND CAST(B.date_cch AS DATE) <= CAST(DATEADD(day,3,A.SUBMIT_DATE) AS DATE)
WHERE A.SUBMIT_DATE IS NOT NULL
 

--2)	collector送出後超過三天status仍在審未審核

SELECT * INTO #PDA_IDRP_SETTLE FROM UPDA_IDRP_SETTLE WHERE (TRACER_RPT & 1 = 0 AND IF_OCIC=1) OR (TRACER_RPT & 2 = 0 AND DATEDIFF(day,CREATE_DATE,GETDATE()) > 3 AND STATUS_CODE = '1')  

IF EXISTS(SELECT 1 FROM #PDA_IDRP_SETTLE)
BEGIN

-- SET IF_TRACER 剔除在下次tracer report 內
UPDATE UPDA_IDRP_SETTLE SET TRACER_RPT=IIF(IF_OCIC=1,1,0) + IIF(DATEDIFF(day,CREATE_DATE,GETDATE()) > 3 AND STATUS_CODE='1',2,0)
WHERE SEQNO IN (SELECT SEQNO FROM #PDA_IDRP_SETTLE)

END

SET @STATUS =1
----------------------------------------------
--回傳區
----------------------------------------------
SELECT @STATUS AS [Status] , @MESSAGE AS [Message]	 
SELECT * FROM #PDA_IDRP_SETTLE
 
END
