using Godot;
using System;
using System.Collections.Generic;

public partial class UiController : Control
{
    [Export] private Button playingButton;
    [Export] private Label timeLabel;
    [Export] private HSlider processSlider;
    [Export] private Button PlayingStateButton;
    [Export] private HSlider volumeSlider;

    [Export] private Panel MidPanel;
    [Export] private Panel pagePanel;
    [Export] private Panel listPanel;
    [Export] private Panel settingPanel;
    [Export] private VBoxContainer listContainer;
    [Export] private VBoxContainer songContainer;
    [Export] private Window addListSubWindow;
    [Export] private LineEdit addListLineEdit;
    [Export] private Label currentPlayingLabel;
    [Export] private ScrollContainer songsScroll;
    [Export] private PackedScene listScene;
    [Export] private PackedScene songScene;
    [Export] private FileDialog addSongFileDialog;
    

    private List<ListInstance> listInstances = new List<ListInstance>();
    private List<SongInstance> songInstances = new List<SongInstance>();

    public bool isProcessDrag;
    private Panel currentMidState;

    public override void _Process(double delta)
    {
        FixMidPanelMove();   
    }
    
    private void FixMidPanelMove()
    {
        pagePanel.Position = new Vector2(-listPanel.Size.X, 0);
        listPanel.Position = new Vector2(0, 0);
        settingPanel.Position = new Vector2(listPanel.Size.X, 0);
        if ((Math.Abs(MidPanel.Size.X) % Math.Abs(MidPanel.Position.X)) != 0 && (Math.Abs(MidPanel.Size.Y) % Math.Abs(MidPanel.Position.Y)) != 0)
        {
            if (MidPanel.Position != Vector2.Zero)
            {
                if (currentMidState == pagePanel)
                {
                    MidPanel.Position = new Vector2(MidPanel.Size.X, 0);
                }
                else if (currentMidState == listPanel)
                {
                    MidPanel.Position = new Vector2(0, 0);
                }
                else if (currentMidState == settingPanel)
                {
                    MidPanel.Position = new Vector2(-MidPanel.Size.X, 0);
                }
            }
        }
    }

    public void RemoveSongInstance(Song song)
    {
        for (int i = 0; i < songInstances.Count; ++i)
        {
            if (songInstances[i].GetSelfSong() == song)
            {
                songInstances[i].QueueFree();
                songInstances.RemoveAt(i);
                return;
            }
        }
    }

    public void FreshListPanel(string[] lists)
    {
        ClearListInfo();
        SetListInfo(lists);
    }

    public void ClearListInfo()
    {
        for (int i = 0; i < listInstances.Count; ++i)
        {
            listInstances[i].QueueFree();
        }
        listInstances.Clear();
    }

    public void ClearSongInfo()
    {
        for (int i = 0; i < songInstances.Count; ++i)
        {
            songInstances[i].QueueFree();
            songInstances[i].ClearEvents();
        }
        songInstances.Clear();
    }

    public void SetScorllVertical(Song s)
    {
        for (int i = 0; i < songInstances.Count; ++i)
        {
            if (songInstances[i].GetSelfSong() == s)
            {
                int positionY = ((int)songInstances[i].Size.Y + 9) * i - (int)songInstances[i].Size.Y - 9;
                Tween tween = CreateTween();
                tween.TweenProperty(songsScroll, "scroll_vertical", positionY, 0.3);
            }
        }
    }
    
    public void SetScorllVerticalZero()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(songsScroll, "scroll_vertical", 0, 0.01);
    }

    public void SetVolumeSlider(float v)
    {
        volumeSlider.Value = v;
    }

    public void SetPlayingStateButtonText(string s)
    {
        PlayingStateButton.Text = s;
    }

    public void SetCurrentPlayingLabel(string playingList ,Song song)
    {
        if (song == null)
        {
            currentPlayingLabel.Text = "...";
            return;
        }
        
        currentPlayingLabel.Text = "<" + playingList + ">  " + song.name;
    }

    public void SetSongInfo(Song newSong)
    {
        SongInstance songUI = songScene.Instantiate<SongInstance>();
        songContainer.AddChild(songUI);
        songUI.SetSongInfo(newSong);
        songInstances.Add(songUI);
    }

    public void SetSongInfo(Song[] songs)
    {
        for (int i = 0; i < songs.Length; ++i)
        {
            SetSongInfo(songs[i]);
        }
    }

    public void VisibleAddSongDialog(bool isShow)
    {
        addSongFileDialog.Visible = isShow;
    }

    public void SetListInfo(string listName)
    {
        ListInstance list = listScene.Instantiate<ListInstance>();
        listContainer.AddChild(list);
        list.Text = listName;
        list.HorizontalAlignment = HorizontalAlignment.Center;
        list.VerticalAlignment = VerticalAlignment.Center;
        listInstances.Add(list);
        list.listName = listName;
    }

    public void SetListInfo(string[] listNames)
    {
        for (int i = 0; i < listNames.Length; ++i)
        {
            SetListInfo(listNames[i]);
        }
    }

    public void SetTimeLabel(TimeSpan totalTime, TimeSpan currentTime)
    {
        timeLabel.Text = FormatTime(currentTime) + "/" + FormatTime(totalTime);
    }

    private string FormatTime(TimeSpan time)
    {
        string m = time.Minutes < 10 ? "0" + time.Minutes : time.Minutes.ToString();
        string s = time.Seconds < 10 ? "0" + time.Seconds : time.Seconds.ToString();
        return m + ":" + s;
    }

    public void SetProcessSlider(double value)
    {
        if (isProcessDrag) return;
        processSlider.Value = value;
    }

    public void SetProcessSliderMax(double value)
    {
        processSlider.MaxValue = value;
    }

    public double GetProcessSliderValue()
    {
        return processSlider.Value;
    }

    public string GetAddListInput()
    {
        return addListLineEdit.Text;
    }

    public void ChangePlayingText(string str)
    {
        playingButton.Text = str;
    }
    public int GetScrollVertical()
    {
        return songsScroll.ScrollVertical;
    }

    void OnListChangeEvent(object o, EventArgs e)
    {
        string listName = (e as ListName_EventArgs).listName;
        foreach (ListInstance i in listInstances)
        {
            if (i.Text == listName)
            {
                i.isCurrentList = true;

            }
        }
    }

    public void ChangeCurrentList(string newList)
    {
        foreach (ListInstance i in listInstances)
        {
            if (i.Text == newList)
            {
                i.SetCurrentList();
            }
            else
            {
                i.CancelCurrentList();
            }
        }
    }

    void On_Process_Drag_Started()
    {
        isProcessDrag = true;
    }

    void On_Process_Drag_Ended(bool hasChanged)
    {
        isProcessDrag = false;
    }

    void On_Page_Pressed()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(MidPanel, "position", new Vector2(MidPanel.Size.X, 0), 0.2);
        currentMidState = pagePanel;
    }

    void On_List_Pressed()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(MidPanel, "position", new Vector2(0, 0), 0.2);
        currentMidState = listPanel;
    }

    void On_Setting_Pressed()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(MidPanel, "position", new Vector2(-MidPanel.Size.X, 0), 0.2);
        currentMidState = settingPanel;
    }

    void On_Add_List_Button_Pressed()
    {
        addListLineEdit.Text = "";
        addListSubWindow.Visible = true;
    }

    void On_Add_List_Cancel_Button_Pressed()
    {
        addListSubWindow.Visible = false;
    }

    void On_Add_Song_Button_Pressed()
    {
        addSongFileDialog.Visible = true;
    }
    
}
