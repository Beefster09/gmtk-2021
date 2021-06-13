using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultitrackMusicPlayer : MonoBehaviour
{
    [SerializeField]
    Track[] tracks;

    AudioSource[] introPlayers;
    AudioSource[] loopPlayers;

    float[] targetVolume;
    float[] volumeVelocity;

    public float volumeChangeTime = 0.25f;
    [Range(0f, 1f)]
    public float masterVolume = 0.7f;
    // Start is called before the first frame update
    void Start()
    {
        introPlayers = new AudioSource[tracks.Length];
        loopPlayers = new AudioSource[tracks.Length];
        targetVolume = new float[tracks.Length];
        volumeVelocity = new float[tracks.Length];
        var dspNow = AudioSettings.dspTime;
        for (int i = 0; i < tracks.Length; i++) {
            targetVolume[i] = tracks[i].startEnabled? 1f : 0f;

            var intro = gameObject.AddComponent<AudioSource>();
            intro.clip = tracks[i].intro;
            intro.volume = targetVolume[i] * masterVolume * tracks[i].volume;
            intro.Play();
            introPlayers[i] = intro;

            var loop = gameObject.AddComponent<AudioSource>();
            loop.clip = tracks[i].loop;
            loop.volume = targetVolume[i] * masterVolume * tracks[i].volume;
            loop.loop = true;
            loop.PlayScheduled(dspNow + tracks[0].intro.length);
            loopPlayers[i] = loop;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < tracks.Length; i++) {
            introPlayers[i].volume = loopPlayers[i].volume = Mathf.SmoothDamp(
                loopPlayers[i].volume,
                targetVolume[i] * masterVolume * tracks[i].volume,
                ref volumeVelocity[i],
                volumeChangeTime
            );
        }
    }

    public void SetTrackVolume(int track, float volume) {
        targetVolume[track] = volume;
    }

    public void StopAll() {
        for (int i = 0; i < tracks.Length; i++) {
            if (introPlayers[i].isPlaying) introPlayers[i].Stop();
            if (loopPlayers[i].isPlaying) {
                loopPlayers[i].Stop();
            }
            else {
                loopPlayers[i].enabled = false;
                loopPlayers[i].SetScheduledEndTime(AudioSettings.dspTime);
            }
        }
    }
}

[Serializable]
public class Track {
    public AudioClip intro;
    public AudioClip loop;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool startEnabled;
}
