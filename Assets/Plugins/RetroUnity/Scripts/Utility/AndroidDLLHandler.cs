using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RetroUnity.Utility
{
    /// <summary>
    /// Android specific implementation for handling DLL loading.
    /// </summary>
    public sealed class AndroidDLLHandler : IDLLHandler
    {
        private Assembly currentAssembly;

        private static readonly AndroidDLLHandler instance = new AndroidDLLHandler();

        /// <summary>
        /// Prevent 'new' keyword.
        /// </summary>
        private AndroidDLLHandler()
        {
        }

        /// <summary>
        /// Gets the current instance (singleton).
        /// </summary>
        public static AndroidDLLHandler Instance
        {
            get
            {
#if UNITY_ANDROID
                return instance;
#else
                Debug.LogError("This DLL handler is only compatible with Android.");
                return null;
#endif
            }
        }
        
        public static void FreeLibrary (IntPtr handle)
        {
            dlclose (handle);
        }
        
        public bool LoadCore(string dllName)
        {
            // Don't neet to specify the path. Just the name
            string dllPath = dllName + "_android.so";
            Debug.Log("LoadCore: " + dllPath);
            _dllPointer = LoadLibrary(dllPath);

            if (_dllPointer == IntPtr.Zero) {
                Debug.LogError("Error loading DLL.");
                return false;
            }

            return true;
        }
        
        public static IntPtr LoadLibrary (string fileName)
        {
            IntPtr retVal = dlopen (fileName, RTLD_NOW);
            var errPtr = dlerror ();
            if (errPtr != IntPtr.Zero) {
                Debug.LogError (Marshal.PtrToStringAnsi (errPtr));
            }
            return retVal;
        }
        
        const int RTLD_NOW = 2;

        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern IntPtr dlopen (String fileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlsym")]
        private static extern IntPtr dlsym (IntPtr handle, String symbol);

        [DllImport("libdl.so", EntryPoint = "dlclose")]
        private static extern int dlclose (IntPtr handle);

        [DllImport("libdl.so", EntryPoint = "dlerror")]
        private static extern IntPtr dlerror ();

        private static IntPtr _dllPointer = IntPtr.Zero;
        

        public T GetMethod<T>(string functionName) where T : class
        {
            if (_dllPointer == IntPtr.Zero) {
                Debug.LogError("DLL not found, cannot get method '" + functionName + "'");
                return default(T);
            }

            IntPtr pAddressOfFunctionToCall = dlsym(_dllPointer, functionName);

            if (pAddressOfFunctionToCall == IntPtr.Zero) {
                return default(T);
            }

            return Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(T)) as T;
            
        }

        public void UnloadCore() {
            FreeLibrary(_dllPointer);
        }

    }
    
    
}