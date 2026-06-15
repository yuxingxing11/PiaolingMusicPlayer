using System;
public class GlobalEvents
{
    static public EventHandler EventChangeList;
    static public EventHandler EventPlaySong;
    static public EventHandler EventChangeSongInfo;
    static public EventHandler EventDeleteSong;
    static public EventHandler EventDeleteList;
}

public class SongInfo_EventArgs : EventArgs
{
    public Song song;
}
public class ListName_EventArgs : EventArgs
{
    public string listName;
}

public class SongName_EventArgs : EventArgs
{
    public string songName;
}