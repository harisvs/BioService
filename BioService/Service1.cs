using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using zkemkeeper;
using DataAccess;
using System.Reflection;
using System.Threading;

namespace BioService
{
    public partial class Service1 : ServiceBase
    {
        Constants constants = new Constants();
        ZkemClient zkemClient = new ZkemClient();
        //Timer timer = new Timer(); // name space(using System.Timers;)  
        private bool isDeviceConnected = false;
        DeviceManipulator manipulator = new DeviceManipulator();
        Dal ObjDal = new Dal();
        DataDal ObjDataDal = new DataDal();
        Errorlog errorlog = new Errorlog();
        string MethodName = string.Empty;
        Constants ObjConst = new Constants();
        private System.Timers.Timer m_mainTimer;
        private bool m_timerTaskSuccess;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //ObjDataDal.WriteToFile("Service is started at " + DateTime.Now);
            //timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            //double IntervalTime = double.Parse(ConfigurationSettings.AppSettings[constants.IntervalTime.ToString()]);
            //timer.Interval = IntervalTime; //number in milisecinds  
            //timer.Enabled = true;
           


            try
            {                
                m_mainTimer = new System.Timers.Timer();
                m_mainTimer.Interval = 60000;   // every one min
                m_mainTimer.Elapsed += m_mainTimer_Elapsed;
                m_mainTimer.AutoReset = false;  // makes it fire only once
                m_mainTimer.Start(); // Start
                m_timerTaskSuccess = false;
            }
            catch (Exception ex)
            {
                ObjDataDal.WriteToFile("Service is started at " + ex.Message.ToString() + DateTime.Now);
            }
        }

        void m_mainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                ObjDataDal.WriteToFile("ConnectDevice is starting " + DateTime.Now);
                ConnectDevice();

                m_timerTaskSuccess = true;
            }
            catch (Exception ex)
            {
                m_timerTaskSuccess = false;
            }
            finally
            {
                if (m_timerTaskSuccess)
                {
                    m_mainTimer.Start();
                }
            }
        }

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    //ShowStatusBar("The device is connected !!", true);
                    //btnConnect.Text = "Disconnect";
                    //ToggleControls(true);
                }
                else
                {
                    //ShowStatusBar("The device is diconnected !!", true);
                    zkemClient.Disconnect();
                    //btnConnect.Text = "Connect";
                    //ToggleControls(false);
                }
            }
        }

        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        //ShowStatusBar("The device is switched off", true);
                        //DisplayEmpty();
                        //btnConnect.Text = "Connect";
                        //ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }

        public void ConnectDevice()
        {
            MethodName = string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            try
            {                
                if (IsDeviceConnected)
                {
                    IsDeviceConnected = false;
                    //this.Cursor = Cursors.Default;
                    return;
                }
                string ipAddress = "192.168.137.2";
                int portNumber = 4370;
                //string ipAddress = tbxDeviceIP.Text.Trim();

                string port = "4370";
                if (ipAddress == string.Empty || portNumber == 0)
                    throw new Exception("The Device IP Address and Port is mandotory !!");

             
                if (!int.TryParse(port, out portNumber))
                    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

                //objZkeeper = new ZkemClient(RaiseDeviceEvent);
                IsDeviceConnected = zkemClient.Connect_Net(ipAddress, portNumber);
                int MachineNo = 1;
                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(zkemClient, MachineNo);
                    ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(zkemClient, MachineNo);
                    if(lstMachineInfo.Count > 0)
                    {
                        ObjDataDal.WriteToFile("Fetched Records : " + lstMachineInfo.Count.ToString() +" , "+ DateTime.Now);

                        bool IsInserted =  ObjDal.InsertBioLogs(lstMachineInfo);
                        if(IsInserted)
                        {
                            manipulator.ClearGLog(zkemClient, MachineNo);
                            ObjDataDal.WriteToFile("Cleared Records from MachineNo : " + MachineNo + ",  ipAddress : " + ipAddress + " , " + DateTime.Now);
                        }
                        //Insert into db


                    }
                    //ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(zkemClient, MachineNo);

                    //bool IsClear =  manipulator.ClearGLog(zkemClient, MachineNo);

                    //zkemClient.ReadAllGLogData(MachineNo);

                    //WriteToFile("Error : " + ex.Message.ToString() + DateTime.Now);
                    //lblDeviceInfo.Text = deviceInfo;
                }

            }
            catch (Exception Ex)
            {
                ObjDataDal.WriteToFile("Error : " + Ex.Message.ToString() + DateTime.Now);
                errorlog.MethodName = MethodName;
                errorlog.MESSAGE = Ex.Message.ToString();
                errorlog.Description = Ex.StackTrace.ToString();
                errorlog.ERRORLEVEL = "1";
                errorlog.MethodName = MethodName;
                errorlog.SOURCE = ObjConst.AppSource;
                errorlog.STATUS = "1"; ;
                errorlog.CREATEDBY = "SYS";
                ObjDataDal.Ins_Error(errorlog);               
                //ShowStatusBar(ex.Message, false);
            }
           // this.Cursor = Cursors.Default;
        }

        


        protected override void OnStop()
        {
            ObjDataDal.WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            ObjDataDal.WriteToFile("Service is recall at " + DateTime.Now);
        }

      
    }
}
