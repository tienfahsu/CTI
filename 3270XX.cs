internal bool ASK3270(ref string ScreenResult, string ThisFlowID,
                              string Param01 = null, string Param02 = null, string Param03 = null, string Param04 = null, string Param05 = null,
                              string Param06 = null, string Param07 = null, string Param08 = null, string Param09 = null, string Param10 = null,
                              string Param11 = null, string Param12 = null, string Param13 = null, string Param14 = null, string Param15 = null,
                              string Param16 = null, string Param17 = null, string Param18 = null, string Param19 = null, string Param20 = null
                         )
        {
           
            try
            {

                ScreenResult = null;
                JsonInput jsonInput = new JsonInput();

                // 3270全畫面皆為大寫英數字母
                if (!String.IsNullOrEmpty(Param01)) jsonInput.INPUT1 = Param01.ToUpper();
                if (!String.IsNullOrEmpty(Param02)) jsonInput.INPUT2 = Param02.ToUpper();
                if (!String.IsNullOrEmpty(Param03)) jsonInput.INPUT3 = Param03.ToUpper();
                if (!String.IsNullOrEmpty(Param04)) jsonInput.INPUT4 = Param04.ToUpper();
                if (!String.IsNullOrEmpty(Param05)) jsonInput.INPUT5 = Param05.ToUpper();
                if (!String.IsNullOrEmpty(Param06)) jsonInput.INPUT6 = Param06.ToUpper();
                if (!String.IsNullOrEmpty(Param07)) jsonInput.INPUT7 = Param07.ToUpper();
                if (!String.IsNullOrEmpty(Param08)) jsonInput.INPUT8 = Param08.ToUpper();
                if (!String.IsNullOrEmpty(Param09)) jsonInput.INPUT9 = Param09.ToUpper();
                if (!String.IsNullOrEmpty(Param10)) jsonInput.INPUT10 = Param10.ToUpper();
                if (!String.IsNullOrEmpty(Param11)) jsonInput.INPUT11 = Param11.ToUpper();
                if (!String.IsNullOrEmpty(Param12)) jsonInput.INPUT12 = Param12.ToUpper();
                if (!String.IsNullOrEmpty(Param13)) jsonInput.INPUT13 = Param13.ToUpper();
                if (!String.IsNullOrEmpty(Param14)) jsonInput.INPUT14 = Param14.ToUpper();
                if (!String.IsNullOrEmpty(Param15)) jsonInput.INPUT15 = Param15.ToUpper();
                if (!String.IsNullOrEmpty(Param16)) jsonInput.INPUT16 = Param16.ToUpper();
                if (!String.IsNullOrEmpty(Param17)) jsonInput.INPUT17 = Param17.ToUpper();
                if (!String.IsNullOrEmpty(Param18)) jsonInput.INPUT18 = Param18.ToUpper();
                if (!String.IsNullOrEmpty(Param19)) jsonInput.INPUT19 = Param19.ToUpper();
                if (!String.IsNullOrEmpty(Param20)) jsonInput.INPUT20 = Param20.ToUpper();

                Conn3270 conn = new Conn3270();
                if (conn.Connect())
                {
                    FLOW flow = new FLOW { FlowID = ThisFlowID, Params = jsonInput };

                    if (conn.Call(flow))
                    {
                        ScreenResult = conn.GetPageContext();
                        FBLog.WriteLog("Call3270", ThisFlowID, FBLog.LogStatus.FuncSuccessful);
                    }
                    else
                    {
                        throw new Exception(conn.GetErrorMsg());
                    }
                }
                ErrorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                FBLog.WriteLog("Call3270", ThisFlowID, FBLog.LogStatus.FuncFailed, ex.Message);
                return false;
            }
      }
      
      
      
      
      bool Result = ASK3270(ref ScreenResult, FlowID, DAYCHK.CARDNMBR);

                if (Result == false) return Set3270.GatewayError;

                string[] Lines = ScreenResult.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // 3個Page要有76列(24 + 1 + 24 + 1 + 24 + 1 + 1) 
                if (Lines.Length != 76) throw new Exception("3270回傳的頁數(列數)不符合");

                // 檢查每列的字元數
                for (int LineCount = 0; LineCount < Lines.Length; LineCount++)
                {
                    if (LineCount == 24 || LineCount == 49 || LineCount == 74 || LineCount == 75) continue;
                    //if (Lines[LineCount].Length != 80) throw new Exception(string.Format("3270回傳的第{0}列，長度不符合", LineCount));
                    if (CompStringLength(ref Lines[LineCount]) != 80) throw new Exception(string.Format("3270回傳的第{0}列，長度不符合", LineCount));
                }

                // 比對卡號是否一致
                if (Lines[2].Substring(50, 19) != DAYCHK.CARDNMBR)
                    return Set3270.DataError;

                // Insert To UFORB_SCREENDUMP
                int Page_Len = 24 * 82;

                Models.UFORB_ScreenDump_Repository uFORB_ScreenDump_Repository = new Models.UFORB_ScreenDump_Repository();
                // FLOWID : ECMS_ARQB237 --> ARQB
                uFORB_ScreenDump_Repository.Insert(DB_CardNumber, _PROCESS, FlowID.Substring(5, 4), 2, ScreenResult.Substring(0 * (Page_Len + 2), Page_Len));
                uFORB_ScreenDump_Repository.Insert(DB_CardNumber, _PROCESS, FlowID.Substring(5, 4), 3, ScreenResult.Substring(1 * (Page_Len + 2), Page_Len));
                uFORB_ScreenDump_Repository.Insert(DB_CardNumber, _PROCESS, FlowID.Substring(5, 4), 7, ScreenResult.Substring(2 * (Page_Len + 2), Page_Len));
