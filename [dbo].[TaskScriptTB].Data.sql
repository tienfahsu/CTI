﻿SET IDENTITY_INSERT [dbo].[TaskScriptTB] ON 

INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (1, N'SD_0_0001', N'檢查最後登入時間，超過60天Disable ID，超過90天Delete ID', N'Schedule_CheckLastLoginDate_SP', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:21:31.343' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:15.540' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:15.540' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (2, N'SD_0_0002', N'EERS Output', N'Select * from EERSVW', N'Y', N'Y', N'EERS', N'EERS158748.txt', N'\\fstwtpescbm01\Data\ToHost\EERS', CAST(N'2019-07-31 11:15:18.570' AS DateTime), N'Execution finished but output file write failure', N'GTI', N'GTI', CAST(N'2015-08-20 17:05:24.510' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-20 17:05:24.510' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (3, N'SD_0_0003', N'Clear Comunication Gateway file over 7 days need to deleted file', N'SELECT [SeqNO],[Folder],[FilePath],[FileName],[CRCCode],[CreatedByID],[CreatedByName],[CreatedDT]  FROM [COGDB1].[dbo].[CG_UploadFileTB]  where Datediff(day,[CreatedDT],getdate()) > 7', N'Y', N'Y', N'DEL', NULL, N'\\apaccititwap167\COG_Portal', CAST(N'2019-07-19 12:22:14.287' AS DateTime), N'Clear Comunication Gateway file over 7 days need to deleted file:0 records.', N'GTI', N'GTI', CAST(N'2016-01-13 15:15:36.097' AS DateTime), N'GTI', N'GTI', CAST(N'2016-01-13 15:15:36.097' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (4, N'SD_0_0004', N'Clear Comunication Gateway file over 7 days need to deleted record', N'insert into [COGDB1].[dbo].[L_TrackingLogTB]   ( [System],[SOEID],[UserIP],[FunctionID],[FunctionName],[Remark],[CreatedDT]   )   select ''COG'' ,''System'' ,''apaccititwap165'',''CG\FileDelete'' ,''Communication Gateway - Schedule Delete'' ,FilePath +'' was pending over 7 days that follow the rule need to clear (delete) it.'' ,getdate() FROM [COGDB1].[dbo].[CG_UploadFileTB]   where Datediff(day,[CreatedDT],getdate()) > 7;DELETE  FROM [COGDB1].[dbo].[CG_UploadFileTB]  where Datediff(day,[CreatedDT],getdate()) > 7', N'Y', N'N', N'DEL', NULL, N'\\apaccititwap167\COG_Portal', CAST(N'2019-07-19 12:22:14.317' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2016-01-13 15:15:36.097' AS DateTime), N'GTI', N'GTI', CAST(N'2016-01-13 15:15:36.097' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (5, N'SD_0_0005', N'Clean Tracking Log over 3 years.', N'Schedule_Clean_TrackingLog_Over3Years_SP', N'Y', N'N', NULL, NULL, NULL, CAST(N'2019-07-19 12:22:14.347' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (6, N'SD_0_0006', N'Clean 3 years ago Report', N'Schedule_AnnualClean3YearsAgeReport_SP', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:22:14.377' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (7, N'SD_0_0007', N'Clean 5 years ago SATrackingLog', N'Delete FROM [COGDB2].[dbo].[L_SATrackingLog]  Where (datediff(MONTH,createddt,getdate()) /12) >=5', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:22:14.407' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (8, N'SD_0_0008', N'Sync Communication Gateway Permission with PersonTB', N'Delete from dbo.CG_PersonPermissionTB where SOEID not in (Select SOEID from BASE_PersonTB)', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:22:14.437' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (9, N'SD_0_0009', N'Sync Report Agent with PersonTB', N'delete  FROM [COGDB1].[dbo].[COG_Report_AgentTB]  where [CHKRSOEID] not in (Select SOEID from dbo.BASE_PersonTB )', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:22:14.470' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (10, N'SD_0_0010', N'Sync Report Agent with PersonTB', N'update [COGDB1].[dbo].[COG_Report_AgentTB] set[AgentSOEID1] = '''' ,[AgentName1] = ''''  where [AgentSOEID1] not in (Select SOEID from dbo.BASE_PersonTB )', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:22:14.497' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (11, N'SD_0_0011', N'Sync Report Agent with PersonTB', N'update [COGDB1].[dbo].[COG_Report_AgentTB] set[AgentSOEID2] = '''' ,[AgentName2] = ''''  where [AgentSOEID2] not in (Select SOEID from dbo.BASE_PersonTB )', N'Y', N'N', N'CSV', NULL, NULL, CAST(N'2019-07-19 12:22:14.523' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (12, N'SD_4_0001', N'ECS Import', N'[COGDB2].dbo.UDEBT_DAILY_ECS_IMPORT_JOB_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.580' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (13, N'SD_4_0002', N'ALS Import', N'[COGDB2].dbo.UDEBT_DAILY_ALS_IMPORT_JOB_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.610' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (14, N'SD_4_0003', N'ECS Payment Import', N'[COGDB2].dbo.UDEBT_DAILY_ECS_PAYMENT_IMPORT_JOB_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.640' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (15, N'SD_4_0004', N'ALS Payment Import', N'[COGDB2].dbo.UDEBT_DAILY_ALS_PAYMENT_IMPORT_JOB_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.670' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (16, N'SD_4_0005', N'EPO Import', N'[COGDB2].dbo.UDEBT_DAILY_TRANSFER_EPO_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.697' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime), N'GTI', N'GTI', CAST(N'2015-08-18 15:24:00.000' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (17, N'SD_4_0006', N'RNL-L Import', N'[COGDB2].dbo.UDEBT_DAILY_TRANSFER_L_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.723' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2018-10-03 20:58:45.423' AS DateTime), N'GTI', N'GTI', CAST(N'2018-10-03 20:58:45.423' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (18, N'SD_4_0007', N'RNL-R Import', N'[COGDB2].dbo.UDEBT_DAILY_TRANSFER_R_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.753' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2018-10-03 20:58:45.440' AS DateTime), N'GTI', N'GTI', CAST(N'2018-10-03 20:58:45.440' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (19, N'SD_4_0008', N'ALS GL Payment Import', N'[COGDB2].dbo.UDEBT_DAILY_ALS_GL_PAYMENT_IMPORT_JOB_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.783' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2018-10-04 18:18:55.390' AS DateTime), N'GTI', N'GTI', CAST(N'2018-10-04 18:18:55.390' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (20, N'SD_1_0001', N'PDA Collector Schedule Sync', N' COGDB2.[dbo].[UCLS_DAILY_SYNC_IC_ASSIGN_SP]', N'Y', N'N', N'', NULL, NULL, CAST(N'2019-07-19 12:22:14.550' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2018-10-16 19:49:04.467' AS DateTime), N'GTI', N'GTI', CAST(N'2018-10-16 19:49:04.467' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (21, N'SD_4_0009', N'IDRP MDRP Import', N'[COGDB2].dbo.UDEBT_DAILY_TRANSFER_IDRP_MDRP_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.813' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2019-01-25 17:07:32.720' AS DateTime), N'GTI', N'GTI', CAST(N'2019-01-25 17:07:32.720' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (22, N'SD_4_0010', N'BADRP Import', N'[COGDB2].dbo.UDEBT_DAILY_TRANSFER_BADRP_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.847' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2019-01-25 17:07:32.733' AS DateTime), N'GTI', N'GTI', CAST(N'2019-01-25 17:07:32.733' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (23, N'SD_4_0011', N'EDRP Import', N'[COGDB2].dbo.UDEBT_DAILY_TRANSFER_EDRP_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.877' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2019-01-25 17:07:32.750' AS DateTime), N'GTI', N'GTI', CAST(N'2019-01-25 17:07:32.750' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (24, N'SD_4_0012', N'CHECK PAYMENT COUNT', N'[COGDB2].dbo.UDEBT_DAILY_CHECKPAYMENT_CNT_SP', N'Y', N'N', N'', N'', N'', CAST(N'2019-07-19 12:22:14.910' AS DateTime), N'Success', N'GTI', N'GTI', CAST(N'2019-01-25 17:06:04.260' AS DateTime), N'GTI', N'GTI', CAST(N'2019-01-25 17:06:04.260' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (25, N'SD_1_0002', N'IDRP_SCHJOB', N'http://demo.pizzaclub.com.tw:8080/COG_Batch/api/PDA/IDRP_SCHJOB', N'Y', N'Y', N'API', N'', N'', NULL, NULL, N'GTI', N'GTI', CAST(N'2019-06-05 08:47:10.187' AS DateTime), N'GTI', N'GTI', CAST(N'2019-06-05 08:47:10.187' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (26, N'SSOLogParse', N'SSOLogParse', N'http://demo.pizzaclub.com.tw:8080/COG_Batch/api/COG/SSOLogParse', N'Y', N'Y', N'API', NULL, NULL, NULL, NULL, N'GTI', N'GTI', CAST(N'2019-11-20 11:07:05.137' AS DateTime), N'GTI', N'GTI', CAST(N'2019-11-20 11:07:05.137' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (27, N'TASKLOG', N'TASKLOG', N'[COGDB1] [dbo].[SCHEDULE_TASKLOGTB_INSERT_MANUAL_SP]', N'Y', N'N', N'SQL', NULL, NULL, NULL, NULL, N'GTI', N'GTI', CAST(N'2019-11-20 11:07:05.137' AS DateTime), N'GTI', N'GTI', CAST(N'2019-11-20 11:07:05.137' AS DateTime))
INSERT [dbo].[TaskScriptTB] ([SeqNO], [StepNO], [Description], [SQLCommand], [Enabled], [OutputFile], [FileType], [FileName], [FilePath], [LastExecuteDT], [LastExecuteResult], [CreatedByID], [CreatedByName], [CreatedDT], [LastUpdatedByID], [LastUpdatedByName], [LastUpdatedDT]) VALUES (29, N'SD_0_0012', N'LogToMail', N'http://demo.pizzaclub.com.tw:8080/COG_Batch/api/COG/LogToMail', N'N', N'Y', N'API', NULL, NULL, CAST(N'2020-01-21 23:56:38.423' AS DateTime), N'無資料', N'GTI', N'GTI', CAST(N'2019-12-20 12:28:06.663' AS DateTime), N'GTI', N'GTI', CAST(N'2019-12-20 12:30:06.663' AS DateTime))
SET IDENTITY_INSERT [dbo].[TaskScriptTB] OFF