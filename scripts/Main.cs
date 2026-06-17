using Godot;
using System;
using System.IO;

public partial class Main : Control
{
    [Export] private UiController ui;
    [Export] private ParticlesManager particlesManager;
    [Export] private LyricController lyricController;

    MusicsPlayer musicsPlayer;
    ListManager listManager;
    DataManager dataManager;
    PlayerState currentPlayerState;

    public override void _EnterTree()
    {
        musicsPlayer = new MusicsPlayer();
        listManager = new ListManager();
        dataManager = new DataManager();
        currentPlayerState = PlayerState.Keep;
        GlobalEvents.EventChangeList += OnListChangeEvent;
        GlobalEvents.EventPlaySong += OnSelectSongPlay;
        GlobalEvents.EventDeleteList += OnDeleteListPressed;
        GlobalEvents.EventDeleteSong += OnDeleteSongPressed;
    }

    public override void _Ready()
    {
        DisplayServer.WindowSetMinSize(DisplayServer.WindowGetSize());

        dataManager.LoadList(listManager);
        Load();
        lyricController.InitLyrics(listManager.GetCurrentSong().lrcPath);
        ui.SetListInfo(listManager.GetListNames());
        ui.SetSongInfo(listManager.GetCurrentListSongs());
        ui.ChangeCurrentList(listManager.GetCurrentList());
        ui.SetScorllVertical(listManager.GetCurrentSong());
        GlobalEvents.EventChangeSongInfo?.Invoke(this, new SongInfo_EventArgs() { song = listManager.GetCurrentSong() });
    }

    public override void _ExitTree()
    {
        dataManager.SaveList(listManager);
        Save();
    }

    public override void _Process(double delta)
    {
        UpdateUI();
        OverPlayWithState();
        lyricController.ShowLyrics(musicsPlayer.GetCurrentTimeNoMillisecond());
    }

    private void Save()
    {
        using (StreamWriter sw = new StreamWriter("save/tr.dat"))
        {
            sw.WriteLine(currentPlayerState);
            sw.WriteLine(musicsPlayer.GetVolume());
            Song song = listManager.GetCurrentSong();
            string playgingList = listManager.GetPlayingList();
            string currentList = listManager.GetCurrentList();
            sw.WriteLine(playgingList);
            sw.WriteLine(currentList);
            sw.WriteLine(song.fileName);
            sw.WriteLine(musicsPlayer.GetPostion());
        }
    }

    private void Load()
    {
        try
        {
            using (StreamReader sr = new StreamReader("save/tr.dat"))
            {
                string playingStateStr = sr.ReadLine();
                LoadPlayrState(playingStateStr);

                string volume = sr.ReadLine();
                musicsPlayer.SetVolume(volume.ToFloat());
                ui.SetVolumeSlider(volume.ToFloat());

                string playingList = sr.ReadLine();
                listManager.SetPlayingList(playingList);

                string currentList = sr.ReadLine();
                listManager.SetCurrentList(currentList);

                string fileName = sr.ReadLine();
                Song s = listManager.GetPlayingListSongWithPath(fileName);
                listManager.SetCurrentSong(s);
                ui.SetCurrentPlayingLabel(playingList, s);
                musicsPlayer.ReadySong(s);
                musicsPlayer.isStop = false;

                string musicPosition = sr.ReadLine();
                ui.SetProcessSliderMax(musicsPlayer.GetLength());
                musicsPlayer.SetPostion(musicPosition.ToFloat());
            }
        }
        catch
        {
            using (new StreamWriter("save/tr.dat")) { }
        }
    }
    
    private void LoadPlayrState(string s)
    {
        if (s == "Keep")
        {
            currentPlayerState = PlayerState.Keep;
            ui.SetPlayingStateButtonText("[顺序播放]");
        }
        else if (s == "LoopOne")
        {
            currentPlayerState = PlayerState.LoopOne;
            ui.SetPlayingStateButtonText("[单曲循环]");
        }
        else if (s == "LoopList")
        {
            currentPlayerState = PlayerState.LoopList;
            ui.SetPlayingStateButtonText("[循环列表]");
        }
        else if (s == "Rand")
        {
            currentPlayerState = PlayerState.Rand;
            ui.SetPlayingStateButtonText("[随机播放]");
        }
    }

    private void OverPlayWithState()
    {
        if (musicsPlayer.IsPlayOver())
        {
            ui.ChangePlayingText("播放");
            switch (currentPlayerState)
            {
                case PlayerState.Keep:
                    if (!listManager.IsLastSong(listManager.GetCurrentSong()))
                    {
                        OnPlayNextSong();
                    }
                    else
                    {
                        musicsPlayer.Stop();
                    }
                    break;
                case PlayerState.LoopList:
                    OnPlayNextSong();
                    break;
                case PlayerState.LoopOne:
                    PlaySong(listManager.GetCurrentSong());
                    break;
                case PlayerState.Rand:
                    Song song = listManager.GetPlayingListSongRand();
                    PlaySong(song);
                    break;
            }
        }
    }

    private void UpdateUI()
    {
        ui.SetTimeLabel(musicsPlayer.GetTotalTime(), musicsPlayer.GetCurrentTime());
        ui.SetProcessSlider(musicsPlayer.GetPostion());
    }

    private void PlaySong(Song newSong)
    {
        if (newSong == null) return;
        listManager.SetCurrentSong(newSong);
        musicsPlayer.ReadySong(newSong);
        musicsPlayer.Play();
        lyricController.InitLyrics(newSong.lrcPath);
        ui.SetProcessSliderMax(musicsPlayer.GetLength());
        ui.ChangePlayingText("暂停");
        ui.SetCurrentPlayingLabel(listManager.GetPlayingList(), newSong);
        GlobalEvents.EventChangeSongInfo?.Invoke(this, new SongInfo_EventArgs() { song = newSong });
    }

    void On_Playing_Pressed()
    {
        if (musicsPlayer.isPlaying)
        {
            musicsPlayer.Pause();
            ui.ChangePlayingText("播放");
        }
        else if(musicsPlayer.isStop)
        {
            Song newSong = listManager.GetCurrentSong();
            if(newSong != null)
            {
                PlaySong(newSong);
            }
        }
        else
        {
            musicsPlayer.Play();
            ui.ChangePlayingText("暂停");
        }
    }

    void On_Process_Value_Changed(float value)
    {
        if (ui.isProcessDrag && (!musicsPlayer.isStop))
        {
            musicsPlayer.SetPostion(value);
        }
    }

    void On_Process_Drag_Started()
    {
        //musicsPlayer.Pause();
        //musicsPlayer.Flush();
    }

    void On_Process_Drag_Ended(bool hasChanged)
    {
        if(musicsPlayer.isPlaying)
            musicsPlayer.Play();
    }

    void On_Volume_Value_Changed(float value)
    {
        musicsPlayer.SetVolume(value);
    }

    void On_Add_List_Ok_Button_Pressed()
    {
        string newList = ui.GetAddListInput();
        if ((!listManager.HasList(newList)) && newList != "")
        {
            listManager.AddList(newList);
            ui.SetListInfo(newList);
        }
    }

    void OnListChangeEvent(object o, EventArgs e)
    {
        string newList = (e as ListName_EventArgs).listName;
        if (newList != listManager.GetCurrentList())
        {
            ui.SetScorllVerticalZero();
        }
        listManager.SetCurrentList(newList);
        ui.ClearSongInfo();
        ui.ChangeCurrentList(newList);
        ui.SetSongInfo(listManager.GetCurrentListSongs());
        if (newList == listManager.GetPlayingList())
        {
            ui.SetScorllVertical(listManager.GetCurrentSong());
        }
        GlobalEvents.EventChangeSongInfo?.Invoke(this, new SongInfo_EventArgs() { song = listManager.GetCurrentSong() });
    }

    void OnSelectSongPlay(object o, EventArgs e)
    {
        string songName = (e as SongName_EventArgs).songName;
        Song newSong = listManager.GetCurrentListSongWithName(songName);
        listManager.SetPlayingList(listManager.GetCurrentList());
        PlaySong(newSong);
        particlesManager.CreateNewClickParticles();
    }

    void OnPlayingStatePressed()
    {
        if (currentPlayerState == PlayerState.Keep)
        {
            ui.SetPlayingStateButtonText("[循环列表]");
            currentPlayerState = PlayerState.LoopList;
        }
        else if (currentPlayerState == PlayerState.LoopList)
        {
            ui.SetPlayingStateButtonText("[单曲循环]");
            currentPlayerState = PlayerState.LoopOne;
        }
        else if (currentPlayerState == PlayerState.LoopOne)
        {
            ui.SetPlayingStateButtonText("[随机播放]");
            currentPlayerState = PlayerState.Rand;
        }
        else if (currentPlayerState == PlayerState.Rand)
        {
            ui.SetPlayingStateButtonText("[顺序播放]");
            currentPlayerState = PlayerState.Keep;
        }
    }
    
    void OnPlayNextSong()
    {
        Song nextSong;
        if (currentPlayerState == PlayerState.Rand)
        {
            nextSong = listManager.GetPlayingListSongRand();
        }
        else
        {
            nextSong = listManager.GetCurrentNextSong();
        }
        PlaySong(nextSong);
    }
    
    void OnPlayPreSong()
    {
        Song preSong;
        if (currentPlayerState == PlayerState.Rand)
        {
            preSong = listManager.GetPlayingListSongRand();
        }
        else
        {
            preSong = listManager.GetCurrentPreSong();
        }
        PlaySong(preSong);
    }

    void OnAddSongDialogSelected(string[] files)
    {
        for (int i = 0; i < files.Length; ++i)
        {
            if (listManager.HasSong(listManager.GetCurrentList(), files[i])) continue;

            Song newSong = new Song(files[i], listManager.GetCurrentList());
            if (newSong.time.TotalSeconds == 0) continue;

            listManager.AddSong(listManager.GetCurrentList(), newSong);
            ui.SetSongInfo(newSong);
        }
    }

    void OnDeleteListPressed(object o, EventArgs e)
    {
        string listName = (e as ListName_EventArgs).listName;
        listManager.RemoveList(listName);
        if (listName == listManager.GetCurrentList())
        {
            listManager.SetCurrentList("默认列表");
            ui.ClearSongInfo();
            ui.SetSongInfo(listManager.GetCurrentListSongs());
            GlobalEvents.EventChangeSongInfo?.Invoke(this, new SongInfo_EventArgs() { song = listManager.GetCurrentSong() });
        }
        if (listName == listManager.GetPlayingList())
        {
            musicsPlayer.Stop();
            ui.SetCurrentPlayingLabel("", null);
            ui.ChangePlayingText("播放");
            listManager.SetCurrentSong((Song)null);
            listManager.SetPlayingList(listManager.GetCurrentList());
        }
        ui.FreshListPanel(listManager.GetListNames());
        ui.ChangeCurrentList(listManager.GetCurrentList());
        dataManager.SaveList(listManager);
    }
    
    void OnDeleteSongPressed(object o, EventArgs e)
    {
        Song s = (e as SongInfo_EventArgs).song;
        if (s == listManager.GetCurrentSong())
        {
            if (listManager.IsLastSong(s))
            {
                listManager.RemoveCurrentListSong(s);
                listManager.SetCurrentSong((Song)null);
                musicsPlayer.Stop();
                ui.SetCurrentPlayingLabel("", null);
                ui.ChangePlayingText("播放");
            }
            else
            {
                Song nextSong = listManager.GetCurrentNextSong();
                listManager.RemoveCurrentListSong(s);
                listManager.SetCurrentSong(nextSong);
                PlaySong(listManager.GetCurrentSong());
            }
        }
        else
        {
            listManager.RemoveCurrentListSong(s);
        }
        
        ui.RemoveSongInstance(s);
    }
}
