using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
     
        public class Errorlog
        {          
            public string SOURCE          { get; set; }
            public string ERROR_TYPE      { get; set; }
            public string ERRORLEVEL      { get; set; }
            public string ERRORCODE       { get; set; }
            public string MESSAGE         { get; set; }
            public string MethodName      { get; set; }
            public string Description     { get; set; }
            public string ErrorRefNo      { get; set; }
            public string STATUS          { get; set; }
            public string CREATEDBY { get; set; }
        }


        public class Servicelog
        {
           public int    RID                  { get; set; }
           public int    Flag                 { get; set; }        
           public string MethodName           { get; set; }
           public string REQUEST_SRC          { get; set; }
           public string REQUEST_KEY          { get; set; }
           public string REQUEST_MSG          { get; set; }
           public string LOG_DATE             { get; set; }
           public string REQUEST_TIME         { get; set; }
           public string STATUS               { get; set; }
           public string RESPONSE_CODE        { get; set; }
           public string RESPONSE_DESCRIPTION { get; set; }
           public string RESPONSE_MSG         { get; set; }
           public string RESPONSE_TIME        { get; set; }
           public string CREATEDBY            { get; set; }
      
        }


        public class Response
        {
           public string StatusCode { get; set; }
           public string Status { get; set; }
           public string Description { get; set; }
        }    
}
