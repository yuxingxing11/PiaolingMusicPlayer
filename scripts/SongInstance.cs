using System;
using Godot;


public partial class SongInstance : VBoxContainer
{
    [Export] Label fileNameLabel;
    [Export] Label timeLabel;
    [Export] Label formatLabel;
    [Export] PackedScene MouseRightPanel;
    
    Song song;
    bool isSelected;

    LabelSettings labelSettings;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsPressed())
            {
                if (mouseButton.ButtonMask == MouseButtonMask.Left)
                {
                    GlobalEvents.EventPlaySong?.Invoke(this, new SongName_EventArgs() { songName = fileNameLabel.Text });
                }
                else if (mouseButton.ButtonMask == MouseButtonMask.Right)
                {
                    MouseRightPanel mr = MouseRightPanel.Instantiate<MouseRightPanel>();
                    GetTree().CurrentScene.AddChild(mr);
                    mr.SetMouseRightPanel(null, song, PanelType.SongType);
                    mr.GlobalPosition = GetGlobalMousePosition() + new Vector2(-10, -10);
                }
            }
        }
    }
    
    public Song GetSelfSong()
    {
        return song;
    }

    public void SetSelectColor(Color color)
    {
        labelSettings.FontColor = color;
    }

    public void SetSongInfo(Song s)
    {
        song = s;
        fileNameLabel.Text = s.name;
        formatLabel.Text = s.format;
        timeLabel.Text = s.FormatTime();
        labelSettings = new LabelSettings();
        labelSettings.FontSize = 20;
        fileNameLabel.LabelSettings = labelSettings;
        timeLabel.LabelSettings = labelSettings;
        formatLabel.LabelSettings = labelSettings;
        GlobalEvents.EventChangeSongInfo += OnSelected;
    }

    public void ClearEvents()
    {
        GlobalEvents.EventChangeSongInfo -= OnSelected;
    }

    void OnSelected(object o, EventArgs e)
    {
        Song s = (e as SongInfo_EventArgs).song;
        if (s == song)
        {
            isSelected = true;
            labelSettings.FontColor = Colors.LightBlue;
        }
        else
        {
            labelSettings.FontColor = Colors.White;
            isSelected = false;
        }
    }

    void On_Mouse_Entered()
    {
        labelSettings.FontColor = Colors.WebGray;
    }

    void On_Mouse_Exited()
    {
        if (isSelected)
            labelSettings.FontColor = Colors.LightBlue;
        else
            labelSettings.FontColor = Colors.White;
    }
}
