SELECT SUBSTRING(RTRIM(NATNID_REGNNUMB) + SPACE(11),1,11) +  SUBSTRING(convert(text, CUST_TITL_LINE1 + SPACE(60)),1,60)+ SUBSTRING(RTRIM(BIZ_TYPE)+ SPACE(6),1,6)+CAST(CAST(CAST(CAST(RIGHT(RTRIM(NATNID_REGNNUMB),7) AS INT)/3 as float) * RAND() AS DECIMAL(15,2)) AS VARCHAR) FROM BUS
WHERE ISNUMERIC(RIGHT(RTRIM(NATNID_REGNNUMB),7)) =1 AND LTRIM(BIZ_TYPE) !=''
                      
                      
                      
SELECT  SUBSTRING([PBUNIT]+SPACE(3),1,3) + 
        SUBSTRING([PBBRNO]+SPACE(4),1,4) + 
        SUBSTRING([PBSRNO]+SPACE(30),1,30) + 
        SUBSTRING([PBAPNO]+SPACE(24),1,24) + 
        SUBSTRING([PBCHARCODE]+SPACE(8),1,8) + 
        SUBSTRING([PBSTATUS]+SPACE(4),1,4) + 
        SUBSTRING([PBCUSTID]+SPACE(11),1,11) + 
        SUBSTRING([CUSTIDNO]+SPACE(3),1,3) + 
        SUBSTRING([PBCUSTTYPE]+SPACE(3),1,3) + 
        SUBSTRING([PBOPENDATE]+SPACE(8),1,8) + 
	SUBSTRING([PBCNAME]+SPACE(60),1,60) + 
        SUBSTRING([PBCURCODE]+SPACE(3),1,3) + 
        SUBSTRING(CAST([PBACTBAL] AS VARCHAR)+SPACE(16),1,16) + 
        SUBSTRING(CAST([PBBAL] AS VARCHAR)+SPACE(16),1,16) + 
        SUBSTRING(CAST([PBSTOPPAYAMT] AS VARCHAR)+SPACE(15),1,15) + 
        SUBSTRING(CAST([PBCARDAMT] AS VARCHAR)+SPACE(15),1,15) + 
        SUBSTRING([PBGSACTCODE],1,1) +
        SUBSTRING([PBJOINTCODE],1,1)+
        SUBSTRING([PBRATETYPE]+SPACE(16),1,16) + 
        SUBSTRING(CAST([PBINTRATE] AS VARCHAR)+SPACE(9),1,9) + 
        SUBSTRING(CAST([PBINTPAYABLE] AS VARCHAR)+SPACE(15),1,15) + 
        SUBSTRING([PBOVRSTATUS],1,1) +
        SUBSTRING([PBPGKIND],1,1) + 
        SUBSTRING(CAST([PBPGAMT] AS VARCHAR)+SPACE(15),1,15) + 	       
        SUBSTRING([PBPGSETDATE]+SPACE(8),1,8) + 
	SUBSTRING([PBTAXCODE],1,1) +
        SUBSTRING(CAST([PBGROSSINT] AS VARCHAR)+SPACE(15),1,15) + 
        SUBSTRING(CAST([PBGROSSTAX] AS VARCHAR)+SPACE(15),1,15) +  
	SUBSTRING([PBINSURCODE],1,1) +
	SUBSTRING([PBNHICODE],1,1) +
	SUBSTRING([PBTAXPAYERID]+SPACE(11),1,11) + 
	SUBSTRING([PBSECCODE],1,1)+
        SUBSTRING([PBLASTTXDATE]+SPACE(8),1,8)  
     
  FROM [CBCDIC].[dbo].[A21]               
  
  
/****** SSMS 中 SelectTopNRows 命令的指令碼  ******/
SELECT SUBSTRING([TDUNIT]+SPACE(3),1,3) + 
      SUBSTRING([TDBRNO]+SPACE(4),1,4) + 
      SUBSTRING([TDSRNO]+SPACE(30),1,30) + 
      SUBSTRING([TDAPNO]+SPACE(24),1,24) + 
      SUBSTRING([TDCHARCODE]+SPACE(8),1,8) + 
      SUBSTRING([TDSTATUS]+SPACE(4),1,4) + 
      SUBSTRING([TDCUSTID]+SPACE(11),1,11) + 
      SUBSTRING([TDCUSTIDNO]+SPACE(3),1,3) + 
      SUBSTRING([TDCUSTTYPE]+SPACE(3),1,3) + 
      SUBSTRING([TDSLIPNO]+SPACE(16),1,16) + 
      SUBSTRING([TDCNAME]+SPACE(60),1,60) + 
      SUBSTRING([TDCURCODE]+SPACE(3),1,3) + 
      SUBSTRING(CAST([TDAMT] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING(CAST([TDSTOPPAYAMT] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING([TDBGNDATE]+SPACE(8),1,8) + 
      SUBSTRING([TDDUEDATE]+SPACE(8),1,8) + 
      SUBSTRING([TDRATETYPE]+SPACE(16),1,16) + 
      SUBSTRING([TDPERIOD]+SPACE(3),1,3) + 
      SUBSTRING([TDINTTYPE]+SPACE(1),1,1) + 
      SUBSTRING([TDNAMECODE]+SPACE(1),1,1) + 
      SUBSTRING(CAST([TDINTRATE] AS VARCHAR)+SPACE(9),1,9) + 
      SUBSTRING([TDINTPAYCODE]+SPACE(1),1,1) + 
      SUBSTRING([TDAUTOPRIM]+SPACE(1),1,1) + 
      SUBSTRING([TDAUTOINTNO]+SPACE(30),1,30) + 
      SUBSTRING([TDISUEDATE]+SPACE(8),1,8) + 
      SUBSTRING([TDREISUEDATE]+SPACE(8),1,8) + 
      SUBSTRING([TDGSACTCODE]+SPACE(1),1,1) + 
      SUBSTRING([TDJOINTCODE]+SPACE(1),1,1) + 
      SUBSTRING([TDSDCASE]+SPACE(19),1,19) + 
      SUBSTRING([TDINTEDATE]+SPACE(8),1,8) +
      SUBSTRING(CAST([TDINTPAY] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING(CAST([TDINTPAYABLE] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING(CAST([TDVIOLATEAMT] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING([TDPGKIND]+SPACE(1),1,1) + 
      SUBSTRING(CAST([TDPGAMT] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING([TDPGSETDATE]+SPACE(8),1,8) +
      SUBSTRING([TDTAXCODE]+SPACE(1),1,1) + 
      SUBSTRING(CAST([TDGROSSINT] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING(CAST([TDGROSSTAX] AS VARCHAR)+SPACE(15),1,15) + 
      SUBSTRING([TDINSURCODE],1,1) +
      SUBSTRING([TDNHICODE],1,1) +
      SUBSTRING([TDTAXPAYERID]+SPACE(11),1,11) + 
      SUBSTRING([TDSECCODE],1,1)+
      SUBSTRING([TDLASTTXDATE]+SPACE(8),1,8) AS CDICF03       
  FROM [CBCDIC].[dbo].[A22]
  
  
  SELECT SUBSTRING([CKUNIT]+SPACE(3),1,3) + 
       SUBSTRING([CKBRNO]+SPACE(4),1,4) + 
       SUBSTRING([CKSRNO]+SPACE(30),1,30) + 
       SUBSTRING([CKAPNO]+SPACE(24),1,24) + 
       SUBSTRING([CKCHARCODE]+SPACE(8),1,8) + 
       SUBSTRING([CKSTATUS]+SPACE(4),1,4) + 
       SUBSTRING([CKCUSTID]+SPACE(11),1,11) + 
       SUBSTRING([CKCUSTIDNO]+SPACE(3),1,3) + 
       SUBSTRING([CKCUSTTYPE]+SPACE(3),1,3) + 
       SUBSTRING([CKOPENDATE]+SPACE(8),1,8) + 
       SUBSTRING([CKCNAME]+SPACE(60),1,60) + 
       SUBSTRING([CKCURCODE]+SPACE(3),1,3) + 
       SUBSTRING(CAST([CKACTBAL] AS VARCHAR)+SPACE(16),1,16) + 
       SUBSTRING(CAST([CKSTOPPAYAMT] AS VARCHAR)+SPACE(15),1,15) + 
       SUBSTRING([CKJOINTCODE]+SPACE(1),1,1) + 
       SUBSTRING([CKOVRSTATUS]+SPACE(1),1,1) + 
       SUBSTRING(CAST([CKINTPAYABLE]AS VARCHAR)+SPACE(15),1,15) +      
       SUBSTRING([CKTAXCODE]+SPACE(1),1,1) + 
       SUBSTRING(CAST([CKINTRATE] AS VARCHAR)+SPACE(9),1,9) + 
       SUBSTRING(CAST([CKGROSSINT] AS VARCHAR)+SPACE(15),1,15) + 
       SUBSTRING(CAST([CKGROSSTAX] AS VARCHAR)+SPACE(15),1,15) + 
       SUBSTRING([CKINSURCODE]+SPACE(1),1,1) + 
       SUBSTRING([CKNHICODE]+SPACE(1),1,1) + 
       SUBSTRING([CKTAXPAYERID]+SPACE(11),1,11) + 
       SUBSTRING([CKSECCODE]+SPACE(1),1,1) +
       SUBSTRING([CKLASTTXDATE]+SPACE(8),1,8)  AS CDICF04     
  FROM [CBCDIC].[dbo].[A23]
  
  

/****** SSMS 中 SelectTopNRows 命令的指令碼  ******/
/* OTCOMPCODE A26 8600 B26 86XX C26 6900 AND OPAPNO 000XXXXXX */
SELECT  
      SUBSTRING([OTAPNO]+SPACE(10),1,10) + 
	  SUBSTRING([OTRCCODE]+SPACE(10),1,10) + 
	  SUBSTRING([OTCOMPCODE]+SPACE(4),1,4) + 
	  SUBSTRING([OTINTPAYMEMO]+SPACE(25),1,25) + 
	  SUBSTRING(CAST([OTPAYSAV] AS VARCHAR)+SPACE(20),1,20) + 
	  SUBSTRING([OTINTPAYABLE]+SPACE(8),1,8) + 
	  SUBSTRING([OTREFNO]+SPACE(8),1,8) AS CDICF07
  FROM [CBCDIC].[dbo].[A26]

SELECT SUBSTRING([UNIT]+SPACE(3),1,3) + 
       SUBSTRING([BRANCH_NO]+SPACE(4),1,4) + 
       SUBSTRING([SR_NO]+SPACE(30),1,30) + 
       SUBSTRING([AP_NO]+SPACE(24),1,24) + 
       SUBSTRING([APPLY_DATE]+SPACE(8),1,8) + 
       SUBSTRING([APPLY_KIND]+SPACE(1),1,1) + 
       SUBSTRING([BILL_NO]+SPACE(7),1,7) + 
       SUBSTRING([CURRENCY_CODE]+SPACE(3),1,3) + 
       SUBSTRING(CAST([BILL_AMT] AS VARCHAR)+SPACE(15),1,15) + 
       SUBSTRING([BILL_DATE]+SPACE(8),1,8) + 
       SUBSTRING([ACC_NAME]+SPACE(60),1,60) AS CDICF10
  FROM [CBCDIC].[dbo].[A34]
  
  
  SELECT SUBSTRING([UNIT]+SPACE(3),1,3) + 
       SUBSTRING([BRANCH_NO]+SPACE(4),1,4) + 
       SUBSTRING([SR_NO]+SPACE(30),1,30) + 
       SUBSTRING([AP_NO]+SPACE(24),1,24) + 
       SUBSTRING([CUST_ID]+SPACE(11),1,11) + 
       SUBSTRING([A76_TYPE]+SPACE(1),1,1) + 
       SUBSTRING([START_NO]+SPACE(7),1,7) + 
       SUBSTRING([END_NO]+SPACE(7),1,7) + 
       SUBSTRING([CURRENCY_CODE]+SPACE(3),1,3) + 
       SUBSTRING(CAST([AMT] AS VARCHAR)+SPACE(15),1,15) + 
       SUBSTRING([DUE_DATE]+SPACE(8),1,8) + 
       SUBSTRING([CODE]+SPACE(3),1,3) + 
       SUBSTRING([DISHONORED_REASON]+SPACE(2),1,2) + 
       SUBSTRING([ENTRY_DATE]+SPACE(8),1,8) + 
       SUBSTRING([RESERVE_DATE]+SPACE(8),1,8) AS CDICF24 
  FROM [CBCDIC].[dbo].[A76]
