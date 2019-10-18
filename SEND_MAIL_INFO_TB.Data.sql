INSERT SEND_MAIL_INFO_TB ([CREATEDT], [CREATEUSER], [PROJECT], [UTILITYID], [UTILITYNAME], [SENDER], [SENDERNAME], [RECEIVER], [CC], [BCC], [SUBJECT], [BODY], [ISHTML], [FILENAME], [REMARK]) VALUES (CAST(N'2019-04-19 11:02:35.347' AS DateTime), N'alpha', N'PDA_IDRP', N'1', N'PDA', N'', N'', N'1006@COGMailGroup', N'', N'', N'Tracer reports', N'<html>
	<head></head>
	<body>
		<span style="font-size:10.0pt">Dear All,<br />
			<br />
			<br /> Tracer reports 如附件.
			<br />
		</span>
		<span style="font-size:10.0pt">________________________________________________________________________________________<br />
			<br />  this email is sent by PDA IDRP batch process automatically  .<br />________________________________________________________________________________________<br />
		</span>
	</body>
</html>
', N'Y', N'', N'')
