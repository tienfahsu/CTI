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
                worksheet.SetCellString(2, 9 + nCol, String.Format("{0}", Model.SETTLE.IF_SPAID ? "V" : "") , xTXTR_B);
                worksheet.AddMergedRegion(2, 2, 9 + nCol, 10 + nCol);
            }
