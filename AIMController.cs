 public class AIMController : Controller
    {

        public ActionResult AIM10100()
        {
            return View(new AIMViewModel());
        }
        [HttpPost]
        public ActionResult AIM10100(AIMViewModel items)
        {
            MessageStatus msg = new MessageStatus();
            try
            {
                msg.Status = true;
                msg.Message = GetConnectingString(items);
            }
            catch (Exception ex)
            {
                msg.Status = false;
                msg.Message = ex.Message;
            }
            return Json(msg);
        }

        public string GetConnectingString(AIMViewModel items)
        {
            PSDKPasswordRequest passRequest = new PSDKPasswordRequest();
            PSDKPassword password = null;
            passRequest.AppID = items.AppID;
            passRequest.ConnectionPort = items.ConnectionPort;
            passRequest.ConnectionTimeout = items.ConnectionTimeout;
            passRequest.Safe = items.Safe;
            passRequest.Folder = items.Folder;
            passRequest.Object = items.Object;
            passRequest.Reason = items.Reason;
            passRequest.RequiredProperties.Add("PolicyId");
            passRequest.RequiredProperties.Add("UserName");
            passRequest.RequiredProperties.Add("Address");
            passRequest.RequiredProperties.Add("Database");
            bool bPasswordRetrived = false;
            int retryIntervalms = 3000;
            while (!bPasswordRetrived)
            { // Sending the request to get the password 
                password = PasswordSDK.GetPassword(passRequest);
                if (password.GetAttribute("PasswordChangeInProcess").Equals("True"))
                {
                    Thread.Sleep(retryIntervalms);
                } else
                {
                    bPasswordRetrived = true;
                }
            }
            string connetionString = "Data Source=" + password.Address + ";Initial Catalog=" + password.Database + ";User ID=" + password.UserName + ";Password=" + password.Content + ";Integrated Security=True";
            return connetionString;
        }
    }
