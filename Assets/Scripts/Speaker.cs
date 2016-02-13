using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour {

    private AudioSource _speaker;
    private float[] _newData = new float[LibretroWrapper.Wrapper.AudioBatchSize];

    private void Start() {
        _speaker = GetComponent<AudioSource>();
        if (_speaker == null) return;
        AudioClip clip = AudioClip.Create("Libretro", 1024, 2, 44100, true, OnAudioRead);
        _speaker.clip = clip;
        _speaker.Play();
        Debug.Log("Unity sample rate: " + AudioSettings.outputSampleRate);
    }

    public void UpdateAudio(float[] data) {
        _newData = data;
        _speaker.Play();
    }

    private void OnAudioRead(float[] data) {
        Array.Copy(_newData, data, data.Length);
    }
}
