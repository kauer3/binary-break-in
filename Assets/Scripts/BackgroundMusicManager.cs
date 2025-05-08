using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public List<AudioClip> songs;
    public bool onHold = false;
    AudioSource audioSource;
    int selectedTrack;
    List<int> trackHistory;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SelectNextTrack();
    }

    // Update is called once per frame
    void Update()
    {
        if (!onHold)
        {
            PlayBackgroundMusic();
        }
    }

    void PlayBackgroundMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(songs[selectedTrack]);

            int currentTrack = selectedTrack;
            while (selectedTrack == currentTrack)
            {
                SelectNextTrack();
            }
        }
    }

    void SelectNextTrack()
    {
        selectedTrack = Random.Range(0, songs.Count);
    }

    public void StopBackgroundMusic()
    {
        onHold = true;
        audioSource.Stop();
    }
}
