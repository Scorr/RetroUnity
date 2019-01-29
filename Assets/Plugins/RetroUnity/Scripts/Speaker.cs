using System;
using UnityEngine;

namespace RetroUnity {
    [RequireComponent(typeof(AudioSource))]
    public class Speaker : MonoBehaviour {

        private AudioSource _speaker;

        private void Start() {
            _speaker = GetComponent<AudioSource>();
            if (_speaker == null) return;
            //var audioConfig = AudioSettings.GetConfiguration();
            //audioConfig.sampleRate = 32000;
            //AudioSettings.Reset(audioConfig);
            //AudioClip clip = AudioClip.Create("Libretro", LibretroWrapper.Wrapper.AudioBatchSize / 2, 2, 44100, true, OnAudioRead);
            //AudioClip clip = AudioClip.Create("Libretro", 256, 2, 32000, true);
            //_speaker.clip = clip;
            _speaker.Play();
            //_speaker.loop = true;
            //Debug.Log("Unity sample rate: " + audioConfig.sampleRate);
            //Debug.Log("Unity buffer size: " + audioConfig.dspBufferSize);
        }

        private void OnAudioFilterRead(float[] data, int channels) {
            // wait until enough data is available
            if (LibretroWrapper.Wrapper.AudioBatch.Count < data.Length)
                return;
            int i;
            for (i = 0; i < data.Length; i++)
                data[i] = LibretroWrapper.Wrapper.AudioBatch[i];
            // remove data from the beginning
            LibretroWrapper.Wrapper.AudioBatch.RemoveRange(0, i);
        }

        private void OnGUI() {
            GUI.Label(new Rect(0f, 0f, 300f, 20f), LibretroWrapper.Wrapper.AudioBatch.Count.ToString());
        }
    }
}
