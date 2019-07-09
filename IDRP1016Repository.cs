using GTICommon.Message;
using GTIMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using PDAModel.Models;
using GTI3270;

namespace PDA.Models.IDRP
{
    public class IDRP1016Repository : BaseRepository
    {
        public MessageStatus Check(IDRP1016ViewModel.Filter filter, string SOEID)
        {
            MessageStatus msg = new MessageStatus();
            msg.Status = false;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }

                strSQL = @"[dbo].[UPDA_IDRP101600_CHECK_SP]";

                msg = conCOGDB2.ConnObj.Query<MessageStatus>(strSQL, param: new
                {
                    ACCTNMBR = filter.ACCTNMBR,
                    SETTLE_DATE = filter.PAYOFF_DATE,
                    COLLECTOR_ID = SOEID
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            catch (Exception ex)
            {
                msg.Status = false;
                msg.Message = ex.Message;
            }

            return msg;
        }


        private MessageStatus UpdatePAYMENT_AMHS(string sACCTNMBR, string SOEID, List<UPDA_IDRP_PAYMENT_AMHS> lstPAYREC)
        {
            MessageStatus msg = new MessageStatus();
            msg.Status = false;
            try
            {

                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }
                using (var tran = conCOGDB2.ConnObj.BeginTransaction())
                {
                    strSQL = "DELETE FROM [dbo].[UPDA_IDRP_PAYMENT_AMHS] WHERE [ACCTNMBR]=@ACCTNMBR AND [CREATORID]=@CREATORID";
                    conCOGDB2.ConnObj.Execute(strSQL, new { ACCTNMBR = sACCTNMBR, CREATORID= SOEID }, tran);


                    strSQL = "INSERT INTO [dbo].[UPDA_IDRP_PAYMENT_AMHS] ([ACCTNMBR], [REC_NO] ,[REC_DATE] ,[REM_AMT]) " +
                               "VALUES(@ACCTNMBR,@REC_NO,@REC_DATE,@REM_AMT)";

                    conCOGDB2.ConnObj.Execute(strSQL, lstPAYREC, tran);

                    tran.Commit();
                    msg.Status = true;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return msg;
        }

        public bool BASE_TEST()
        {
            try
            {

                Conn3270 conn = new Conn3270();
                if (conn.Connect())
                {
                    FLOW _flow = new FLOW { FlowID = "ALS_AMU1", Params = new JsonInput { INPUT1 = "11000800110620" } };
                //    Console.WriteLine("Json ---- {0}", JsonConvert.SerializeObject(_flow));

                    if (conn.Call(_flow))
                    {
                        //Console.WriteLine(conn.getString(1, 1, 5, PageName: "ARQB"));
                        Console.WriteLine(conn.getString(20, 72, 3));

                        Console.WriteLine(conn.getString(24, 2, 36));
                    }
                    else
                    {
                        Console.WriteLine("Error:{0}", conn.GetErrorMsg());
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
            }
            return false;
        }
        public MessageStatus Do3270AMHS(string sACCTNMBR, string SOEID)
        {
            List<UPDA_IDRP_PAYMENT_AMHS> lstAMHS = new List<UPDA_IDRP_PAYMENT_AMHS>();

            BASE_TEST();

            Conn3270 conn = new Conn3270(); //GTI3270


            //List<string> lstACCTNMBR = new List<string>() { "13000100000005", "13000100000006", "13000100000007", "13000100000008", "13000100000010", "13000100000011", "13000100000012", "13000100000014", "13000100000015", "13000100000016", "13000100000017", "13000100000021", "13000100000022", "13000100000023", "13000100000024" };
          //  List<string> lstACCTNMBR = new List<string>() { "13000100000031", "13000100000032", "13000100000033", "13000100000034", "13000100000035", "13000100000051", "13000100000055", "13000100000071", "13000100000072", "13000100000121", "13000100000200", "13000100000201", "13000100002000", "13000100010000", "13000100800001" };
         //   List<string> lstACCTNMBR = new List<string>() { "13000100800012", "13000100800013", "13000100800014", "13000100800052", "13000100800075", "13000100800094", "13000100800100", "13000100800111", "13000100800132", "13000100800183", "13000100800218", "13000100800253", "13000100800273", "13000100800308", "13000100800338" };

            List<string> lstACCTNMBR = new List<string>() { "13000100800360", "13000100800385", "13000100800410", "13000100800443", "13000100800462", "13000100800494", "13000100800516", "13000100800546", "13000100800569", "13000100800626", "13000100800677", "13000100800703", "13000100800755", "13000100800843", "13000100800918" };

            //  sACCTNMBR = "11000300998568";
            string BEG_DATE = "01/01/12"; // MM/DD/YY 2015 - 11 - 28 00:00:00.000
            string END_DATE = "12/12/19"; // MM/DD/YY 2015 - 11 - 28 00:00:00.000

            string lineOfText;
            using (System.IO.StreamReader sr = System.IO.File.OpenText("C:\\APPS\\DOC\\CACSEXT-CACS9.txt.27.txt"))
            {
                while ((lineOfText = sr.ReadLine()) != null)
                {
                    if (!lineOfText.StartsWith("860107"))
                        continue;
                    if (lineOfText.Length < 19)
                        continue;

                    string sxACCTNMBR = lineOfText.Substring(6, 14);

                    if (String.Compare(sxACCTNMBR, "13001088007482") <= 0)
                        continue;

                    try
                    {
                        bool fSuccess = conn.Connect();


                        FLOW _flow = new FLOW { FlowID = "ALS_AMHS", Params = new JsonInput { INPUT1 = sxACCTNMBR, INPUT2 = BEG_DATE, INPUT3 = END_DATE, INPUT4 = "8080" } };
                        if (conn.Call(_flow))
                        {
                            bool fHAVDATA = false;
                            if (conn.Results.Count > 1)
                            {
                                Console.WriteLine(sxACCTNMBR);
                         
                            }
                            string sTemp = conn.getString(9, 5, 36).Trim();
                            if (!String.IsNullOrEmpty(sTemp))
                            {
                                fHAVDATA = true;
                                Console.WriteLine(sxACCTNMBR);
                               
                            }
                            if (fHAVDATA)
                            {
                                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                                foreach (var xItem in conn.Results)
                                {
                                    foreach (var strItem in xItem.PageContent_Source)
                                    {
                                        sb.AppendLine(strItem);
                                    }
                                }
                                if (conn.Results.Count > 1)
                                {

                                    System.IO.File.WriteAllText("C:\\APPS\\DOC\\OUT\\M" + sxACCTNMBR + ".TXT", sb.ToString());
                                }
                                else
                                {
                                    System.IO.File.WriteAllText("C:\\APPS\\DOC\\OUT\\S" + sxACCTNMBR + ".TXT", sb.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageStatus msg = new MessageStatus();
                        msg.Status = false;
                        msg.Message = ex.Message;
                        return msg;
                    }

                }
            }



            
            return UpdatePAYMENT_AMHS(sACCTNMBR, SOEID, lstAMHS);
        }
        public MessageStatus Do3270AMHS_TEST(string sACCTNMBR, string SOEID)
        {
            List<UPDA_IDRP_PAYMENT_AMHS> lstAMHS = new List<UPDA_IDRP_PAYMENT_AMHS>();

            // for test SAVE 
            DateTime dtFIRST_DAY = DateTime.Now.AddMonths(-10);
            for (int i = 0; i < 10; i++)
            {
                UPDA_IDRP_PAYMENT_AMHS item = new UPDA_IDRP_PAYMENT_AMHS();
                item.ACCTNMBR = sACCTNMBR;
                item.REC_NO = i + 1;
                item.REC_DATE = dtFIRST_DAY.AddMonths(i);
                item.REM_AMT = 5600;
                item.CREATORID = SOEID;              
                lstAMHS.Add(item);

            }
            //--------------------------
            return UpdatePAYMENT_AMHS(sACCTNMBR, SOEID, lstAMHS);
        }

        public MessageStatus Send(IDRP1016ViewModel.Filter filter, string SOEID, decimal dSETTLE_AMT = 0)
        {
            MessageStatus msg = new MessageStatus();
            msg.Status = false;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }

                strSQL = @"[dbo].[UPDA_IDRP101600_SEND_SP]";
            

                msg = conCOGDB2.ConnObj.Query<MessageStatus>(strSQL, new
                {
                    ACCTNMBR = filter.ACCTNMBR,
                    SETTLE_DATE = filter.PAYOFF_DATE,
                    COLLECTOR_ID = SOEID,
                    SETTLE_AMT = dSETTLE_AMT
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            catch (Exception ex)
            {
                msg.Status = false;
                msg.Message = ex.Message;
            }
            return msg;
        }
    }
}
