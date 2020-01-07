using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace DataAccess
{
    public class DataDal
    {
     
        public string FilePath = ConfigurationSettings.AppSettings["FilePath"];
        string fp = System.Reflection.Assembly.GetEntryAssembly().Location;
        public string Constring = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public Response ExecuteNonQuery(string ProcName, List<SqlParameter> prms,string MethodName, object ObjToSerialize)
        {         
            Response response = new Response();
            Errorlog errorlog = new Errorlog();
            Servicelog servicelog = new Servicelog();
            string s = Serialize(ObjToSerialize);
            servicelog.RID = 0;
            servicelog.Flag = 1;
            servicelog.MethodName = MethodName;
            //servicelog.REQUEST_SRC = "BIOSERVICE"
            //servicelog.REQUEST_KEY = ip
            servicelog.REQUEST_MSG = s;
            servicelog.STATUS = "REQUESTED";
            servicelog.CREATEDBY = "SYS";
         
            Ins_ServiceLog(servicelog);         
            try
            {
                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        DataSet ds = new DataSet();
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = ProcName;
                        cmd.Parameters.Clear();
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        if (prms != null)
                        {
                            cmd.Parameters.Clear();
                            foreach (SqlParameter p in prms)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                response.StatusCode = dt.Rows[0]["ID"].ToString();
                                response.Status = dt.Rows[0]["Status"].ToString();
                                response.Description = dt.Rows[0]["Description"].ToString();
                            }
                        }                      
                    }
                }
            }
            catch (Exception Ex)
            {
                errorlog.MethodName = MethodName;
                errorlog.MESSAGE = Ex.Message.ToString();
                Ins_Error(errorlog);                 
            }
            return response;
        }

        public void Ins_ServiceLog(Servicelog ObjServicelog)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "UPS_LOGService";
                    cmd.Parameters.AddWithValue("@RID",                  ObjServicelog.RID);
                    cmd.Parameters.AddWithValue("@Flag",                 ObjServicelog.Flag);                   
                    cmd.Parameters.AddWithValue("@MethodName",           ObjServicelog.MethodName);
                    cmd.Parameters.AddWithValue("@REQUEST_SRC",          "BIOSERVICE");
                    cmd.Parameters.AddWithValue("@REQUEST_KEY",          ObjServicelog.REQUEST_KEY);
                    cmd.Parameters.AddWithValue("@REQUEST_MSG",          ObjServicelog.REQUEST_MSG);
                    cmd.Parameters.AddWithValue("@STATUS",               ObjServicelog.STATUS);
                    cmd.Parameters.AddWithValue("@RESPONSE_CODE",        ObjServicelog.RESPONSE_CODE);
                    cmd.Parameters.AddWithValue("@RESPONSE_DESCRIPTION", ObjServicelog.RESPONSE_DESCRIPTION);
                    cmd.Parameters.AddWithValue("@RESPONSE_MSG",         ObjServicelog.RESPONSE_MSG);
                    cmd.Parameters.AddWithValue("@CREATEDBY",            ObjServicelog.CREATEDBY);    
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteScalar();
                    con.Close();
                }
            }
            catch (Exception Ex)
            {
                WriteToFile(Ex.Message.ToString());
                WriteToFile(Ex.StackTrace.ToString());
                //Ins_Error(errorlog);
            }
        }

        public void Ins_Error(Errorlog errorlog)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Constring))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SPS_LogError";
                    cmd.Parameters.AddWithValue("@Description",errorlog.Description);
                    cmd.Parameters.AddWithValue("@ERRORCODE",  errorlog.ERRORCODE);
                    cmd.Parameters.AddWithValue("@ERRORLEVEL", errorlog.ERRORLEVEL);
                    cmd.Parameters.AddWithValue("@ErrorRefNo", errorlog.ErrorRefNo);
                    cmd.Parameters.AddWithValue("@ERROR_TYPE", errorlog.ERROR_TYPE);
                    cmd.Parameters.AddWithValue("@MESSAGE",    errorlog.MESSAGE);
                    cmd.Parameters.AddWithValue("@MethodName", errorlog.MethodName);
                    cmd.Parameters.AddWithValue("@SOURCE",     errorlog.SOURCE);
                    cmd.Parameters.AddWithValue("@STATUS",     errorlog.STATUS);
                    cmd.Parameters.AddWithValue("@CreatedBy",  errorlog.CREATEDBY);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteScalar();
                    con.Close();                 
                }
            }
            catch (Exception Ex)
            {
                WriteToFile(Ex.Message.ToString());
                WriteToFile(Ex.StackTrace.ToString());
            }            
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }


        public string Serialize(object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }

        public string GetCurrentMethod()
        {
            string method = string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            return method;
        }

    }
}
