using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RetroUnity.Utility {
    /// <summary>
    /// Windows specific implementation for handling DLL loading. Requires kernel32.dll.
    /// </summary>
    public sealed class WindowsDLLHandler : IDLLHandler {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static IntPtr _dllPointer = IntPtr.Zero;

        // Prevent warning on other platforms.
#pragma warning disable 0414
        private static readonly WindowsDLLHandler _instance = new WindowsDLLHandler();
#pragma warning restore 0414

        /// <summary>
        /// Prevent 'new' keyword.
        /// </summary>
        private WindowsDLLHandler() {
        }

        /// <summary>
        /// Gets the current instance (singleton).
        /// </summary>
        public static WindowsDLLHandler Instance {
            get {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return _instance;
#else
                Debug.LogError("This DLL handler is only compatible with Windows.");
                return null;
#endif
            }
        }

        public bool LoadCore(string dllPath) {
            _dllPointer = LoadLibrary(dllPath);

            if (_dllPointer == IntPtr.Zero) {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.LogErrorFormat("Failed to load library (ErrorCode: {0})", errorCode);
                return false;
            }

            return true;
        }

        public void UnloadCore() {
            FreeLibrary(_dllPointer);
        }

        public T GetMethod<T>(string functionName) where T : class {
            if (_dllPointer == IntPtr.Zero) {
                Debug.LogError("DLL not found, cannot get method '" + functionName + "'");
                return default(T);
            }

            IntPtr pAddressOfFunctionToCall = GetProcAddress(_dllPointer, functionName);

            if (pAddressOfFunctionToCall == IntPtr.Zero) {
                Debug.LogError("Address for function " + functionName + " not found.");
                return default(T);
            }

            return Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(T)) as T;
        }
    }
}