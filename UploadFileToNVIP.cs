 public static MessageStatus UploadFileToNVIP(Stream sourceFile, string saveFileName, string account, string password)
        {
            string _errorMsg = string.Empty;
            MessageStatus message = new MessageStatus();
            string pathNVIP = WebConfigurationManager.AppSettings["NVIP_API_PATH"];
            string User172 = WebConfigurationManager.AppSettings["User172"];

            //string[] domainUser = new string[] { null, User172 };
            //if (User172.IndexOf('\\') >=0)
            try
            {
                String[] domainUser = User172.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                //ImpersonationHelper ih = new ImpersonationHelper(domainUser[0], domainUser[1], password);
                using (ImpersonateHelper ih = new ImpersonateHelper(domainUser[0], domainUser[1], password))
                {

                    //WindowsIdentity useri = WindowsIdentity.GetCurrent();
                    // ConnectNetDrive(pathNVIP, account, password, out _errorMsg);
                    string targetFile = Path.Combine(pathNVIP, saveFileName);
                    try
                    {
                        using (FileStream saveFile = new FileStream(targetFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            sourceFile.CopyTo(saveFile);
                            saveFile.Close();
                            saveFile.Dispose();
                        }
                        message.Status = true;
                    }
                    catch (Exception e)
                    {
                        message.Status = false;
                        message.Message += e.Message;
                    }

                }
            }
            catch (Exception e)
            {
                message.Status = false;
                message.Message = e.Message;
            }
            return message;
        }
