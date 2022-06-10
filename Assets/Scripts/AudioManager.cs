using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClipsList;
    List<AudioSource> audioSourcePool;
    int audioSourcePoolPreloadAmout = 10;

    private void Start()
    {
        InitializeAudioSourcePool();
    }

    private void OnEnable()
    {
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
        Board.ClickedOnAStuckTile += OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile += OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed += OnButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed += OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed += OnButtonWasClicked;
        PlayerControlls.SwippedRightToLeft += OnSpinCamera;
        PlayerControlls.SwippedLeftToRight += OnSpinCamera;
    }
    private void OnDisable()
    {
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        Board.ClickedOnAStuckTile -= OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile -= OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed -= OnButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed -= OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed -= OnButtonWasClicked;
        PlayerControlls.SwippedRightToLeft -= OnSpinCamera;
        PlayerControlls.SwippedLeftToRight -= OnSpinCamera;

    }

    private void OnSpinCamera(object sender, EventArgs e) => PlayAudio("CameraSpin");
    private void OnButtonWasClicked(object sender, EventArgs e) => PlayAudio("ButtonClicked");
    private void OnClickedOnAFreeTile(object sender, Tile e) => PlayAudio("FreeTile");
    private void OnClickedOnAStuckTile(object sender, EventArgs e) => PlayAudio("StuckTile");
    private void OnPlayerFoundTilePair(object sender, (Tile, Tile) e) => PlayAudio("FoundTilePair");

    private void PlayAudio(string clipToPlay)
    {
        AudioSource sourceToUse = GetAudioSourceFromPool();

        try
        {
            sourceToUse.clip = audioClipsList.Find(_ => _.name == clipToPlay);
        }
        catch (Exception e)
        {
        }

        sourceToUse.Play();
    }

    

    private void InitializeAudioSourcePool()
    {
        audioSourcePool = new List<AudioSource>();
        for (int i = 0; i < audioSourcePoolPreloadAmout; i++)
        {
            audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
        }
    }
    private AudioSource GetAudioSourceFromPool()
    {
        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            if (audioSourcePool[i].isPlaying == false)
            {
                return audioSourcePool[i];
            }
        }

        //All audio sources are busy, we must create a new one
        return ExpandAudioPool();
    }

    private AudioSource ExpandAudioPool()
    {
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool.Add(newAudioSource);
        return newAudioSource;
    }
}

