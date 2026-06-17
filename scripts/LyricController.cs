using Godot;
using System;
using System.Collections.Generic;

public partial class LyricController : Control
{
    [Export] ScrollContainer lyricScrollContainer;
    [Export] Label defaultLrcLabel;
    [Export] PackedScene lyricLabelScene;

    Dictionary<TimeSpan, Label> lyrics = new Dictionary<TimeSpan, Label>();

    TimeSpan currentTime;
    TimeSpan lastTime;
    int defaultFontSize = 20;

    public void InitLyrics(string filePath)
    {
        lastTime = TimeSpan.Zero;
        currentTime = TimeSpan.Zero;
        defaultLrcLabel.Visible = false;
        FreeLrcLabel();
        lyrics.Clear();
        Dictionary<TimeSpan, string> t = LrcParser.ParseLrc(filePath);

        if (t == null)
        {
            defaultLrcLabel.Visible = true;
            return;
        }

        foreach (var item in t)
        {
            Label lyricLabel = lyricLabelScene.Instantiate<Label>();
            lyricLabel.LabelSettings = new LabelSettings();
            lyricLabel.LabelSettings.FontSize = defaultFontSize;
            lyricScrollContainer.GetChild(0).AddChild(lyricLabel);
            lyricLabel.Text = item.Value;
            lyrics.Add(item.Key, lyricLabel);
        }
    }

    public void ShowLyrics(TimeSpan _currentTime)
    {
        if (lyrics == null) return;
        if (_currentTime == currentTime) return;

        foreach (var item in lyrics)
        {
            if (item.Key == _currentTime)
            {
                lastTime = currentTime;
                currentTime = _currentTime;
                Tween tween = CreateTween();
                int index = lyrics[_currentTime].GetIndex();
                tween.TweenProperty(lyricScrollContainer, "scroll_vertical",
                index * (lyrics[_currentTime].Size.Y + 4) - lyricScrollContainer.Size.Y / 2+20, 0.1);
                lyrics[currentTime].LabelSettings.FontSize += 6;
                lyrics[currentTime].LabelSettings.FontColor = Colors.SkyBlue;
                if(lastTime.TotalSeconds > 0)
                {
                    lyrics[lastTime].LabelSettings.FontSize = defaultFontSize;
                    lyrics[lastTime].LabelSettings.FontColor = Colors.White;
                }
            }
        }
    }
    
    private void FreeLrcLabel()
    {
        if (lyrics == null) return;
        foreach(var i in lyrics)
        {
            i.Value.QueueFree();
        }
    }
}
