using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
namespace BioService
{
    public class Dal
    {
        DataDal ObjDataDal = new DataDal();
        bool IsInserted = false;       
        Errorlog errorlog = new Errorlog();
        SqlParameter[] paramss;
        
        Constants ObjConst = new Constants();
        string MethodName = string.Empty;
    
        public bool InsertBioLogs(ICollection<MachineInfo> lstMachineInfo)
        {
            MethodName = string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            try
            {
                if(lstMachineInfo.Count > 0)
                {
                    foreach (var item in lstMachineInfo)
                    {
                        var @params = new List<SqlParameter>
                        {
                            new SqlParameter("@DateOnlyRecord", item.DateOnlyRecord),
                            new SqlParameter("@DateTimeRecord", item.DateTimeRecord),
                            new SqlParameter("@IndRegID", item.IndRegID),
                            new SqlParameter("@MachineNumber", item.MachineNumber),
                            new SqlParameter("@TimeOnlyRecord", item.TimeOnlyRecord),
                            new SqlParameter("@AStatus", item.AStatus),
                            new SqlParameter("@User", item.User),
                        };
                        ObjDataDal.ExecuteNonQuery(ObjConst.proc_InsBioLogs, @params, MethodName, item);
                        IsInserted = true;
                    }                    
                }
            }
            catch (Exception Ex)
            {
                IsInserted = false;
                errorlog.MethodName = MethodName;
                errorlog.MESSAGE = Ex.Message.ToString();
                errorlog.Description = Ex.StackTrace.ToString();              
                errorlog.ERRORLEVEL = "1";
                errorlog.MethodName = MethodName;
                errorlog.SOURCE = ObjConst.AppSource;
                errorlog.STATUS = "1"; ;
                errorlog.CREATEDBY = "SYS";
                ObjDataDal.Ins_Error(errorlog);                
            }          
            return IsInserted;
        }
    }
}
