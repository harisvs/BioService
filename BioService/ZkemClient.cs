using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;
namespace BioService
{
    class ZkemClient
    {
        CZKEM objCZKEM = new CZKEM();
        Action<object, string> RaiseDeviceEvent;
        public const string acx_Connect = "Connected";
        public const string acx_NewUser = "NewUser";

        public bool Connect_Net(string IPAdd, int Port)
        {
            if (objCZKEM.Connect_Net(IPAdd, Port))
            {
                //65535, 32767
                if (objCZKEM.RegEvent(1, 32767))
                {
                    // [ Register your events here ]
                    // [ Go through the _IZKEMEvents_Event class for a complete list of events
                    objCZKEM.OnConnected += ObjCZKEM_OnConnected;
                    objCZKEM.OnDisConnected += objCZKEM_OnDisConnected;
                    //objCZKEM.OnEnrollFinger += ObjCZKEM_OnEnrollFinger;
                    //objCZKEM.OnFinger += ObjCZKEM_OnFinger;
                    //objCZKEM.OnWriteCard += ObjCZKEM_OnNewUser;
                    objCZKEM.OnAttTransactionEx += new _IZKEMEvents_OnAttTransactionExEventHandler(zkemClient_OnAttTransactionEx);
                }
                return true;
            }
            return false;
        }

        private void ObjCZKEM_OnConnected()
        {
            RaiseDeviceEvent(this, UniversalStatic.acx_Connect);
        }
        void objCZKEM_OnDisConnected()
        {
            // Implementing the Event
            RaiseDeviceEvent(this, UniversalStatic.acx_Disconnect);
        }
        private void zkemClient_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            string time = Year + "-" + Month + "-" + Day + " " + Hour + ":" + Minute + ":" + Second;
            //breakpoints here do not stop
            string text = "Verify OK.UserID=" + EnrollNumber.ToString() + " isInvalid=" + IsInValid.ToString() + " state=" + AttState.ToString() + " verifystyle=" + VerifyMethod.ToString() + " time=" + time;
        }
        public bool ReadAllUserID(int dwMachineNumber)
        {
            return objCZKEM.ReadAllUserID(dwMachineNumber);
        }
        public bool ReadAllTemplate(int dwMachineNumber)
        {
            return objCZKEM.ReadAllTemplate(dwMachineNumber);
        }
        public bool SSR_GetAllUserInfo(int dwMachineNumber, out string dwEnrollNumber, out string Name, out string Password, out int Privilege, out bool Enabled)
        {
            return objCZKEM.SSR_GetAllUserInfo(dwMachineNumber, out dwEnrollNumber, out Name, out Password, out Privilege, out Enabled);
        }

        public bool GetUserTmpExStr(int dwMachineNumber, string dwEnrollNumber, int dwFingerIndex, out int Flag, out string TmpData, out int TmpLength)
        {
            return objCZKEM.GetUserTmpExStr(dwMachineNumber, dwEnrollNumber, dwFingerIndex, out Flag, out TmpData, out TmpLength);
        }

        public void Disconnect()
        {
            // [ Unregister events
            objCZKEM.OnConnected -= ObjCZKEM_OnConnected;
            objCZKEM.OnDisConnected -= objCZKEM_OnDisConnected;
            //objCZKEM.OnEnrollFinger -= ObjCZKEM_OnEnrollFinger;
            //objCZKEM.OnFinger -= ObjCZKEM_OnFinger;
            objCZKEM.OnAttTransactionEx -= new _IZKEMEvents_OnAttTransactionExEventHandler(zkemClient_OnAttTransactionEx);

            objCZKEM.Disconnect();
        }

        public bool ReadAllGLogData(int dwMachineNumber)
        {
            return objCZKEM.ReadAllGLogData(dwMachineNumber);
        }
        public bool SSR_GetGeneralLogData(int dwMachineNumber, out string dwEnrollNumber, out int dwVerifyMode, out int dwInOutMode, out int dwYear, out int dwMonth, out int dwDay, out int dwHour, out int dwMinute, out int dwSecond, ref int dwWorkCode)
        {
            return objCZKEM.SSR_GetGeneralLogData(dwMachineNumber, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode);
        }
        public bool GetAllUserID(int dwMachineNumber, ref int dwEnrollNumber, ref int dwEMachineNumber, ref int dwBackupNumber, ref int dwMachinePrivilege, ref int dwEnable)
        { return objCZKEM.GetAllUserID(dwMachineNumber, dwEnrollNumber, dwEMachineNumber, dwBackupNumber, dwMachinePrivilege, dwEnable); }
        public bool SSR_GetUserInfo(int dwMachineNumber, string dwEnrollNumber, out string Name, out string Password, out int Privilege, out bool Enabled)
        {
            return objCZKEM.SSR_GetUserInfo(dwMachineNumber, dwEnrollNumber, out Name, out Password, out Privilege, out Enabled);
        }
        public bool GetUserTmpEx(int dwMachineNumber, string dwEnrollNumber, int dwFingerIndex, out int Flag, out byte TmpData, out int TmpLength)
        {
            return objCZKEM.GetUserTmpEx(dwMachineNumber, dwEnrollNumber, dwFingerIndex, out Flag, out TmpData, out TmpLength);
        }
        public bool SSR_SetUserInfo(int dwMachineNumber, string dwEnrollNumber, string Name, string Password, int Privilege, bool Enabled)
        {
            return objCZKEM.SSR_SetUserInfo(dwMachineNumber, dwEnrollNumber, Name, Password, Privilege, Enabled);
        }
        public bool BeginBatchUpdate(int dwMachineNumber, int UpdateFlag)
        {
            return objCZKEM.BeginBatchUpdate(dwMachineNumber, UpdateFlag);
        }
        public bool ClearData(int dwMachineNumber, int DataFlag)
        {
            return objCZKEM.ClearData(dwMachineNumber, DataFlag);
        }

        public bool RefreshData(int dwMachineNumber)
        {
            return objCZKEM.RefreshData(dwMachineNumber);
        }

        public void GetLastError(ref int dwErrorCode)
        {
            objCZKEM.GetLastError(dwErrorCode);
        }
        public bool ClearGLog(int dwMachineNumber)
        {
            return objCZKEM.ClearGLog(dwMachineNumber);
        }

        public bool GetFirmwareVersion(int dwMachineNumber, ref string strVersion)
        { return objCZKEM.GetFirmwareVersion(dwMachineNumber, strVersion); }

        public bool GetVendor(ref string strVendor)
        { return objCZKEM.GetVendor(strVendor); }

        public bool GetWiegandFmt(int dwMachineNumber, ref string sWiegandFmt)
        { return objCZKEM.GetWiegandFmt(dwMachineNumber, sWiegandFmt); }
        public bool GetSDKVersion(ref string strVersion)
        { return objCZKEM.GetSDKVersion(strVersion); }

        public bool GetSerialNumber(int dwMachineNumber, out string dwSerialNumber)
        { return objCZKEM.GetSerialNumber(dwMachineNumber, out dwSerialNumber); }

        public bool GetDeviceMAC(int dwMachineNumber, ref string sMAC)
        { return objCZKEM.GetDeviceMAC(dwMachineNumber, sMAC); }

        public bool SetUserTmpExStr(int dwMachineNumber, string dwEnrollNumber, int dwFingerIndex, int Flag, string TmpData)
        {
            return objCZKEM.SetUserTmpExStr(dwMachineNumber, dwEnrollNumber, dwFingerIndex, Flag, TmpData);
        }

    }
}
