using System;
using System.Collections.Generic;
using System.Linq;

public class ListManager
{
    Dictionary<string, List<Song>> Lists;
    Song currentSong;
    string currentList;
    string playingList;

    public ListManager()
    {
        Lists = new Dictionary<string, List<Song>>();
        Lists.Add("默认列表", new List<Song>());
        Lists.Add("我喜欢", new List<Song>());
        currentList = "默认列表";
        playingList = currentList;
        currentSong = null;
    }

    public void SetPlayingList(string listName)
    {
        playingList = listName;
    }

    public Song GetCurrentNextSong()
    {
        for (int i = 0; i < Lists[playingList].Count; ++i)
        {
            if (currentSong == Lists[playingList][i])
            {
                if (i == Lists[playingList].Count - 1)
                {
                    return Lists[playingList][0];
                }
                else
                {
                    return Lists[playingList][i+1];
                }
            }
        }
        return null;
    }

    public Song GetCurrentPreSong()
    {
        for (int i = 0; i < Lists[playingList].Count; ++i)
        {
            if (currentSong == Lists[playingList][i])
            {
                if (i == 0)
                {
                    return Lists[playingList][Lists[playingList].Count - 1];
                }
                else if(i > 0)
                {
                    return Lists[playingList][i-1];
                }
            }
        }
        return null;
    }

    public string GetPlayingList()
    {
        return playingList;
    }

    public Song[] GetPlayingListSongs()
    {
        return Lists[playingList].ToArray();
    }

    public Song[] GetCurrentListSongs()
    {
        return Lists[currentList].ToArray();
    }

    public string[] GetListNames()
    {
        return Lists.Keys.ToArray();
    }

    public Song GetCurrentSong()
    {
        return currentSong;
    }

    public Song GetCurrentListSongWithName(string name)
    {
        foreach (Song song in Lists[currentList])
        {
            if (song.name == name)
            {
                return song;
            }
        }
        return null;
    }

    public Song GetPlayingListSongWithPath(string path)
    {
        foreach (Song song in Lists[playingList])
        {
            if (song.fileName == path)
            {
                return song;
            }
        }
        return null;
    }

    public Song GetPlayingListSongRand()
    {
        if (Lists[playingList] == null) return null;
        if (Lists[playingList].Count == 1) return Lists[playingList][0];

        Random random = new Random();
        while(true)
        {
            int i = random.Next(0, Lists[playingList].Count);
            if(Lists[playingList][i] != currentSong)
                return Lists[playingList][i];
        }
    }

    public Song[] GetSongsWithList(string listName)
    {
        return Lists[listName].ToArray();
    }

    public string[] GetSongsPathWithList(string listName)
    {
        string[] songs = new string[Lists[listName].Count];
        for (int i = 0; i < songs.Length; ++i)
        {
            songs[i] = Lists[listName][i].fileName;
        }
        return songs;
    }

    public string GetCurrentList()
    {
        return currentList;
    }

    public void SetCurrentSong(Song newSong)
    {
        currentSong = newSong;
    }

    public void SetCurrentSong(string fileName)
    {
        for (int i = 0; i < Lists[currentList].Count; ++i)
        {
            if (fileName == Lists[currentList][i].fileName)
            {
                SetCurrentSong(Lists[currentList][i]);
                return;
            }
        }
    }

    public void SetCurrentList(string newList)
    {
        currentList = newList;
    }

    public void AddSong(string listName, string filePath)
    {
        Song song = new Song(filePath, listName);
        Lists[listName].Add(song);
    }

    public void AddSong(string listName, Song song)
    {
        Lists[listName].Add(song);
    }

    public void AddSongs(string listName, string[] filePaths)
    {
        for (int i = 0; i < filePaths.Length; ++i)
        {
            if (!HasSong(listName, filePaths[i]))
            {
                AddSong(listName, filePaths[i]);
            }
        }
    }

    public void RemoveList(string listName)
    {
        if (listName == "默认列表" || listName == "我喜欢") return;

        Lists[listName].Clear();
        Lists.Remove(listName);
    }
    
    public void RemoveCurrentListSong(Song _song)
    {
        Lists[currentList]?.Remove(_song);
    }

    public bool HasSong(string listName, string filePath)
    {
        foreach (Song song in Lists[listName])
        {
            if (song.fileName == filePath)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsLastSong(Song _song)
    {
        if (Lists[playingList] == null) return false;

        if (_song == Lists[playingList][Lists[playingList].Count - 1])
        {
            return true;
        }
        return false;
    }

    public void AddList(string newList)
    {
        Lists.Add(newList, new List<Song>());
    }

    public bool HasList(string newList)
    {
        foreach (string i in Lists.Keys)
        {
            if (newList == i)
            {
                return true;
            }
        }
        return false;
    }
}