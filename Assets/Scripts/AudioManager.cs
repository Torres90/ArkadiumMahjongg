using CustomHelperFunctions;
using System;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField] List<AudioClip> audioClipsList;
    List<AudioSource> audioSourcePool;
    int audioSourcePoolPreloadAmout = 5;
    AudioSource audioSourcePlayingLoopingMusic;

    private void Start()
    {
        InitializeAudioSourcePool();
        PlayLoopingMusic("Run For It");
    }

    private void OnEnable()
    {
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
        Board.ClickedOnAStuckTile += OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile += OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed += OnButtonWasClicked;
        PauseButton.PauseButtonWasPressed += OnPauseButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed += OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed += OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed += OnResumeButtonWasClicked;
        PlayerControlls.SwippedRightToLeft += OnSpinCamera;
        PlayerControlls.SwippedLeftToRight += OnSpinCamera;
        StartGameButton.LoadMainScene += OnLoadMainScene;
    }
    private void OnDisable()
    {
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
        Board.ClickedOnAStuckTile -= OnClickedOnAStuckTile;
        Board.ClickedOnAFreeTile -= OnClickedOnAFreeTile;
        PauseButton.PauseButtonWasPressed -= OnButtonWasClicked;
        PauseButton.PauseButtonWasPressed += OnPauseButtonWasClicked;
        RestartGameButton.RestartGameButtonPressed -= OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed -= OnButtonWasClicked;
        ResumeButton.ResumeButtonWasPressed -= OnResumeButtonWasClicked;
        PlayerControlls.SwippedRightToLeft -= OnSpinCamera;
        PlayerControlls.SwippedLeftToRight -= OnSpinCamera;
        StartGameButton.LoadMainScene -= OnLoadMainScene;

    }

    //Music
    private void OnResumeButtonWasClicked(object sender, EventArgs e) => PlayLoopingMusic("Bubblegum Pop Hiphop");
    private void OnPauseButtonWasClicked(object sender, EventArgs e) => PlayLoopingMusic("Low Growl");

    //Sound Effects
    private void OnLoadMainScene(object sender, EventArgs e) => PlayLoopingMusic("Bubblegum Pop Hiphop");
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

    private void PlayLoopingMusic(string clipToPlay)
    {
        if (audioSourcePlayingLoopingMusic == null)
        {
            audioSourcePlayingLoopingMusic = GetAudioSourceFromPool();
        }
        

        try
        {
            audioSourcePlayingLoopingMusic.clip = audioClipsList.Find(_ => _.name == clipToPlay);
        }
        catch (Exception e)
        {
        }
        audioSourcePlayingLoopingMusic.loop = true;
        audioSourcePlayingLoopingMusic.Play();
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
