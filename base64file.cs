using System;
using System.Collections.Generic;
using System.IO; 
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BASE64FILE
{
    class Program
    {
        public static Dictionary<string, string> m_dtParamInfo = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        public static bool CommandLineParse(string[] args)
        {
            string sAction = "";
            int nPARAM = 1;

            foreach (string sArgItem in args)
            {
                if (sArgItem.StartsWith("-") || sArgItem.StartsWith("/"))
                {
                    sAction = sArgItem.Substring(1);
                    m_dtParamInfo[sAction] = sAction;
                    
                }
                else
                {
                    sAction = String.Format("FILE{0}", nPARAM++);
          
                    string sValue = sArgItem;
                    if (sValue.StartsWith("\"") && sValue.EndsWith("\""))
                    {
                        sValue = sValue.Substring(1, sArgItem.Length - 2);
                    }
                    m_dtParamInfo[sAction] = sValue;
                    sAction = "";
                }

            }

            return true;
        }
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("BASE64FILE VERSION 1.0\n");

            CommandLineParse(args);
            if (m_dtParamInfo.ContainsKey("H")  || (!m_dtParamInfo.ContainsKey("FILE1") && !m_dtParamInfo.ContainsKey("FILE2")))
            {
                 HELP();
                return;
            }
            if(!File.Exists(m_dtParamInfo["FILE1"]))
            {
                Console.WriteLine("Input File Not exists !");
                return;
            }


            if (m_dtParamInfo.ContainsKey("D"))
            {
               string sText =  File.ReadAllText(m_dtParamInfo["FILE1"]);
               byte[] binData = Convert.FromBase64String(sText);
               File.WriteAllBytes(m_dtParamInfo["FILE2"], binData);
            }
            else
            {
                Byte[] bytes = File.ReadAllBytes(m_dtParamInfo["FILE1"]);
                String fileText = Convert.ToBase64String(bytes);
                File.WriteAllText(m_dtParamInfo["FILE2"], fileText, Encoding.ASCII);
                Clipboard.SetData(DataFormats.Text, (Object)fileText);
            }          

        }
        public static void HELP()
        {
            Console.WriteLine("Convert File to Base64 txt");
            Console.WriteLine("BASE64FILE [/D] inputfile outputfile");
            Console.WriteLine("    /D  decode base64 to binary");
        }
       
    }
}
