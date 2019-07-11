  public List<AssemblyName> LoadReferences(string assemblyPath)
        {
            List<AssemblyName> mITEMS = new List<AssemblyName>();
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
            {
                string sPath = Path.Combine(Path.GetDirectoryName(assemblyPath), assemblyName.Name + ".dll");
                if(File.Exists(sPath))
                {
                    assemblyName.CodeBase = sPath;
                    if(!mITEMS.Any( c => c.Name == assemblyName.Name))
                        mITEMS.Add(assemblyName);
                    List<AssemblyName> SubITEMS = LoadReferences(sPath);
                    foreach(var sSUBNAME in SubITEMS)
                        if (!mITEMS.Any(c => c.Name == sSUBNAME.Name))
                            mITEMS.Add(sSUBNAME);
                  
 
                }
            }
            return mITEMS;
        }
