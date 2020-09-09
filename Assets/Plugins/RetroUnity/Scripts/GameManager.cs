using System.IO;
using RetroUnity.Utility;
using UnityEngine;

namespace RetroUnity {
    public class GameManager : MonoBehaviour {

        [SerializeField] private string CoreName = "snes9x_libretro";
        [SerializeField] private string RomName = "Chrono Trigger (USA).sfc";
        private LibretroWrapper.Wrapper wrapper;

        private float _frameTimer;

        public Renderer Display;

        private void Awake() {
            LoadRom(Application.streamingAssetsPath + "/" + RomName);
        }

        private void Update() {
            if (wrapper != null) {
                _frameTimer += Time.deltaTime;
                float timePerFrame = 1f / (float)wrapper.GetAVInfo().timing.fps;

                while (_frameTimer >= timePerFrame)
                {
                    wrapper.Update();
                    _frameTimer -= timePerFrame;
                }
            }
            if (LibretroWrapper.tex != null) {
                Display.material.mainTexture = LibretroWrapper.tex;
            }
        }

        public void LoadRom(string path) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            // If the file doesn't exist the application gets stuck in a loop.
            if (!File.Exists(path))
            {
                Debug.LogError(path + " not found.");
                return;
            }
#endif
            Display.material.color = Color.white;

            wrapper = new LibretroWrapper.Wrapper(CoreName);

            wrapper.Init();
            wrapper.LoadGame(path);
        }

        private void OnDestroy() {
            wrapper.DLLHandler.UnloadCore();
        }
    }
}
