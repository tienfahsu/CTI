   /// <summary>
        /// Check Role Exists 
        /// </summary>
        /// <param name="sSysCode">COG, PDA, OR SC_RC = sSysCode_sRoleCode PDA_COLLECTOR </param>
        /// <param name="sRoleCode">null OR COLLECTOR</param>
        /// <returns></returns>
        public bool IsRoleExists(string sSysCode, string sRoleCode = null)
        {
            if (String.IsNullOrEmpty(sRoleCode))
            {                
                if (this.lsPersonRolesList.Where(w => w.SC_RC == sSysCode).Count() > 0)
                    return true;
            }
            else
            {
                if (this.lsPersonRolesList.Where(w => w.SysCode == sSysCode && w.RoleCode == sRoleCode).Count() > 0)
                    return true;
            }
            return false;
        }
