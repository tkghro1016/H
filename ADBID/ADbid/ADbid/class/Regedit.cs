using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ADbid
{
    public class Regedit
    {
        public Regedit()
        {
        }

        #region // 펑션 레지스트리 가져오기 및 쓰기
        // Reg_Read
        public string Reg_Read(string rKey)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ADbid");
            if (reg == null) return "";
            else return Convert.ToString(reg.GetValue(rKey));
        }

        // Reg_Write
        public void Reg_Write(string rKey, string rVal) // 키, 값, 종류
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ADbid");
            reg.SetValue(rKey, rVal);
            reg.Close();
        }

        // Reg_Del
        public void Reg_Del(string rKey, string rVal) // 키, 값, 종류
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ADbid", true);
            if (reg != null) reg.DeleteValue(rKey);
        }

        // Reg_Category
        public RegistryKey Reg_Category(string rPath)
        {
            RegistryKey reg = null;
            if (rPath.StartsWith("HKEY_CLASSES_ROOT")) reg = Registry.ClassesRoot;
            if (rPath.StartsWith("HKEY_CURRENT_USER")) reg = Registry.CurrentUser;
            if (rPath.StartsWith("HKEY_LOCAL_MACHINE")) reg = Registry.LocalMachine;
            if (rPath.StartsWith("HKEY_USERS")) reg = Registry.Users;
            if (rPath.StartsWith("HKEY_CURRENT_CONFIG")) reg = Registry.CurrentConfig;
            return reg;
        }
        #endregion
    }
}
