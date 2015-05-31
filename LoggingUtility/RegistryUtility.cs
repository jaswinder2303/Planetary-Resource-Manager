namespace LoggingUtility
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32;

    /// <summary>
    /// Utility class for reading and writing to the registry in both 32-bit and 64-bit machines.
    /// This class only supports writing to current user and local machine hives.
    /// </summary>
    public class RegistryUtility
    {
        private static RegistryUtility _instance = new RegistryUtility();
        private static UIntPtr HKEY_CURRENT_USER = (UIntPtr)0x80000001;
        private static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;

        private static int KEY_READ = 0x20019;
        private static int KEY_WOW64_64KEY = 0x0100;
        private static int KEY_WOW64_32KEY = 0x0200;

        private const int SUCCESS = 0;
        private const int BUFFER_MAX_LENGTH = 2048;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW", SetLastError = true)]
        private static extern int RegOpenKeyExW(UIntPtr hKey, string subKey, uint options, int sam, out UIntPtr phkResult);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        private static extern int RegQueryValueEx(UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType, StringBuilder lpData, ref uint lpcbData);

        [DllImport("advapi32.dll")]
        private static extern int RegCloseKey(UIntPtr hKey);

        /// <summary>
        /// Initializes the <see cref="RegistryUtility"/> class.
        /// </summary>
        static RegistryUtility()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryUtility"/> class.
        /// </summary>
        private RegistryUtility()
        {
        }

        /// <summary>
        /// Gets the Instance.
        /// </summary>
        /// <value>The registry instance.</value>
        public static RegistryUtility Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Sets the user registry value.
        /// </summary>
        /// <param name="subKeyPath">The sub key path.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">Thrown if subKeyPath, keyName or value is null or empty</exception>
        public void SetUserRegistryValue(string subKeyPath, string keyName, object value)
        {
            SetRegistryValue(Registry.CurrentUser, subKeyPath, keyName, value, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets the user registry value.
        /// </summary>
        /// <param name="subKeyPath">The sub key path.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueKind">Kind of the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if subKeyPath, keyName or value is null or empty</exception>
        public void SetUserRegistryValue(string subKeyPath, string keyName, object value, RegistryValueKind valueKind)
        {
            SetRegistryValue(Registry.CurrentUser, subKeyPath, keyName, value, valueKind);
        }

        /// <summary>
        /// Sets the machine registry value.
        /// </summary>
        /// <param name="subKeyPath">The sub key path.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">Thrown if subKeyPath, keyName or value is null or empty</exception>
        public void SetMachineRegistryValue(string subKeyPath, string keyName, object value)
        {
            SetRegistryValue(Registry.LocalMachine, subKeyPath, keyName, value, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets the machine registry value.
        /// </summary>
        /// <param name="subKeyPath">The sub key path.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueKind">Kind of the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if subKeyPath, keyName or value is null or empty</exception>
        public void SetMachineRegistryValue(string subKeyPath, string keyName, object value, RegistryValueKind valueKind)
        {
            SetRegistryValue(Registry.LocalMachine, subKeyPath, keyName, value, valueKind);
        }

        /// <summary>
        /// Sets the registry value.
        /// </summary>
        /// <param name="baseKey">The base key.</param>
        /// <param name="subKeyPath">The sub key path.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="value">The value.</param>
        private void SetRegistryValue(RegistryKey baseKey, string subKeyPath, string keyName, object value, RegistryValueKind valueKind)
        {
            lock (this)
            {
                // Check parameters
                if (string.IsNullOrEmpty(subKeyPath)) { throw new ArgumentNullException("subKeyPath", "SetRegistryValue: subKeyPath is null or empty."); }
                if (string.IsNullOrEmpty(keyName)) { throw new ArgumentNullException("keyName", "SetRegistryValue: keyName is null or empty."); }
                if (value == null) { throw new ArgumentNullException("value", "SetRegistryValue: value is null"); }

                RegistryKey subKey = null;

                try
                {
                    subKey = baseKey.CreateSubKey(subKeyPath);

                    if (subKey != null)
                    {
                        subKey.SetValue(keyName, value, valueKind);
                    }
                }
                finally
                {
                    if (subKey != null)
                    {
                        subKey.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the user registry value.
        /// </summary>
        /// <param name="regKey">The reg key without the root (HKEY).</param>
        /// <param name="valueName">Name of the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if regKey or valueName is null or empty</exception>
        /// <returns></returns>
        public string GetUserRegistryValue(string regKey, string valueName)
        {
            return GetRegistryValue(HKEY_CURRENT_USER, regKey, valueName);
        }

        /// <summary>
        /// Gets the machine registry value.
        /// </summary>
        /// <param name="regKey">The reg key without the root (HKEY).</param>
        /// <param name="valueName">Name of the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if regKey or valueName is null or empty</exception>
        /// <returns></returns>
        public string GetMachineRegistryValue(string regKey, string valueName)
        {
            return GetRegistryValue(HKEY_LOCAL_MACHINE, regKey, valueName);
        }

        /// <summary>
        /// Gets the registry value.
        /// </summary>
        /// <param name="baseKeyHandle">The base key handle.</param>
        /// <param name="regKey">The reg key.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>the value of the registry key</returns>
        private string GetRegistryValue(UIntPtr baseKeyHandle, string regKey, string valueName)
        {
            string value = string.Empty;
            // Check parameters
            if (string.IsNullOrEmpty(regKey)) { throw new ArgumentNullException("regKey", "GetRegistryValue: regKey is null or empty."); }

            // Check parameters
            if (string.IsNullOrEmpty(valueName)) { throw new ArgumentNullException("valueName", "GetRegistryValue: valueName is null or empty."); }

            try
            {
                //Read main key first
                UIntPtr regKeyHandle = GetRegistryKeyHandle(baseKeyHandle, regKey);

                //No exceptions thrown, we have a handle
                uint size = 1024;
                uint type;
                StringBuilder buffer = new StringBuilder(BUFFER_MAX_LENGTH);

                //read key value
                if (RegQueryValueEx(regKeyHandle, valueName, 0, out type, buffer, ref size) == SUCCESS)
                {
                    value = buffer.ToString();
                }

                //close handle
                RegCloseKey(regKeyHandle);
            }
            catch
            {
                //key not found
            }

            return value;
        }

        /// <summary>
        /// Try to find the regsitrykey in the 64 bit part of the registry.
        /// If not found, try to find the registrykey in the 32 bit part of the registry.
        /// </summary>
        /// <param name="baseKeyHandle">The base key handle.</param>
        /// <param name="regKeyPath">The reg key path.</param>
        /// <returns>A registrykeyhandle</returns>
        private UIntPtr GetRegistryKeyHandle(UIntPtr baseKeyHandle, string regKeyPath)
        {
            UIntPtr regKeyHandle;

            // KEY_WOW64_64KEY
            // Access a 64-bit key from either a 32-bit or 64-bit application (not supported on Windows 2000).
            // 64-bit key = all keys in HKEY_LOCAL_MACHINE\Software except the HKEY_LOCAL_MACHINE\Software\Wow6432Node
            //
            // Check if the registrykey can be found in the 64 bit registry part of the register
            if (RegOpenKeyExW(baseKeyHandle, regKeyPath, 0, KEY_READ | KEY_WOW64_64KEY, out regKeyHandle) != SUCCESS)
            {
                // KEY_WOW64_32KEY
                // Access a 32-bit key from either a 32-bit or 64-bit application. (not supported on Windows 2000)
                // 32-bit key = all keys in HKEY_LOCAL_MACHINE\Software\Wow6432Node
                //
                // Check if the registrykey can be found in the 32 bit registry part of the register
                if (RegOpenKeyExW(baseKeyHandle, regKeyPath, 0, KEY_READ | KEY_WOW64_32KEY, out regKeyHandle) != SUCCESS)
                {
                    throw new KeyNotFoundException(string.Format(@"GetRegistryKeyHandle: Could not find registrykey {1}", regKeyPath));
                }
            }

            return regKeyHandle;
        }
    }
}

