using System;
using Godot;
using NAudio.Wave;
using TagLib;

public class Song
{
    public string lrcPath;
    public string name;
    public string actress;
    public string fileName;
    public string[] artists;
    public string album;
    public string listFrom;
    public TimeSpan time;
    public string format;

    public Song(string filePath, string listName)
    {
        try
        {
            fileName = filePath;
            lrcPath = fileName.Left(fileName.LastIndexOf('.')) + ".lrc";
            using (TagLib.File file = TagLib.File.Create(filePath))
            {
                name = file.Tag.Title ?? ParseFileName();
                artists = file.Tag.Performers;
                if(artists == null)
                {
                    artists = new string[1];
                    artists[1] = "未知歌手";
                    GD.Print(artists);
                }
                album = file.Tag.Album ?? "未知专辑";
            }
            AudioFileReader fileReader = new AudioFileReader(fileName);
            time = fileReader.TotalTime;
            fileReader.Dispose();
            listFrom = listName;
            format = ParseFormat();
        }
        catch
        {
            fileName = filePath;
            time = new TimeSpan(0);
            format = ParseFormat();
        }
        
    }

    public string FormatTime()
    {
        string t = "";
        if (time.Minutes >= 10)
        {
            t = time.Minutes.ToString();
        }
        else
        {
            t = "0" + time.Minutes.ToString();
        }

        t += ":";

        if (time.Seconds >= 10)
        {
            t += time.Seconds.ToString();
        }
        else
        {
            t += "0" + time.Seconds.ToString();
        }

        return t;
    }

    public string GetArtists()
    {
        string r = null;
        for (int i = 0; i < artists.Length; ++i)
        {
            r += artists[i];
            if (i + 1 < artists.Length)
            {
                r += ", ";
            }
        }
        
        return r;
    }

    private string ParseFileName()
    {
        int index = fileName.LastIndexOf('/');
        if (index < 0) return fileName;

        string n = fileName.Right(index + 1);
        int pointIndex = n.LastIndexOf('.');
        return n.Left(pointIndex);
    }

    private string ParseFormat()
    {
        int i = fileName.LastIndexOf(".");
        return fileName.Right(i + 1);
    }
}