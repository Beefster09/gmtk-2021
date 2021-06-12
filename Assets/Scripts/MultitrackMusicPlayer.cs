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
            intro.volume = targetVolume[i];
            intro.Play();
            introPlayers[i] = intro;

            var loop = gameObject.AddComponent<AudioSource>();
            loop.clip = tracks[i].loop;
            loop.volume = targetVolume[i];
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
                targetVolume[i],
                ref volumeVelocity[i],
                volumeChangeTime
            );
        }
    }

    public void SetTrackVolume(int track, float volume) {
        targetVolume[track] = volume;
    }
}

[Serializable]
public class Track {
    public AudioClip intro;
    public AudioClip loop;
    public bool startEnabled;
}
