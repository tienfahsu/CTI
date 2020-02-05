using GTICommon.Message;
using PDAModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PDA.Models.IDRP
{
    public class IDRPCASEViewModel
    {
        public string ID { get; set; }
        public string PAYOFF_DATE { get; set; }

        public int SEQNO { get; set; }
        
        public UPDA_IDRP_SETTLE SETTLE { get; set; }

        public List<UPDA_IDRP_PAYMENTREC> PAYMENTREC { get; set; }

        public List<CUS_IDRP_PAYMENTOUT_EX> PAYMENTOUT { get; set; }

        public MessageStatus MS { get; set; }


        public List<PAYITEM> ITEMS { get; set; }

       public PAYITEM SETITEM { get; set; }

        public IDRPCASEViewModel()
        {
            this.SETTLE = new UPDA_IDRP_SETTLE();
            this.PAYMENTREC = new List<UPDA_IDRP_PAYMENTREC>();
            this.PAYMENTOUT = new List<CUS_IDRP_PAYMENTOUT_EX>();
            this.MS = new MessageStatus();

            this.ITEMS = new List<PAYITEM>();
            this.SETITEM = null;
        }

        public void Calc(bool fStopSETITEM = false)
        {
            this.ITEMS.Clear();

            PAYITEM itemPREV = new PAYITEM();
            itemPREV.REC_DATE = this.SETTLE.FIRSTPAY_D;
            itemPREV.CALC_Balance = this.SETTLE.T_ALLBANK;
            itemPREV.CALC_OverduePay = 0;

            int nMAXCNT = Math.Max(Math.Max(this.PAYMENTOUT.Count, this.PAYMENTREC.Count), this.SETTLE.TENOR);
            int nGRACE_CNT = 0;
            int i = 0;
            decimal Total_CALC_RushOff = 0;
            while (i < nMAXCNT)
            {
                PAYITEM item = new PAYITEM((i < this.PAYMENTREC.Count) ? this.PAYMENTREC[i] : null);
                // 重新排REC_NO  因為會加結清日那筆
                item.REC_NO = itemPREV.REC_NO + 1;
                // 如果不是 CUS_IDRP_PAYMENTREC 資料 計算 繳款日期 下個月
                if (item.REC_DATE is null)
                {                                      
                    if(itemPREV.REC_DATE.HasValue)
                       item.REC_DATE = itemPREV.REC_DATE.Value.AddMonths(1);
                }
                // IDRP PAYMENTOUT 資料
                item.SetPayOut((i < this.PAYMENTOUT.Count) ? this.PAYMENTOUT[i] : null);


                //結清日
                if (this.SETTLE.SETTLE_DATE.Value >= itemPREV.REC_DATE.Value && this.SETTLE.SETTLE_DATE.Value <= item.REC_DATE.Value)
                {
                    if (item.REC_DATE.Value.Date == this.SETTLE.SETTLE_DATE.Value.Date)
                    {
                        this.SETITEM = item;
                    }
                    else
                    {
                        TimeSpan ts = this.SETTLE.SETTLE_DATE.Value.Date - itemPREV.REC_DATE.Value.Date;
                        if (ts.TotalDays == 0)
                        {
                            this.SETITEM = itemPREV;
                        }
                        else
                        {
                            //加一筆 結清日
                            PAYITEM itemSET = new PAYITEM();
                            itemSET.REC_NO = item.REC_NO;

                            itemSET.REC_DATE = this.SETTLE.SETTLE_DATE;

                            itemSET.CALC_IntFee = (this.SETTLE.T_ALLBANK - Total_CALC_RushOff) * (this.SETTLE.APR / 100 * (decimal)(ts.TotalDays - 1) / 365);
                            itemSET.CALC_RushOff = this.SETTLE.PMT_INST - itemSET.CALC_IntFee;
                            itemSET.CALC_OverduePay = itemSET.REAL_AMT - this.SETTLE.PMT_INST + itemPREV.CALC_OverduePay;

                            Total_CALC_RushOff += item.CALC_RushOff;

                            itemSET.CALC_Balance = this.SETTLE.T_ALLBANK - (Total_CALC_RushOff + itemSET.CALC_RushOff) - itemSET.CALC_OverduePay;
                            itemSET.CALC_OverdueRate = itemSET.CALC_OverduePay / this.SETTLE.PMT_INST;

                         

                            this.ITEMS.Add(itemSET);
                            this.SETITEM = itemSET;


                            item.REC_NO += 1;
                           
                        }
                    }
                    this.SETITEM.IF_SETTLE = true;
                    this.SETTLE.SETTLE_AMT = this.SETITEM.CALC_Balance;
                    if (fStopSETITEM)
                    {
                        break;
                    }
                    
                }




                if (item.IF_GRACE)
                {
                    item.CALC_IntFee = 0;
                    item.CALC_RushOff = 0;
                    item.CALC_OverduePay = item.REAL_AMT + itemPREV.CALC_OverduePay;
                    nGRACE_CNT++;
                    nMAXCNT++;
                }
                else
                {  
                    item.CALC_IntFee = (this.SETTLE.T_ALLBANK - Total_CALC_RushOff) * this.SETTLE.APR / 100 / 12;
                    //FA 2020/02/05 ADD IF_LARGEPAY
                    if (item.IF_LARGEPAY)
                    {
                        item.CALC_RushOff = item.REAL_AMT - item.CALC_IntFee;
                        item.CALC_OverduePay = itemPREV.CALC_OverduePay;
                    }
                    else
                    {
                        item.CALC_RushOff = this.SETTLE.PMT_INST - item.CALC_IntFee;
                        item.CALC_OverduePay = item.REAL_AMT - this.SETTLE.PMT_INST + itemPREV.CALC_OverduePay;
                    }
                }
                Total_CALC_RushOff += item.CALC_RushOff;


                item.CALC_Balance = this.SETTLE.T_ALLBANK - Total_CALC_RushOff - item.CALC_OverduePay;
                item.CALC_OverdueRate = item.CALC_OverduePay / this.SETTLE.PMT_INST;

               


                this.ITEMS.Add(item);
                itemPREV = item;

               

                i++;
            }

        }
        public decimal GetCheckPoint()
        {
            if(this.SETTLE.T_ALLBANK > 0)
                return (this.SETTLE.T_CITI / this.SETTLE.T_ALLBANK) * this.SETTLE.SETTLE_AMT;
            return 0;
        }
        
        public class PAYITEM : UPDA_IDRP_PAYMENTREC
        {
            /// <summary>
            /// 實際繳款金額 = 繳款金額 - 單獨清償銀行金額
            /// </summary>
            public decimal REAL_AMT { get { return this.REM_AMT - SPD_AMT; } }

            public string DIS_DATE { get; set; } //(varchar(10), not null)
            public decimal? DIS_AMT { get; set; } //(decimal(8,0), not null)
            public int? BANK_CNT { get; set; } //(varchar(3), not null)

            public decimal? BSPD_AMT { get; set; } //(decimal(8,0), not null)

            public decimal CALC_IntFee { get; set; }
            public decimal CALC_RushOff { get; set; }
            public decimal CALC_OverduePay { get; set; }
            public decimal CALC_Balance { get; set; }

            public decimal CALC_OverdueRate { get; set; }
            /// <summary>
            /// IS CUS_IDRP_PAYMENTREC  DATA
            /// </summary>
            public bool IF_PAYREC { get; set; }

            /// <summary>
            /// 結清日 那一筆
            /// </summary>
            public bool IF_SETTLE { get; set; }
       

            
            public PAYITEM(UPDA_IDRP_PAYMENTREC rec = null)
            {
                if (rec != null)
                {
                    this.SEQNO = rec.SEQNO;
                    this.PSEQNO = rec.PSEQNO;                    
                    this.REC_NO = rec.REC_NO;
                    this.REC_DATE = rec.REC_DATE;
                    this.REM_AMT = rec.REM_AMT;
                    this.IF_GRACE = rec.IF_GRACE;
                    this.IF_LARGEPAY = rec.IF_LARGEPAY;
                    this.IF_PAYREC = true;
                    this.IF_SETTLE = false;
                    this.SPD_AMT = rec.SPD_AMT;
                }
                else
                {
                    this.SEQNO = 0;
                    this.PSEQNO = 0;                 
                    this.REC_NO = 0;
                    this.REC_DATE = null;
                    this.REM_AMT = 0;
                    this.IF_GRACE = false;
                    this.IF_LARGEPAY = false;
                    this.IF_PAYREC = false;
                    this.IF_SETTLE = false;
                    this.SPD_AMT = 0;
                }
            }
            public void SetPayOut(CUS_IDRP_PAYMENTOUT_EX exItem )
            {
                if (exItem != null)
                {
                    this.DIS_DATE = exItem.DIS_DATE;
                    this.DIS_AMT = exItem.DIS_AMT;
                    this.BANK_CNT = exItem.BANK_CNT;
                    this.BSPD_AMT = exItem.SPD_AMT;
                }
                else
                {
                    this.DIS_DATE = "";
                    this.DIS_AMT = null;
                    this.BANK_CNT = null;
                    this.BSPD_AMT = null;
                }
            }
            
            
            
        }

        /// <summary>
        ///  DRP_ID_NAME_YYYYMMDD (ID Mask 3) (NAME Mask 1) 
        /// </summary>
        /// <returns></returns>
        public string GetDownloadFileName()
        {
            string sCUSTID = this.SETTLE.CUSTID.PadRight(10);
            sCUSTID = sCUSTID.Substring(0, 4) + "xxx" + sCUSTID.Substring(sCUSTID.Length - 3, 3);
            string sCUSTNAME = this.SETTLE.CUSTNAME.PadRight(2);
            sCUSTNAME = sCUSTNAME.Substring(0, 1) + new string('x', sCUSTNAME.Length - 2) + sCUSTNAME.Substring(sCUSTNAME.Length - 1, 1);
            return string.Format("DRP_{0}_{1}_{2}", sCUSTID, sCUSTNAME, DateTime.Now.ToString("yyyyMMdd"));
        }

        public static Dictionary<string, string> mStatusCodeMapping = new Dictionary<string, string>() { { "1", "尚未審核(Debt)" }, { "2", "已確認(Debt)" }, { "3", "拒絕(Debt)" }, { "4", "尚未審核(Letter)" }, { "5", "已確認(Letter)" }, { "6", "拒絕(Letter)" } };

        public static string GetStatusName(string sStatusCode)
        {
          
            string sStatusName = sStatusCode;
            if (mStatusCodeMapping.ContainsKey(sStatusCode))
                sStatusName = mStatusCodeMapping[sStatusCode];
            
            return sStatusName;
        }
       
        public static SelectList GetStatusList(bool fAddAll = false)
        {

           
            if (fAddAll)
            {
                List<KeyValuePair<string,string>> mItems = mStatusCodeMapping.ToList();
                mItems.Insert(0, new KeyValuePair<string, string>("", "All"));

                return new SelectList(mItems, "Key", "Value");
            }
            else
            {
                return new SelectList(mStatusCodeMapping, "Key", "Value");
            }


        }


       

        public static string STATUS_DebtSend = "1";
        public static string STATUS_DebtApproval = "2";
        public static string STATUS_DebtReject = "3";
        public static string STATUS_LetterSend = "4";
        public static string STATUS_LetterApproval = "5";
        public static string STATUS_LetterReject = "6";



    }
}