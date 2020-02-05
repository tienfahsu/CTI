using GTIMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDAModel.Models;
using Dapper;
using System.Data;
using GTICommon.Message;
using NPOI.SS.UserModel;
using GTIOpenXML;
using System.IO;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using System.Web.Mvc;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace PDA.Models.IDRP
{
    public class IDRPCASERepository : BaseRepository
    {
        public IDRPCASEViewModel Preview(string sID, string sPAYOFFDATE, string SOEID)
        {
            IDRPCASEViewModel model = new IDRPCASEViewModel();
            model.ID = sID;
            model.PAYOFF_DATE = sPAYOFFDATE;
            model.MS.Status = false;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }

                strSQL = @"[dbo].[UPDA_IDRP101100_PREVIEW_SP]";

                var sqlResult = conCOGDB2.ConnObj.QueryMultiple(strSQL, new
                {
                    CUSTID = sID,
                    SETTLE_DATE = sPAYOFFDATE,
                    COLLECTOR_ID = SOEID
                }, commandType: CommandType.StoredProcedure);

                model.MS = sqlResult.Read<MessageStatus>().FirstOrDefault();
                if (model.MS.Status)
                {
                    model.SETTLE = sqlResult.Read<UPDA_IDRP_SETTLE>().FirstOrDefault();
                    model.PAYMENTREC = sqlResult.Read<UPDA_IDRP_PAYMENTREC>().ToList();
                    model.PAYMENTOUT = sqlResult.Read<CUS_IDRP_PAYMENTOUT_EX>().ToList();

                    model.Calc(true);
                }
            }
            catch (Exception ex)
            {
                model.MS.Status = false;
                model.MS.Message = ex.Message;
            }
            return model;
        }
        public IDRPCASEViewModel Preview2ND(string sACCTNMBR, string sPAYOFFDATE, string SOEID)
        {
            IDRPCASEViewModel model = new IDRPCASEViewModel();
            model.ID = sACCTNMBR;
            model.PAYOFF_DATE = sPAYOFFDATE;
            model.MS.Status = false;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }

                strSQL = @"[dbo].[UPDA_IDRP101600_PREVIEW_SP]";


                var sqlResult = conCOGDB2.ConnObj.QueryMultiple(strSQL, new
                {
                    ACCTNMBR = sACCTNMBR,
                    SETTLE_DATE = sPAYOFFDATE,
                    COLLECTOR_ID = SOEID
                }, commandType: CommandType.StoredProcedure);

                model.MS = sqlResult.Read<MessageStatus>().FirstOrDefault();
                if (model.MS.Status)
                {
                    model.SETTLE = sqlResult.Read<UPDA_IDRP_SETTLE>().FirstOrDefault();
                    model.PAYMENTREC = sqlResult.Read<UPDA_IDRP_PAYMENTREC>().ToList();
                    //  model.PAYMENTOUT = sqlResult.Read<CUS_IDRP_PAYMENTOUT_EX>().ToList();
                    model.Calc(true);
                }
            }
            catch (Exception ex)
            {
                model.MS.Status = false;
                model.MS.Message = ex.Message;
            }
            return model;
        }

        public IDRPCASEViewModel GetCase(int SEQNO, bool fStopSETITEM = false)
        {
            IDRPCASEViewModel model = new IDRPCASEViewModel();
            model.SEQNO = SEQNO;
            model.MS.Status = false;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }

                strSQL = @"[dbo].[UPDA_IDRP101100_CASE_SP]";

                var sqlResult = conCOGDB2.ConnObj.QueryMultiple(strSQL, new
                {
                    SEQNO = SEQNO
                }, commandType: CommandType.StoredProcedure);

                model.MS = sqlResult.Read<MessageStatus>().FirstOrDefault();
                if (model.MS.Status)
                {
                    model.SETTLE = sqlResult.Read<UPDA_IDRP_SETTLE>().FirstOrDefault();
                    model.PAYMENTREC = sqlResult.Read<UPDA_IDRP_PAYMENTREC>().ToList();
                    model.PAYMENTOUT = sqlResult.Read<CUS_IDRP_PAYMENTOUT_EX>().ToList();

                    model.ID = model.SETTLE.CUSTID;
                    if (model.SETTLE.SETTLE_DATE.HasValue)
                        model.PAYOFF_DATE = model.SETTLE.SETTLE_DATE.Value.ToString("yyyy/MM/dd");

                    model.Calc(fStopSETITEM);
                }
            }
            catch (Exception ex)
            {
                model.MS.Status = false;
                model.MS.Message = ex.Message;
            }
            return model;
        }

        public static bool CreateExcel(IDRPCASEViewModel Model, out MemoryStream ms, bool fFormula = false, bool fAdvance = true)
        {
            string sLogoImage = HttpContext.Current.Request.MapPath("~/Images/citilogo.png");

            ms = new MemoryStream();

            int nCol = fAdvance ? 2 : 0;

            ExcelOLE worksheet = new ExcelOLE();

            worksheet.GetSheet("試算表");

            // //FontName,Height,NISUB,Color
            Dictionary<string, string> dtFillColor = new Dictionary<string, string>() { { "FillForegroundColor", "Yellow" }, { "FillPattern", "SolidForeground" } };
            Dictionary<string, string> dtFillColorRED = new Dictionary<string, string>() { { "FillForegroundColor", "Yellow" }, { "FillPattern", "SolidForeground" }, { "Font", ",,,Red" } };

            int xTXTBLUE = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Font", ",,,Blue" } });

            int xTXTC = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" } });
            int xTXTCNRate = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" }, { "Format", "#,##0.00" } });
            int xTXTR = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "Format", "$#,##0" } });
            int xTXTR_B = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "Format", "$#,##0" }, { "Font", ",,,Blue" } });
            //  int xTXTRRATE = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "Format", "#,##0.00%" } });
            int xTXTRRATE_B = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "Format", "#,##0.00%" }, { "Font", ",,,Blue" } });
            int xTXTRN = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" } });
            int xTXTRN_B = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "Font", ",,,Blue" } });



            // int xBorderTD = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" }, { "FillForegroundColor", "Yellow" }, { "FillPattern", "SolidForeground" } });
            int xborderTDC = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" } });
            int xborderTDC_B = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" }, { "Font", ",,,Blue" } });
            int xborderTDR = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" }, { "Format", "$#,##0" } });
            int xborderTDR_B = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Right" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" }, { "Format", "$#,##0" }, { "Font", ",,,Blue" } });
            //  int xborderTDDate = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" },{ "Format", "yyyy/MM/dd" } });
            int xborderTDDate_B = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" }, { "BorderLeft", "Thin" }, { "BorderRight", "Thin" }, { "BorderTop", "Thin" }, { "BorderBottom", "Thin" }, { "Format", "yyyy/MM/dd" }, { "Font", ",,,Blue" } });

            //int xTXTC  = worksheet.CreateCellStyle(new Dictionary<string, string>() { { "Alignment", "Center" }, { "VerticalAlignment", "Center" } });


            if (fAdvance)
            {
                worksheet.SetColumnWidth(0, 9 * 256);  //大額還款
                worksheet.SetColumnWidth(1, 9 * 256);  // 喘息
            }
            worksheet.SetColumnWidth(0 + nCol, 12 * 256); //期數
            worksheet.SetColumnWidth(1 + nCol, 13 * 256); //繳款日期
            worksheet.SetColumnWidth(2 + nCol, 13 * 256); //實際繳款金額
            worksheet.SetColumnWidth(3 + nCol, 13 * 256); //當期利息費用
            worksheet.SetColumnWidth(4 + nCol, 12 * 256);  //本金沖抵
            worksheet.SetColumnWidth(5 + nCol, 14 * 256); //帳上累積溢繳款
            worksheet.SetColumnWidth(6 + nCol, 14 * 256); //本金帳款餘額
            if (fAdvance)
            {
                worksheet.SetColumnWidth(7 + nCol, 11 * 256); //-
                worksheet.SetColumnWidth(8 + nCol, 13 * 256); //日期
                worksheet.SetColumnWidth(9 + nCol, 10 * 256); //金額
                worksheet.SetColumnWidth(10 + nCol, 10 * 256); //分配家數
            }

            worksheet.SetCellString(1, 0 + nCol, "總債權金額", xTXTBLUE);
            worksheet.SetCellObject(1, 1 + nCol, Model.SETTLE.T_ALLBANK, xTXTR_B);
            worksheet.SetCellString(1, 4 + nCol, "姓名", xTXTBLUE);
            worksheet.SetCellString(1, 5 + nCol, Model.SETTLE.CUSTNAME, xTXTBLUE);
            //@Url.Content("~/Images/citilogo.png")
            byte[] LogoBytes = System.IO.File.ReadAllBytes(sLogoImage);
            worksheet.SetPicture(LogoBytes, 255, 0, 0, 0, 6 + nCol, 1, 6 + nCol, 2, System.Drawing.Imaging.ImageFormat.Png);

            if (fAdvance)
            {
                worksheet.SetCellString(1, 8 + nCol, "最後分配金額", xTXTBLUE);
                worksheet.SetCellObject(1, 9 + nCol, Model.GetCheckPoint(), xTXTR_B);
                worksheet.AddMergedRegion(1, 1, 9 + nCol, 10 + nCol);
            }

            worksheet.SetCellString(2, 0 + nCol, "協商期數", xTXTBLUE);
            worksheet.SetCellObject(2, 1 + nCol, Model.SETTLE.TENOR, xTXTRN_B);
            worksheet.SetCellString(2, 4 + nCol, "身份證字號", xTXTBLUE);
            worksheet.SetCellString(2, 5 + nCol, Model.SETTLE.CUSTID, xTXTBLUE);


            if (fAdvance)
            {
                worksheet.SetCellString(2, 8 + nCol, "單獨受償", xTXTBLUE);
                worksheet.SetCellObject(2, 9 + nCol, String.Format("{0}", Model.SETTLE.IF_SPAID ? "V" : ""), xTXTR_B);
                worksheet.AddMergedRegion(2, 2, 9 + nCol, 10 + nCol);
            }

            worksheet.SetCellString(3, 0 + nCol, "協商月付金額", xTXTBLUE);
            worksheet.SetCellObject(3, 1 + nCol, Model.SETTLE.PMT_INST, xTXTR_B);

            worksheet.SetCellString(4, 0 + nCol, "協商利率", xTXTBLUE);
            worksheet.SetCellObject(4, 1 + nCol, Model.SETTLE.APR / 100, xTXTRRATE_B);

            worksheet.SetCellString(5, 0 + nCol, "協商首繳日", xTXTBLUE);
            worksheet.SetCellString(5, 1 + nCol, String.Format("{0:yyyy/MM/dd}", Model.SETTLE.FIRSTPAY_D), xTXTRN_B);
            if (fAdvance)
            {
                worksheet.SetCellString(5, 8 + nCol, "IDRP AP");
                worksheet.SetCellString(5, 9 + nCol, "IDRP AP");



                worksheet.SetCellString(6, 0, "大額還款", xTXTC);
                worksheet.SetCellString(6, 1, "喘息", xTXTC);
            }
            worksheet.SetCellString(6, 0 + nCol, "期數", xborderTDC);
            worksheet.SetCellString(6, 1 + nCol, "繳款日期", xborderTDC_B);
            worksheet.SetCellString(6, 2 + nCol, "實際繳款金額", xborderTDR_B);
            worksheet.SetCellString(6, 3 + nCol, "當期利息費用", xborderTDR);
            worksheet.SetCellString(6, 4 + nCol, "本金沖抵", xborderTDR);
            worksheet.SetCellString(6, 5 + nCol, "帳上累積溢繳款", xborderTDR);
            worksheet.SetCellString(6, 6 + nCol, "本金帳款餘額", xborderTDR);
            if (fAdvance)
            {
                worksheet.SetCellString(6, 7 + nCol, "", xTXTC);
                worksheet.SetCellString(6, 8 + nCol, "日期", xTXTC);
                worksheet.SetCellString(6, 9 + nCol, "金額", xTXTR);
                worksheet.SetCellString(6, 10 + nCol, "分配家數", xTXTR);

                worksheet.SetCellString(7, 0, "", xTXTC);
                worksheet.SetCellString(7, 1, "", xTXTC);
            }
            worksheet.SetCellString(7, 0 + nCol, "", xborderTDC);
            worksheet.SetCellString(7, 1 + nCol, "", xborderTDC);
            worksheet.SetCellString(7, 2 + nCol, "", xborderTDR);
            worksheet.SetCellString(7, 3 + nCol, "", xborderTDR);
            worksheet.SetCellString(7, 4 + nCol, "", xborderTDR);
            worksheet.SetCellString(7, 5 + nCol, "", xborderTDR);
            if (fFormula)
                worksheet.SetCellFormula(7, 6 + nCol, String.Format("{0}{1}", ExcelBase.GetColNameByIndex(1 + nCol), 2), xborderTDR);
            else
                worksheet.SetCellObject(7, 6 + nCol, Model.SETTLE.T_ALLBANK, xborderTDR);
            if (fAdvance)
            {
                worksheet.SetCellString(7, 7 + nCol, "", xTXTC);
                worksheet.SetCellString(7, 8 + nCol, "", xTXTC);
                worksheet.SetCellString(7, 9 + nCol, "", xTXTR);
                worksheet.SetCellString(7, 10 + nCol, "", xTXTR);
            }
            IDRPCASEViewModel.PAYITEM itemPREV = new IDRPCASEViewModel.PAYITEM();
            itemPREV.REC_DATE = Model.SETTLE.FIRSTPAY_D;
            itemPREV.CALC_Balance = Model.SETTLE.T_ALLBANK;
            itemPREV.CALC_OverduePay = 0;

            int nRow = 8;
            foreach (var item in Model.ITEMS)
            {

                if (fAdvance)
                {
                    worksheet.SetCellString(nRow, 0, String.Format("{0}", item.IF_LARGEPAY ? "V" : ""), xTXTC);
                    worksheet.SetCellString(nRow, 1, String.Format("{0}", item.IF_GRACE ? "V" : ""), xTXTC);
                }
                worksheet.SetCellString(nRow, 0 + nCol, String.Format("{0}", item.REC_NO), xborderTDC);
                //worksheet.SetCellString( nRow, 1 + nCol, (item.REC_DATE.HasValue ? String.Format("{0:yyyy/MM/dd}", item.REC_DATE) : ""), xborderTDC);
                worksheet.SetCellObject(nRow, 1 + nCol, item.REC_DATE, xborderTDDate_B);

                worksheet.SetCellObject(nRow, 2 + nCol, item.REAL_AMT, xborderTDR_B);
                if (fFormula)
                {
                    //總債權金額
                    string sFnT_ALLBANK = String.Format("{0}{1}", ExcelBase.GetColNameByIndex(1 + nCol), 2);

                    //協商月付金額
                    string sFnPMT_INST = String.Format("{0}{1}", ExcelBase.GetColNameByIndex(1 + nCol), 4);

                    if (item.IF_GRACE)
                    {

                        worksheet.SetCellObject(nRow, 3 + nCol, (decimal)0, xborderTDR);
                        worksheet.SetCellObject(nRow, 4 + nCol, (decimal)0, xborderTDR);
                        worksheet.SetCellFormula(nRow, 5 + nCol, String.Format(nRow == 8 ? "{0}{1}" : "{0}{1}+{2}{3}", ExcelBase.GetColNameByIndex(2 + nCol), nRow + 1, ExcelBase.GetColNameByIndex(5 + nCol), nRow), xborderTDR);

                    }
                    else
                    {
                        //結清日 用年去算
                        if (item.IF_SETTLE && item.REC_DATE.Value.Date.Day != itemPREV.REC_DATE.Value.Date.Day)
                        {
                            //當期利息費用
                            string sFormulaIntFee = String.Format("({0}8-SUM({1}8:{1}{2}))*{3}5/365*({3}{4}-{3}{2}-1)", ExcelBase.GetColNameByIndex(6 + nCol), ExcelBase.GetColNameByIndex(4 + nCol), nRow, ExcelBase.GetColNameByIndex(1 + nCol), nRow + 1);
                            //=($G$13-SUM($E$14:E47))*$B$6/365*(B48-B47-1)
                            worksheet.SetCellFormula(nRow, 3 + nCol, sFormulaIntFee, xborderTDR);
                        }
                        else
                        {
                            // = (I8 - SUM(G8: G30)) * D5 / 12
                            //=($I$9-SUM($G$10:G10))*$D$6/12
                            worksheet.SetCellFormula(nRow, 3 + nCol, String.Format("({0}8-SUM({1}8:{1}{2}))*{3}5/12", ExcelBase.GetColNameByIndex(6 + nCol), ExcelBase.GetColNameByIndex(4 + nCol), nRow, ExcelBase.GetColNameByIndex(1 + nCol)), xborderTDR);
                        }

                        if (item.IF_LARGEPAY)
                        {
                            //=$D$5-F10
                            worksheet.SetCellFormula(nRow, 4 + nCol, String.Format("{0}{2}-{1}{2}", ExcelBase.GetColNameByIndex(2 + nCol), ExcelBase.GetColNameByIndex(3 + nCol), nRow + 1), xborderTDR);
                            //=H10
                            if (nRow == 8) //First 
                                worksheet.SetCellObject(nRow, 5 + nCol, 0);
                            else
                                worksheet.SetCellFormula(nRow, 5 + nCol, String.Format("{0}{1}",  ExcelBase.GetColNameByIndex(5 + nCol), nRow), xborderTDR);
                        }
                        else
                        {
                            //=$D$5-F10
                            worksheet.SetCellFormula(nRow, 4 + nCol, String.Format("{0}-{1}{2}", sFnPMT_INST, ExcelBase.GetColNameByIndex(3 + nCol), nRow + 1), xborderTDR);
                            //=E11-$D$5+H10
                            worksheet.SetCellFormula(nRow, 5 + nCol, String.Format(nRow == 8 ? "{0}{1}-{2}" : "{0}{1}-{2}+{3}{4}", ExcelBase.GetColNameByIndex(2 + nCol), nRow + 1, sFnPMT_INST, ExcelBase.GetColNameByIndex(5 + nCol), nRow), xborderTDR);
                        }
                    }


                    // =$D$2 - SUM(G$10:G11) - H11
                    worksheet.SetCellFormula(nRow, 6 + nCol, String.Format("{0}-SUM({1}8:{1}{2})-{3}{2}", sFnT_ALLBANK, ExcelBase.GetColNameByIndex(4 + nCol), nRow + 1, ExcelBase.GetColNameByIndex(5 + nCol)), xborderTDR);
                    if (fAdvance)
                    {
                        //=H72/$D$5
                        worksheet.SetCellFormula(nRow, 7 + nCol, String.Format("{0}{1}/{2}", ExcelBase.GetColNameByIndex(5 + nCol), nRow + 1, sFnPMT_INST), xTXTCNRate);
                    }
                }
                else
                {
                    worksheet.SetCellObject(nRow, 3 + nCol, item.CALC_IntFee, xborderTDR);
                    worksheet.SetCellObject(nRow, 4 + nCol, item.CALC_RushOff, xborderTDR);
                    worksheet.SetCellObject(nRow, 5 + nCol, item.CALC_OverduePay, xborderTDR);
                    worksheet.SetCellObject(nRow, 6 + nCol, item.CALC_Balance, xborderTDR);
                    if (fAdvance)
                    {
                        worksheet.SetCellObject(nRow, 7 + nCol, item.CALC_OverdueRate, xTXTCNRate);
                    }
                }
                if (fAdvance)
                {
                    worksheet.SetCellString(nRow, 8 + nCol, item.DIS_DATE, xTXTC);
                    worksheet.SetCellObject(nRow, 9 + nCol, (item.DIS_AMT.HasValue ? (object)item.DIS_AMT.Value : ""), xTXTR);
                    worksheet.SetCellObject(nRow, 10 + nCol, (item.BANK_CNT.HasValue ? (object)item.BANK_CNT.Value : ""), xTXTRN);
                }

                if (item.IF_SETTLE)
                {
                    worksheet.AppendStyle(nRow, 0 + nCol, dtFillColor);
                    worksheet.AppendStyle(nRow, 1 + nCol, dtFillColor);
                    worksheet.AppendStyle(nRow, 2 + nCol, dtFillColor);
                    worksheet.AppendStyle(nRow, 3 + nCol, dtFillColor);
                    worksheet.AppendStyle(nRow, 4 + nCol, dtFillColor);
                    worksheet.AppendStyle(nRow, 5 + nCol, dtFillColor);
                    worksheet.AppendStyle(nRow, 6 + nCol, dtFillColorRED);

                }

                itemPREV = item;
                nRow++;
            }


            worksheet.Write(ms);

            ms.Position = 0;
            worksheet.Close();
            return true;
        }

        public static bool CreateExcel_BASE(IDRPCASEViewModel Model, out MemoryStream ms, bool fFormula = false, bool fAdvance = true)
        {

            string sLogoImage = HttpContext.Current.Request.MapPath("~/Images/citilogo.png");

            ms = new MemoryStream();

            int nCol = fAdvance ? 2 : 0;

            IWorkbook wb = ExcelBase.GetWorkbook();

            short MFormat = wb.CreateDataFormat().GetFormat("$#,##0");
            short RATEFormat = wb.CreateDataFormat().GetFormat("#,##0.00%");
            short NRATEFormat = wb.CreateDataFormat().GetFormat("#,##0.00");

            ICellStyle xTXTC = wb.CreateCellStyle();
            xTXTC.Alignment = HorizontalAlignment.Center;
            xTXTC.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle xTXTCNRate = wb.CreateCellStyle();
            xTXTCNRate.Alignment = HorizontalAlignment.Center;
            xTXTCNRate.VerticalAlignment = VerticalAlignment.Center;
            xTXTCNRate.DataFormat = NRATEFormat;


            ICellStyle xTXTR = wb.CreateCellStyle();
            xTXTR.Alignment = HorizontalAlignment.Right;
            xTXTR.VerticalAlignment = VerticalAlignment.Center;
            xTXTR.DataFormat = MFormat;

            ICellStyle xTXTRRATE = wb.CreateCellStyle();
            xTXTRRATE.Alignment = HorizontalAlignment.Right;
            xTXTRRATE.VerticalAlignment = VerticalAlignment.Center;
            xTXTRRATE.DataFormat = RATEFormat;

            ICellStyle xTXTRN = wb.CreateCellStyle();
            xTXTRN.Alignment = HorizontalAlignment.Right;
            xTXTRN.VerticalAlignment = VerticalAlignment.Center;


            ICellStyle xBorderTD = wb.CreateCellStyle();
            xBorderTD.Alignment = HorizontalAlignment.Center;
            xBorderTD.VerticalAlignment = VerticalAlignment.Center;
            xBorderTD.FillForegroundColor = HSSFColor.Yellow.Index2;
            xBorderTD.FillPattern = FillPattern.SolidForeground;
            xBorderTD.BorderLeft = BorderStyle.Thin;
            xBorderTD.BorderRight = BorderStyle.Thin;
            xBorderTD.BorderTop = BorderStyle.Thin;
            xBorderTD.BorderBottom = BorderStyle.Thin;

            ICellStyle xborderTDC = wb.CreateCellStyle();
            xborderTDC.Alignment = HorizontalAlignment.Center;
            xborderTDC.VerticalAlignment = VerticalAlignment.Center;
            xborderTDC.BorderLeft = BorderStyle.Thin;
            xborderTDC.BorderRight = BorderStyle.Thin;
            xborderTDC.BorderTop = BorderStyle.Thin;
            xborderTDC.BorderBottom = BorderStyle.Thin;

            ICellStyle xborderTDR = wb.CreateCellStyle();
            xborderTDR.Alignment = HorizontalAlignment.Right;
            xborderTDR.VerticalAlignment = VerticalAlignment.Center;
            xborderTDR.DataFormat = MFormat;
            xborderTDR.BorderLeft = BorderStyle.Thin;
            xborderTDR.BorderRight = BorderStyle.Thin;
            xborderTDR.BorderTop = BorderStyle.Thin;
            xborderTDR.BorderBottom = BorderStyle.Thin;

            ISheet worksheet = ExcelBase.GetSheet(wb, "試算表");

            if (fAdvance)
            {
                worksheet.SetColumnWidth(0, 9 * 256);  //大額還款
                worksheet.SetColumnWidth(1, 9 * 256);  // 喘息
            }
            worksheet.SetColumnWidth(0 + nCol, 12 * 256); //期數
            worksheet.SetColumnWidth(1 + nCol, 13 * 256); //繳款日期
            worksheet.SetColumnWidth(2 + nCol, 13 * 256); //實際繳款金額
            worksheet.SetColumnWidth(3 + nCol, 13 * 256); //當期利息費用
            worksheet.SetColumnWidth(4 + nCol, 12 * 256);  //本金沖抵
            worksheet.SetColumnWidth(5 + nCol, 14 * 256); //帳上累積溢繳款
            worksheet.SetColumnWidth(6 + nCol, 14 * 256); //本金帳款餘額
            if (fAdvance)
            {
                worksheet.SetColumnWidth(7 + nCol, 11 * 256); //-
                worksheet.SetColumnWidth(8 + nCol, 13 * 256); //日期
                worksheet.SetColumnWidth(9 + nCol, 10 * 256); //金額
                worksheet.SetColumnWidth(10 + nCol, 10 * 256); //分配家數
            }


            ExcelBase.SetRowColString(worksheet, 1, 0 + nCol, "總債權金額");
            ExcelBase.SetRowColObject(worksheet, 1, 1 + nCol, Model.SETTLE.T_ALLBANK, xTXTR);
            ExcelBase.SetRowColString(worksheet, 1, 4 + nCol, "姓名");
            ExcelBase.SetRowColString(worksheet, 1, 5 + nCol, Model.SETTLE.CUSTNAME);
            //@Url.Content("~/Images/citilogo.png")
            byte[] LogoBytes = System.IO.File.ReadAllBytes(sLogoImage);
            IDrawing patriarch = worksheet.DrawingPatriarch;
            if (patriarch == null)
                patriarch = worksheet.CreateDrawingPatriarch();
            ExcelBase.InsertImage(wb, patriarch, LogoBytes, 255, 0, 0, 0, 6 + nCol, 1, 6 + nCol, 2);
            if (fAdvance)
            {
                ExcelBase.SetRowColString(worksheet, 1, 8 + nCol, "最後分配金額");
                ExcelBase.SetRowColObject(worksheet, 1, 9 + nCol, Model.GetCheckPoint(), xTXTR);
                worksheet.AddMergedRegion(new CellRangeAddress(1, 1, 9 + nCol, 10 + nCol));
            }

            ExcelBase.SetRowColString(worksheet, 2, 0 + nCol, "協商期數");
            ExcelBase.SetRowColObject(worksheet, 2, 1 + nCol, Model.SETTLE.TENOR, xTXTRN);
            ExcelBase.SetRowColString(worksheet, 2, 4 + nCol, "身份證字號");
            ExcelBase.SetRowColString(worksheet, 2, 5 + nCol, Model.SETTLE.CUSTID);

            ExcelBase.SetRowColString(worksheet, 3, 0 + nCol, "協商月付金額");
            ExcelBase.SetRowColObject(worksheet, 3, 1 + nCol, Model.SETTLE.PMT_INST, xTXTR);

            ExcelBase.SetRowColString(worksheet, 4, 0 + nCol, "協商利率");
            ExcelBase.SetRowColObject(worksheet, 4, 1 + nCol, Model.SETTLE.APR / 100, xTXTRRATE);

            ExcelBase.SetRowColString(worksheet, 5, 0 + nCol, "協商首繳日");
            ExcelBase.SetRowColString(worksheet, 5, 1 + nCol, String.Format("{0:yyyy/MM/dd}", Model.SETTLE.FIRSTPAY_D), xTXTRN);
            if (fAdvance)
            {
                ExcelBase.SetRowColString(worksheet, 5, 8 + nCol, "IDRP AP");
                ExcelBase.SetRowColString(worksheet, 5, 9 + nCol, "IDRP AP");



                ExcelBase.SetRowColString(worksheet, 6, 0, "大額還款", xTXTC);
                ExcelBase.SetRowColString(worksheet, 6, 1, "喘息", xTXTC);
            }
            ExcelBase.SetRowColString(worksheet, 6, 0 + nCol, "期數", xborderTDC);
            ExcelBase.SetRowColString(worksheet, 6, 1 + nCol, "繳款日期", xborderTDC);
            ExcelBase.SetRowColString(worksheet, 6, 2 + nCol, "實際繳款金額", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 6, 3 + nCol, "當期利息費用", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 6, 4 + nCol, "本金沖抵", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 6, 5 + nCol, "帳上累積溢繳款", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 6, 6 + nCol, "本金帳款餘額", xborderTDR);
            if (fAdvance)
            {
                ExcelBase.SetRowColString(worksheet, 6, 7 + nCol, "", xTXTC);
                ExcelBase.SetRowColString(worksheet, 6, 8 + nCol, "日期", xTXTC);
                ExcelBase.SetRowColString(worksheet, 6, 9 + nCol, "金額", xTXTR);
                ExcelBase.SetRowColString(worksheet, 6, 10 + nCol, "分配家數", xTXTR);

                ExcelBase.SetRowColString(worksheet, 7, 0, "", xTXTC);
                ExcelBase.SetRowColString(worksheet, 7, 1, "", xTXTC);
            }
            ExcelBase.SetRowColString(worksheet, 7, 0 + nCol, "", xborderTDC);
            ExcelBase.SetRowColString(worksheet, 7, 1 + nCol, "", xborderTDC);
            ExcelBase.SetRowColString(worksheet, 7, 2 + nCol, "", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 7, 3 + nCol, "", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 7, 4 + nCol, "", xborderTDR);
            ExcelBase.SetRowColString(worksheet, 7, 5 + nCol, "", xborderTDR);
            if (fFormula)
                ExcelBase.SetRowColFormula(worksheet, 7, 6 + nCol, String.Format("{0}{1}", ExcelBase.GetColNameByIndex(1 + nCol), 2), xborderTDR);
            else
                ExcelBase.SetRowColObject(worksheet, 7, 6 + nCol, Model.SETTLE.T_ALLBANK, xborderTDR);
            if (fAdvance)
            {
                ExcelBase.SetRowColString(worksheet, 7, 7 + nCol, "", xTXTC);
                ExcelBase.SetRowColString(worksheet, 7, 8 + nCol, "", xTXTC);
                ExcelBase.SetRowColString(worksheet, 7, 9 + nCol, "", xTXTR);
                ExcelBase.SetRowColString(worksheet, 7, 10 + nCol, "", xTXTR);
            }

            int nRow = 8;
            foreach (var item in Model.ITEMS)
            {

                if (fAdvance)
                {
                    ExcelBase.SetRowColString(worksheet, nRow, 0, String.Format("{0}", item.IF_LARGEPAY ? "V" : ""), xTXTC);
                    ExcelBase.SetRowColString(worksheet, nRow, 1, String.Format("{0}", item.IF_GRACE ? "V" : ""), xTXTC);
                }
                ExcelBase.SetRowColString(worksheet, nRow, 0 + nCol, String.Format("{0}", item.REC_NO), xborderTDC);
                ExcelBase.SetRowColString(worksheet, nRow, 1 + nCol, (item.REC_DATE.HasValue ? String.Format("{0:yyyy/MM/dd}", item.REC_DATE) : ""), xborderTDC);
                ExcelBase.SetRowColObject(worksheet, nRow, 2 + nCol, item.REAL_AMT, xborderTDR);
                if (fFormula)
                {
                    //總債權金額
                    string sFnT_ALLBANK = String.Format("{0}{1}", ExcelBase.GetColNameByIndex(1 + nCol), 2);

                    //協商月付金額
                    string sFnPMT_INST = String.Format("{0}{1}", ExcelBase.GetColNameByIndex(1 + nCol), 4);

                    if (item.IF_GRACE)
                    {

                        ExcelBase.SetRowColObject(worksheet, nRow, 3 + nCol, (decimal)0, xborderTDR);
                        ExcelBase.SetRowColObject(worksheet, nRow, 4 + nCol, (decimal)0, xborderTDR);
                        ExcelBase.SetRowColFormula(worksheet, nRow, 5 + nCol, String.Format(nRow == 8 ? "{0}{1}" : "{0}{1}+{2}{3}", ExcelBase.GetColNameByIndex(2 + nCol), nRow + 1, ExcelBase.GetColNameByIndex(5 + nCol), nRow), xborderTDR);

                    }
                    else
                    {
                        //=($I$9-SUM($G$10:G10))*$D$6/12
                        ExcelBase.SetRowColFormula(worksheet, nRow, 3 + nCol, String.Format("({0}8-SUM({1}8:{1}{2}))*{3}5/12", ExcelBase.GetColNameByIndex(6 + nCol), ExcelBase.GetColNameByIndex(4 + nCol), nRow, ExcelBase.GetColNameByIndex(1 + nCol)), xborderTDR);
                        //=$D$5-F10
                        ExcelBase.SetRowColFormula(worksheet, nRow, 4 + nCol, String.Format("{0}-{1}{2}", sFnPMT_INST, ExcelBase.GetColNameByIndex(3 + nCol), nRow + 1), xborderTDR);
                        //=E11-$D$5+H10
                        ExcelBase.SetRowColFormula(worksheet, nRow, 5 + nCol, String.Format(nRow == 8 ? "{0}{1}-{2}" : "{0}{1}-{2}+{3}{4}", ExcelBase.GetColNameByIndex(2 + nCol), nRow + 1, sFnPMT_INST, ExcelBase.GetColNameByIndex(5 + nCol), nRow), xborderTDR);
                    }
                    // =$D$2 - SUM(G$10:G11) - H11
                    ExcelBase.SetRowColFormula(worksheet, nRow, 6 + nCol, String.Format("{0}-SUM({1}8:{1}{2})-{3}{2}", sFnT_ALLBANK, ExcelBase.GetColNameByIndex(4 + nCol), nRow + 1, ExcelBase.GetColNameByIndex(5 + nCol)), xborderTDR);
                    if (fAdvance)
                    {
                        //=H72/$D$5
                        ExcelBase.SetRowColFormula(worksheet, nRow, 7 + nCol, String.Format("{0}{1}/{2}", ExcelBase.GetColNameByIndex(5 + nCol), nRow + 1, sFnPMT_INST), xTXTCNRate);
                    }
                }
                else
                {
                    ExcelBase.SetRowColObject(worksheet, nRow, 3 + nCol, item.CALC_IntFee, xborderTDR);
                    ExcelBase.SetRowColObject(worksheet, nRow, 4 + nCol, item.CALC_RushOff, xborderTDR);
                    ExcelBase.SetRowColObject(worksheet, nRow, 5 + nCol, item.CALC_OverduePay, xborderTDR);
                    ExcelBase.SetRowColObject(worksheet, nRow, 6 + nCol, item.CALC_Balance, xborderTDR);
                    if (fAdvance)
                    {
                        ExcelBase.SetRowColObject(worksheet, nRow, 7 + nCol, item.CALC_OverdueRate, xTXTCNRate);
                    }
                }
                if (fAdvance)
                {
                    ExcelBase.SetRowColString(worksheet, nRow, 8 + nCol, item.DIS_DATE, xTXTC);
                    ExcelBase.SetRowColObject(worksheet, nRow, 9 + nCol, (item.DIS_AMT.HasValue ? (object)item.DIS_AMT.Value : ""), xTXTR);
                    ExcelBase.SetRowColObject(worksheet, nRow, 10 + nCol, (item.BANK_CNT.HasValue ? (object)item.BANK_CNT.Value : ""), xTXTRN);
                }


                nRow++;
            }


            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            wb.Close();
            return true;
        }


        public UPDA_IDRP_SETTLE GetCaseByCustID(string CUSTID,string STSCODE)
        {
            UPDA_IDRP_SETTLE retSETTLE = null;
            try
            {

                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }
                strSQL = @"SELECT * FROM UPDA_IDRP_SETTLE WHERE CUSTID=@_CUSTID AND STATUS_CODE=@_STS_CODE";
                retSETTLE = conCOGDB2.ConnObj.Query<UPDA_IDRP_SETTLE>(strSQL, new
                {
                    _CUSTID = CUSTID,
                    _STS_CODE = STSCODE
                }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return retSETTLE;
        }
        public UPDA_IDRP_SETTLE GetCaseBySEQNO(int SEQNO)
        {
            UPDA_IDRP_SETTLE retSETTLE = null;
            try
            {

                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }
                strSQL = @"SELECT * FROM UPDA_IDRP_SETTLE WHERE SEQNO=@_SEQNO";
                retSETTLE = conCOGDB2.ConnObj.Query<UPDA_IDRP_SETTLE>(strSQL, new
                {
                    _SEQNO = SEQNO                  
                }).SingleOrDefault();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return retSETTLE;
        }

        public MessageStatus UpdatePAYMENTREC(int nPSEQNO, List<UPDA_IDRP_PAYMENTREC> lstPAYREC)
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
                    strSQL = "DELETE FROM [dbo].[UPDA_IDRP_PAYMENTREC] WHERE [PSEQNO]=@PSEQNO";
                    conCOGDB2.ConnObj.Execute(strSQL, new { PSEQNO = nPSEQNO }, tran);


                    strSQL = "INSERT INTO [dbo].[UPDA_IDRP_PAYMENTREC] ([PSEQNO] ,[REC_NO] ,[REC_DATE] ,[REM_AMT] ,[IF_GRACE],[IF_LARGEPAY],[SPD_AMT]) " +
                               "VALUES(@PSEQNO,@REC_NO,@REC_DATE,@REM_AMT,@IF_GRACE ,@IF_LARGEPAY,@SPD_AMT)";

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
        public UPDA_IDRP_SENDLETTER GetSendLetter(int nSEQNO)
        {
            UPDA_IDRP_SENDLETTER retSL = new UPDA_IDRP_SENDLETTER();
            retSL.PSEQSN = nSEQNO;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }
                strSQL = "SELECT A.* FROM  UPDA_IDRP_SENDLETTER A LEFT JOIN UPDA_IDRP_SETTLE B ON B.SEND_LETTER_SEQNO = A.SEQNO WHERE B.SEQNO=@_SEQNO";
                retSL = conCOGDB2.ConnObj.Query<UPDA_IDRP_SENDLETTER>(strSQL, new
                {
                    _SEQNO = nSEQNO,

                }).SingleOrDefault();

                if (retSL == null)
                {
                    retSL = new UPDA_IDRP_SENDLETTER();
                    retSL.PSEQSN = nSEQNO;
                }

            }
            catch (Exception ex)
            {
                retSL.SEQNO = -1;
                retSL.ADDRESS = ex.Message;
            }
            return retSL;
        }

        public UPDA_IDRP_SENDLETTER SaveSendLetter(int? nSEQNO, int nPSEQSN, string sRECIPIECT, string sZIP_CODE, string sADDRESS, string sSEND_BY,bool fSendFlag = false)
        {
            UPDA_IDRP_SENDLETTER retSENDLETTER = null;
            try
            {
                if (!conCOGDB2.IsConnected)
                {
                    conCOGDB2.connect();
                }

                strSQL = @"[dbo].[UPDA_IDRP9021D0_SAVE_SP]";
                retSENDLETTER = conCOGDB2.ConnObj.Query<UPDA_IDRP_SENDLETTER>(strSQL, new
                {
                    SEQNO = nSEQNO,
                    PSEQSN = nPSEQSN,
                    RECIPIECT = sRECIPIECT,
                    ZIP_CODE = sZIP_CODE,
                    ADDRESS = sADDRESS,
                    SEND_BY = sSEND_BY,
                    SENDFLAG = fSendFlag
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return retSENDLETTER;        }

        public MessageStatus LoadExecl(int SEQNO, string sExcelFile)
        {
            MessageStatus msg = new MessageStatus();
            msg.Status = false;
            int nCol = 2;
            int PSEQNO = SEQNO;
            List<UPDA_IDRP_PAYMENTREC> lstPayREC = new List<UPDA_IDRP_PAYMENTREC>();

            using (FileStream fs = File.OpenRead(sExcelFile))
            {
                ExcelOLE worksheet = new ExcelOLE(fs, Path.GetExtension(sExcelFile).ToLower() == ".xls");

                string CUSTID = worksheet.GetCellString(2, 5 + nCol);
                if (String.IsNullOrEmpty(CUSTID))
                {
                    msg.Message = "上傳檔案身份證字號欄位是空的或是格式不對 !";
                    return msg;
                }

                UPDA_IDRP_SETTLE idrpSettle = GetCaseBySEQNO(PSEQNO);
                if (idrpSettle == null)
                {
                    msg.Message = String.Format("找不到對應的案件資料 SEQNO={0} !", PSEQNO);
                    return msg;

                }
                if (idrpSettle.CUSTID != CUSTID)
                {
                    msg.Message = String.Format("上傳檔案身份證字號{0}與目前案件不合 !", CUSTID);
                    return msg;
                }           

                int TENOR = idrpSettle.TENOR + 1; // (int)worksheet.GetCellDouble(2, 1 + nCol);
                int nRow = 8;
                string IF_GRACE = "";
                string IF_LARGEPAY = "";
                //string REC_NO = "";
                DateTime? XREC_DATE = null;

                double REAL_AMT = 0;
                int nRECCNT = 0;
                UPDA_IDRP_PAYMENTREC tmpPAYREC = null;
                while (nRECCNT < TENOR)
                {
                    IF_LARGEPAY = worksheet.GetCellString(nRow, 0).Trim();
                    IF_GRACE = worksheet.GetCellString(nRow, 1).Trim();

                    // REC_NO = worksheet.GetCellString(nRow, 0 + nCol);
                    // if (String.IsNullOrEmpty(REC_NO))
                    //    break;

                    XREC_DATE = worksheet.GetCellDateTime(nRow, 1 + nCol);
                    if (XREC_DATE == null)
                        break;

                    REAL_AMT = worksheet.GetCellDouble(nRow, 2 + nCol);
                    if (REAL_AMT == 0)
                        break;

                    tmpPAYREC = new UPDA_IDRP_PAYMENTREC();
                    tmpPAYREC.SEQNO = 0;
                    tmpPAYREC.PSEQNO = PSEQNO;
                    tmpPAYREC.REC_NO = nRECCNT++;
                    tmpPAYREC.REC_DATE = new DateTime(XREC_DATE.Value.Year, XREC_DATE.Value.Month, idrpSettle.FIRSTPAY_D.Value.Day);
                    tmpPAYREC.REM_AMT = (decimal)REAL_AMT;
                    tmpPAYREC.IF_LARGEPAY = (IF_LARGEPAY == "V" || IF_LARGEPAY == "v");
                    tmpPAYREC.IF_GRACE = (IF_GRACE == "V" || IF_GRACE == "v");
                    tmpPAYREC.SPD_AMT = 0;

                    lstPayREC.Add(tmpPAYREC);

                    if (tmpPAYREC.IF_GRACE)
                        TENOR++;
                    nRow++;
                }


            }
            if (lstPayREC.Count > 0)
            {
                msg = UpdatePAYMENTREC(PSEQNO, lstPayREC);
            }
            else
            {
                msg.Message = "無更新任何資料!";
            }

            return msg;
        }
        /*
        public void CreatePDF(ControllerContext context,int nSEQNO)
        {
            IDRPCASEViewModel model = this.GetCase(nSEQNO);

            ViewDataDictionary viewData = new ViewDataDictionary(model);

            // The string writer where to render the HTML code of the view
            StringWriter stringWriter = new StringWriter();

            // Render the Index view in a HTML string
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(context, "IDRP9012D0", null);
            ViewContext viewContext = new ViewContext(
                    context,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(),
                    stringWriter
                    );
            viewResult.View.Render(viewContext, stringWriter);

            string destinationPath = "C:\\Tmp\\TEST.PDF";

            System.IO.File.WriteAllBytes(destinationPath,   ConvertHtmlTextToPDF(stringWriter.ToString()));
        }*/

        public class HeaderFooterPageEvent : PdfPageEventHelper
        {
            private iTextSharp.text.Font bf = null;
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                base.OnOpenDocument(writer, document);
                BaseFont chBaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                bf = new iTextSharp.text.Font(chBaseFont, 9, 0);
            }
            //A4 page measures 595 by 842 user units. 21cm * 29.7cm = 8.27in x 11.69in  use 72 DPI = (8.27 * 72 =595.44) x (11.69 * 72 =841.68 )
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                // ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, new Phrase("Top Left", bf), 30, 830, 0);
                //ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, new Phrase("Top Right", bf), 550, 830, 0);
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                //ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, new Phrase("http://www.citibank.com/", bf), 110, 25, 0);
                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, new Phrase("page " + document.PageNumber, bf), 550, 25, 0);
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
            }

        }

        public bool CreatePDF(IDRPCASEViewModel model, UPDA_IDRP_SENDLETTER SL, Stream outputStream)
        {

            /*
            IDRPCASEViewModel model = new IDRPCASEViewModel();// this.GetCase(nSEQNO);

            model.SETTLE.CUSTID = "A123456789";
            model.SETTLE.CUSTNAME = "中文測試";
            model.SETTLE.T_ALLBANK = 1120220;
            model.SETTLE.TENOR = 120;
            model.SETTLE.PMT_INST = 5000;
            model.SETTLE.APR = 2;
            model.SETTLE.FIRSTPAY_D = new DateTime(2010, 12, 12);
            model.SETTLE.SETTLE_DATE = DateTime.Now;


            for (int i = 0; i < 120; i++)
            {
                UPDA_IDRP_PAYMENTREC rec = new UPDA_IDRP_PAYMENTREC();
                rec.REC_NO = i + 1;
                rec.REC_DATE = model.SETTLE.FIRSTPAY_D.Value.AddMonths(i);
                rec.REM_AMT = model.SETTLE.PMT_INST;
                rec.IF_GRACE = false;
                rec.IF_LARGEPAY = false;
                rec.SPD_AMT = 0;

                model.PAYMENTREC.Add(rec);
            }
            model.Calc();*/

            string sLogoImage = HttpContext.Current.Request.MapPath("~/Images/citilogo.png");
            // string sBarCodeFont = HttpContext.Current.Request.MapPath("~/Fonts/FRE3OF9X.ttf");
            string EUDC_TTF = HttpContext.Current.Request.MapPath("~/Fonts/EUDC.TTF");
            BaseFont baseEudcTTF = BaseFont.CreateFont(EUDC_TTF, BaseFont.IDENTITY_H, true);

            iTextSharp.text.Image citilogo = iTextSharp.text.Image.GetInstance(sLogoImage);
            citilogo.ScaleToFit(30f, 30f);
            //string chMingliu0FontPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts) + @"\mingliu.ttc,0"; //kaiu.ttf
            // BaseFont chBaseFont = BaseFont.CreateFont(chMingliu0FontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //var myFont = new iTextSharp.text.Font(chBaseFont, 10, 0);
            //  var bodyFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            // var myFont = FontFactory.GetFont("細明體", BaseFont.IDENTITY_H, false, 10);
            FontFactory.Register(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts) + @"\kaiu.ttf");
            // FontFactory.Register(sBarCodeFont);

            iTextSharp.text.Font myFont = FontFactory.GetFont("標楷體", BaseFont.IDENTITY_H, 9f);
            iTextSharp.text.Font myFontEUDC = new iTextSharp.text.Font(baseEudcTTF, 9f, iTextSharp.text.Font.NORMAL);

            iTextSharp.text.Font myFontX = FontFactory.GetFont("標楷體", BaseFont.IDENTITY_H, 14f);
            iTextSharp.text.Font myFontTitle = FontFactory.GetFont("標楷體", BaseFont.IDENTITY_H, 14f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font myFontTitleEUDC = new iTextSharp.text.Font(baseEudcTTF, 14f, iTextSharp.text.Font.BOLD);

            iTextSharp.text.Font redFont = FontFactory.GetFont("標楷體", BaseFont.IDENTITY_H, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(System.Drawing.Color.Red));
            //  iTextSharp.text.Font myBarCodeFont = FontFactory.GetFont("3 of 9 Barcode", BaseFont.CP1252, 20, Font.NORMAL);




            Document doc = new Document(PageSize.A4);

            iTextSharp.text.pdf.PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);

            writer.PageEvent = new HeaderFooterPageEvent();
            doc.Open();
            PdfContentByte cb = writer.DirectContent;

            if (!String.IsNullOrEmpty(SL.BARCODE))
            {


                //ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(SL.BARCODE, myBarCodeFont), 154f, 795f, 0);             
                // ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(SL.BARCODE, FontFactory.GetFont(FontFactory.TIMES_ROMAN, BaseFont.CP1252, 12, iTextSharp.text.Font.NORMAL)), 154f, 785f, 0);

                // doc.Add(new Paragraph("\n", myFontX) { IndentationLeft = 15, SpacingBefore = 20 });



                iTextSharp.text.pdf.Barcode39 code39ext = new iTextSharp.text.pdf.Barcode39();
                code39ext.Code = SL.BARCODE;// "01NA420190319007";             

                code39ext.Extended = false; //Generates extended barcode 39.
                code39ext.BarHeight = 20f; //The height of the bars.
                code39ext.X = 0.8f; //the minimum bar width
                code39ext.N = 2.2f;//Gets the bar multiplier for wide bars. NB:WB = NS:WS =1:2.5 (Recommended ratio) NB:WB = NS:WS =1:2 to 1:3

                code39ext.StartStopText = false; //Show the start and stop character '*' in the text for the barcode 39 or 'ABCD' for codabar.
                code39ext.Font = BaseFont.CreateFont(FontFactory.TIMES_ROMAN, BaseFont.CP1252, false);
                code39ext.Size = 10f; //The size of the text or the height of the shorter bar in Postnet.
                                      //code39ext.N = 2.2f;
                code39ext.Baseline = 8f; //If positive, the text distance under the bars.If zero or negative, the text distance above the bars.           
                code39ext.TextAlignment = Element.ALIGN_LEFT; //The text alignment.

                // Image imageCode39 = Image.GetInstance(code39ext.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White), (BaseColor) null);

                Image imageCode39 = code39ext.CreateImageWithBarcode(cb, null, null);
                imageCode39.SetAbsolutePosition(154f, 795f);
                doc.Add(imageCode39);

            }
            //841.68 / 3 = 280.56
            doc.Add(new Paragraph(SL.ZIP_CODE + "\n" + SL.ADDRESS, myFontX) { IndentationLeft = 15, SpacingBefore = 70 });

            //RECIPIECT SUPPORT 中文難字
            //----------------------------------------------------------------------------------
            Phrase recipiectPhrase = new Phrase();
            foreach (char nameitem in SL.RECIPIECT.ToCharArray())
            {
                if (nameitem >= 0xE000 && nameitem <= 0xF8FF) // 造字
                {
                    recipiectPhrase.Add(new Chunk(nameitem, myFontTitleEUDC));
                }
                else
                {
                    recipiectPhrase.Add(new Chunk(nameitem, myFontTitle));
                }
            }
            Paragraph recipiectParagraph = new Paragraph() { IndentationLeft = 15 };
            recipiectParagraph.Add(recipiectPhrase);


            doc.Add(recipiectParagraph);
            //----------------------------------------------------------------------------------

            //  doc.Add(new Paragraph(SL.RECIPIECT, myFontTitle) { IndentationLeft = 15 });
            doc.Add(new Paragraph("\n", myFontX) { IndentationLeft = 15, SpacingAfter = 140 });


            /*
            PdfPTable tableLetter = new PdfPTable(1);
            tableLetter.DefaultCell.Border = Rectangle.NO_BORDER;
            tableLetter.TotalWidth = 285;
            tableLetter.HorizontalAlignment = Element.ALIGN_CENTER;


            tableLetter.AddCell(new PdfPCell(new Paragraph("model.SETTLE.CUSTNAME", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            tableLetter.AddCell(new PdfPCell(new Paragraph("model.SETTLE.CUSTNAME", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });

            doc.Add(tableLetter);*/


            PdfPTable table = new PdfPTable(7);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.WidthPercentage = 85;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            table.SetWidths(new float[] { 85f, 80f, 90f, 90f, 85f, 100f, 100f });

            PdfPCell cell = new PdfPCell();

            table.AddCell(new PdfPCell(new Paragraph("總債權金額", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", model.SETTLE.T_ALLBANK), myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell(new Paragraph("姓名", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });

            //CUSTNAME SUPPORT 中文難字
            //----------------------------------------------------------------------------------
            Phrase custNamePhrase = new Phrase();
            foreach (char nameitem in model.SETTLE.CUSTNAME.ToCharArray())
            {
                if (nameitem >= 0xE000 && nameitem <= 0xF8FF) // 造字
                {
                    custNamePhrase.Add(new Chunk(nameitem, myFontEUDC));
                }
                else
                {
                    custNamePhrase.Add(new Chunk(nameitem, myFont));
                }
            }
            Paragraph custNameParagraph = new Paragraph();
            custNameParagraph.Add(custNamePhrase);
            table.AddCell(new PdfPCell(custNameParagraph) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            // table.AddCell(new PdfPCell(new Paragraph(model.SETTLE.CUSTNAME, myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });           
            table.AddCell(new PdfPCell(citilogo, false) { Border = 0, Rowspan = 3 });


            table.AddCell(new PdfPCell(new Paragraph("協商期數", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Paragraph(String.Format("{0}", model.SETTLE.TENOR), myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell(new Paragraph("身份證字號", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Paragraph(model.SETTLE.CUSTID, myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.CompleteRow();

            table.AddCell(new PdfPCell(new Paragraph("協商月付金額", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", model.SETTLE.PMT_INST), myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.CompleteRow();

            table.AddCell(new PdfPCell(new Paragraph("協商利率", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Paragraph(String.Format("{0:#,##0.00}%", model.SETTLE.APR), myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.CompleteRow();

            table.AddCell(new PdfPCell(new Paragraph("協商首繳日", myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Paragraph(String.Format("{0:yyyy/MM/dd}", model.SETTLE.FIRSTPAY_D), myFont)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });
            table.AddCell(new PdfPCell() { Border = 0 });

            table.AddCell(new PdfPCell() { Border = 0, Colspan = 7, FixedHeight = 6 });

            //doc.Add(table);

            //table = new PdfPTable(7);
            //table.DefaultCell.Border = Rectangle.NO_BORDER;
            //table.WidthPercentage = 85;
            //table.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.SetWidths(new float[] { 85f, 80f, 90f, 90f, 85f, 100f, 100f });
            table.AddCell(new PdfPCell(new Paragraph("期數", myFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
            table.AddCell(new PdfPCell(new Paragraph("繳款日期", myFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
            table.AddCell(new PdfPCell(new Paragraph("實際繳款金額", myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
            table.AddCell(new PdfPCell(new Paragraph("當期利息費用", myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
            table.AddCell(new PdfPCell(new Paragraph("本金沖抵", myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
            table.AddCell(new PdfPCell(new Paragraph("帳上累積溢繳款", myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
            table.AddCell(new PdfPCell(new Paragraph("本金帳款餘額", myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });


            table.AddCell(new PdfPCell());
            table.AddCell(new PdfPCell());
            table.AddCell(new PdfPCell());
            table.AddCell(new PdfPCell());
            table.AddCell(new PdfPCell());
            table.AddCell(new PdfPCell());
            table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", model.SETTLE.T_ALLBANK), myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

            foreach (var item in model.ITEMS)
            {
                //item.IF_SETTLE
                PdfPCell cellC1 = table.AddCell(new PdfPCell(new Paragraph(String.Format("{0}", item.REC_NO), myFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

                PdfPCell cellC2 = table.AddCell(new PdfPCell(new Paragraph(item.REC_DATE.HasValue ? String.Format("{0:yyyy/MM/dd}", item.REC_DATE) : "", myFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

                PdfPCell cellC3 = table.AddCell(new PdfPCell(new Paragraph(item.IF_PAYREC ? String.Format("${0:#,##0}", item.REAL_AMT) : "", myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

                PdfPCell cellC4 = table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", item.CALC_IntFee), myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

                PdfPCell cellC5 = table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", item.CALC_RushOff), myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

                PdfPCell cellC6 = table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", item.CALC_OverduePay), myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });

                PdfPCell cellC7 = table.AddCell(new PdfPCell(new Paragraph(String.Format("${0:#,##0}", item.CALC_Balance), item.IF_SETTLE ? redFont : myFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, UseAscender = true, ExtraParagraphSpace = 2 });
                if (item.IF_SETTLE)
                {
                    cellC1.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                    cellC2.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                    cellC3.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                    cellC4.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                    cellC5.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                    cellC6.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                    cellC7.BackgroundColor = new BaseColor(System.Drawing.Color.Yellow);
                }
            }


            doc.Add(table);
            doc.Close();
            writer.Close();
            // string destinationPath = "C:\\Tmp\\TESTX.PDF";
            //  System.IO.File.WriteAllBytes(destinationPath, outputStream.ToArray());

            return true;

        }
        public static byte[] ConvertHtmlTextToPDF(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }


            MemoryStream outputStream = new MemoryStream();//要把PDF寫到哪个串流
            byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串转成byte[]
            MemoryStream msInput = new MemoryStream(data);
            Document doc = new Document();//要寫PDF的文件，建構子沒填的話預設直式A4
            iTextSharp.text.pdf.PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件預設開啟時的縮放為100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            doc.Open();
            //使用XMLWorkerHelper把Html parse到PDF檔案裡
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
            //将pdfDest設定的資料寫道PDF檔
            PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            msInput.Close();
            outputStream.Close();
            //回傳PDF檔案
            return outputStream.ToArray();

        }




    }
}