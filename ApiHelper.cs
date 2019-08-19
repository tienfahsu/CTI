 
 
 public class ApiHelper
 {
        public MessageStatus ZipCompress_WithZipName(List<MSFile> aryMS, string fileZipName, string SOEID, string PWD = "Qwertyuiop[]\\01234")
        {
            //string SOEID = PersonInfo.PersonMaster.PersonKey;
            string sZIPFileFolder = ZipApi.GetZipFilePath(); // from  DB Config OR AppSettings
            if (String.IsNullOrEmpty(sZIPFileFolder))
            {
                sZIPFileFolder = System.Web.Configuration.WebConfigurationManager.AppSettings["ExportExcel"];
                sZIPFileFolder = String.IsNullOrWhiteSpace(sZIPFileFolder) ? "~/" : sZIPFileFolder;
            }
            string sZipFile = string.Format("{0}_{1}_{2}", fileZipName, SOEID, DateTime.Now.ToString("yyyyMMddhhmmss"));
            string sZipFilePath = Path.Combine(Server.MapPath(sZIPFileFolder), sZipFile);
            ZipApi.Folder = sZIPFileFolder; // save zip folder 

            MessageStatus msg = ZipApi.Compress(sZipFile, aryMS, sZipFilePath, SOEID, PWD);
            if (msg.Status)
            {
                msg.File?.All(x => { x.url = VirtualPathUtility.ToAbsolute(x.url); return true; });
            }
            return msg;
        }
}
