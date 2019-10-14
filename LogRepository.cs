using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTIMVC.Models;
using DBconnection;
using System.Data.SqlClient;
using Dapper;

namespace GTIMVC.Models.Log
{
    public class LogRepository
    {
        #region static

       // static conDBS db;

        public void wrTrackingLog(string SysCode, string PersonKey, string UserIP, string FunctionID, string FunctionName, string Remark)
        {
            conDBS db = new conDBS();
           
            if (!db.IsConnected)
            {
                if (!db.connect())
                {
                    throw new Exception("Database Connection Error ! " + db.ErrorMessage);
                }
            }

            db.ParaColl.Clear();
            db.ParaColl.Add(new SqlParameter("@System", SysCode));
            db.ParaColl.Add(new SqlParameter("@SOEID", PersonKey));
            db.ParaColl.Add(new SqlParameter("@UserIP", UserIP));
            db.ParaColl.Add(new SqlParameter("@FunctionID", FunctionID));
            db.ParaColl.Add(new SqlParameter("@FunctionName", FunctionName));
            db.ParaColl.Add(new SqlParameter("@Remark", Remark));


            string strSQL = "EXEC Insert_TrackingLog_SP";
            int effRow = 0;



            if (db.Exec(strSQL, out effRow))
            {

            }
            else
            {

            }

            db.disconnect();


        }

        public void wrActivityLog(string SysCode, string PersonKey, string UserIP, string FunctionID, string FunctionName, string Remark, string ConnectKey = null)
        {
            conDBS db = new conDBS();

            if (!string.IsNullOrWhiteSpace(ConnectKey))
            {
                db = new conDBS(ConnectKey);
            }

            if (!db.IsConnected)
            {
                if (!db.connect())
                {
                    throw new Exception("Database Connection Error ! " + db.ErrorMessage);
                }
            }

            db.ParaColl.Clear();
            db.ParaColl.Add(new SqlParameter("@System", SysCode));
            db.ParaColl.Add(new SqlParameter("@SOEID", PersonKey));
            db.ParaColl.Add(new SqlParameter("@UserIP", UserIP));
            db.ParaColl.Add(new SqlParameter("@FunctionID", FunctionID));
            db.ParaColl.Add(new SqlParameter("@FunctionName", FunctionName));
            db.ParaColl.Add(new SqlParameter("@Remark", Remark));


            string strSQL = "EXEC Insert_ActivityLog_SP";
            int effRow = 0;


            if (db.Exec(strSQL, out effRow))
            {

            }
            else
            {

            }

            db.disconnect();


        }

        /*
        public void wrProgramLog(string SysCode, string PersonKey, string UserIP, string FunctionID, string FunctionName, string Remark, string ConnectKey = null)
        {
            //conDBS db;

            if (string.IsNullOrWhiteSpace(ConnectKey))
            {
                if (db == null)
                    db = new conDBS();
            }
            else
            {
                db = new conDBS(ConnectKey);
            }

            if (!db.IsConnected)
            {
                if (!db.connect())
                {
                    throw new Exception("Database Connection Error ! " + db.ErrorMessage);
                }
            }
            //
            // 20181123 改使用 Dapper 方式 exec SP
            // 因DBConnection 經常會無法取得 SqlParameterCollection 造成 null Clear() error
            //

            //db.ParaColl.Clear();
            //db.ParaColl.Add(new SqlParameter("@System", SysCode));
            //db.ParaColl.Add(new SqlParameter("@SOEID", PersonKey));
            //db.ParaColl.Add(new SqlParameter("@UserIP", UserIP));
            //db.ParaColl.Add(new SqlParameter("@FunctionID", FunctionID));
            //db.ParaColl.Add(new SqlParameter("@FunctionName", FunctionName));
            //db.ParaColl.Add(new SqlParameter("@Remark", Remark));


            string strSQL = "EXEC Insert_ProgramLog_SP";
            int effRow = 0;


            //if (db.Exec(strSQL, out effRow))
            //{

            //}
            //else
            //{

            //}

            using (var conn = db.ConnObj)
            {
                //conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    strSQL = "Insert_ProgramLog_SP";
                    effRow = db.ConnObj.Execute(strSQL, new
                    {
                        @System = SysCode,
                        @SOEID = PersonKey,
                        @UserIP = UserIP,
                        @FunctionID = FunctionID,
                        @FunctionName = FunctionName,
                        @Remark = Remark
                    }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }

            //if (db.IsConnected)
            //{
            //    strSQL = "Insert_ProgramLog_SP";
            //    effRow = db.ConnObj.Execute(strSQL, new
            //    {
            //        @System = SysCode,
            //        @SOEID = PersonKey,
            //        @UserIP = UserIP,
            //        @FunctionID = FunctionID,
            //        @FunctionName = FunctionName,
            //        @Remark = Remark
            //    }, commandType: System.Data.CommandType.StoredProcedure);
            //}


            db.disconnect();


        }*/
        public void wrProgramLog(string SysCode, string PersonKey, string UserIP, string FunctionID, string FunctionName, string Remark, string ConnectKey = null)
        {
            conDBS db = new conDBS();
            if (!string.IsNullOrWhiteSpace(ConnectKey))                
            {
                db = new conDBS(ConnectKey);
            }

            if (!db.IsConnected || db.ParaColl == null)
            {
                if (!db.connect())
                {
                    throw new Exception("Database Connection Error ! " + db.ErrorMessage);
                }
            }


            db.ParaColl.Clear();
            db.ParaColl.Add(new SqlParameter("@System", SysCode));
            db.ParaColl.Add(new SqlParameter("@SOEID", PersonKey));
            db.ParaColl.Add(new SqlParameter("@UserIP", UserIP));
            db.ParaColl.Add(new SqlParameter("@FunctionID", FunctionID));
            db.ParaColl.Add(new SqlParameter("@FunctionName", FunctionName));
            db.ParaColl.Add(new SqlParameter("@Remark", Remark));


            string strSQL = "EXEC Insert_ProgramLog_SP";
            int effRow = 0;


            if (db.Exec(strSQL, out effRow))
            {

            }
            else
            {

            }

            db.disconnect();
           

        }

        public bool SATrackingLog(string SOEID, string Data, string UtilityID, string UtilityName, string Action)
        {
            bool blRet = false;
            conDBS db = new conDBS();
            
            if (!db.IsConnected)
            {
                if (!db.connect())
                {
                    throw new Exception("Database Connection Error ! " + db.ErrorMessage);
                }
            }

            db.ParaColl.Clear();

            db.ParaColl.Add(new SqlParameter("@SOEID", SOEID));
            db.ParaColl.Add(new SqlParameter("@Datas", Data));
            db.ParaColl.Add(new SqlParameter("@UtilityID", UtilityID));
            db.ParaColl.Add(new SqlParameter("@UtilityName", UtilityName));
            db.ParaColl.Add(new SqlParameter("@Action", Action));


            string strSQL = "EXEC Insert_SATrackingLog_SP";
            int effRow = 0;



            if (db.Exec(strSQL, 600, out effRow))
            {
                blRet = true;
            }
            else
            {
                blRet = false;
            }

            db.disconnect();

            return blRet;
        }
        #endregion

    }
}
