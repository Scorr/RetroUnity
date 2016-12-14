using System.Reflection;
using UnityEngine;

namespace RetroUnity.Utility {
    /// <summary>
    /// Android specific implementation for handling DLL loading.
    /// </summary>
    public sealed class AndroidDLLHandler : IDLLHandler {

        private Assembly currentAssembly;

        // Prevent warning on other platforms.
#pragma warning disable 0414
        private static readonly AndroidDLLHandler _instance = new AndroidDLLHandler();
#pragma warning restore 0414

        /// <summary>
        /// Prevent 'new' keyword.
        /// </summary>
        private AndroidDLLHandler() {
        }

        /// <summary>
        /// Gets the current instance (singleton).
        /// </summary>
        public static AndroidDLLHandler Instance {
            get {
#if UNITY_ANDROID
                return _instance;
#else
                Debug.LogError("This DLL handler is only compatible with Android.");
                return null;
#endif
            }
        }

        public bool LoadCore(string dllPath) {
            // TODO: Not working yet.
            using (var www = new WWW(dllPath)) {
                while (!www.isDone) { }
                currentAssembly = Assembly.Load(www.bytes);
            }
            
            return currentAssembly != null;
        }

        public void UnloadCore() {
            throw new System.NotImplementedException();
        }

        public T GetMethod<T>(string functionName) where T : class {
            throw new System.NotImplementedException();
        }
    }
}
