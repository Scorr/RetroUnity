#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace RetroUnity
{
    public class CoreDownloader
    {
        // http://buildbot.libretro.com/nightly
        static Dictionary<string, string> cores = new Dictionary<string, string>()
        {
            { "http://buildbot.libretro.com/nightly/windows/x86_64/latest/snes9x_libretro.dll.zip", Application.streamingAssetsPath } ,
            { "http://buildbot.libretro.com/nightly/linux/x86_64/latest/snes9x_libretro.so.zip", Application.streamingAssetsPath } ,
            { "http://buildbot.libretro.com/nightly/apple/osx/x86_64/latest/snes9x_libretro.dylib.zip", Application.streamingAssetsPath }, 
            { "http://buildbot.libretro.com/nightly/android/latest/armeabi-v7a/snes9x_libretro_android.so.zip", Path.Combine(Application.dataPath,"Plugins") }, 
        };

        
        [MenuItem("Libretro/Download cores")]
        static void DownloadCores()
        {
            CoreDownloader coreDownloader = new CoreDownloader();
            foreach(var item in cores)
            {
                string url = item.Key;
                string extractDirectory = item.Value;
                string zipPath = coreDownloader.DownloadFile(url, extractDirectory);
                Debug.Log("File successfully downloaded and saved to " + zipPath);
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(extractDirectory, entry.FullName));
                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }    
                        entry.ExtractToFile(destinationPath);
                    }
                    File.Delete(zipPath);
                }                
                
                Debug.Log("Unzipping successfully downloaded and saved to " + item.Value);
            }

            setNativeLibraryForAndroid();
        }

        [MenuItem("Libretro/Set Native Library for Android")]
        static void setNativeLibraryForAndroid()
        {
            // native library, avaiable only for android
            PluginImporter androidPlugin = AssetImporter.GetAtPath("Assets/Plugins/snes9x_libretro_android.so") as PluginImporter;
            androidPlugin.SetCompatibleWithEditor(false);
            androidPlugin.SetCompatibleWithAnyPlatform(false);
            androidPlugin.SetCompatibleWithPlatform(BuildTarget.Android, true);
            androidPlugin.SetPlatformData(BuildTarget.Android, "CompileFlags", "-fno-objc-arc");                        
        }

        String DownloadFile(string url, string directory)
        {
                using (WebClient webClient = new WebClient())
                {
                    string fileName = Path.GetFileName(url);
                    string filePath = Path.Combine(directory, fileName);
                    webClient.DownloadFile(new Uri(url), filePath);
                    return filePath;
                }         
        }
        
    }
}

#endif
