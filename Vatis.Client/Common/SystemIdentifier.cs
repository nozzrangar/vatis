using System;
using System.Globalization;
using System.Management;

namespace Vatsim.Vatis.Client.Common
{
    public static class SystemIdentifier
    {
        public static string GetSystemDriveVolumeId()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("SystemDrive");
            return int.Parse(new ManagementObject($"win32_logicaldisk.deviceid=\"{environmentVariable.ToUpper()}\"").Properties["VolumeSerialNumber"].Value.ToString(), NumberStyles.HexNumber).ToString();
        }
    }
}
