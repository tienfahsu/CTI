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

        
           sACCTNMBR = "16000700002313";
          //  sACCTNMBR = "11000300998568";
            string BEG_DATE = "01/01/12"; // MM/DD/YY 2015 - 11 - 28 00:00:00.000
            string END_DATE = "12/12/15"; // MM/DD/YY 2015 - 11 - 28 00:00:00.000

            try
            {
                bool fSuccess = conn.Connect();


                FLOW _flow = new FLOW { FlowID = "ALS_AMHS", Params = new JsonInput { INPUT1 = sACCTNMBR, INPUT2 = BEG_DATE, INPUT3 = END_DATE, INPUT4 = "8080" } };
                if (conn.Call(_flow))
                {
                    if (conn.getString(24, 2, 36) != "AMPCPSDR AM1047 F: ACCOUNT NOT FOUND")
                    {

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
