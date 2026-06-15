using System;
using Godot;
using NAudio.Wave;

public class Song
{
    public string name;
    public string actress;
    public string fileName;
    public string listFrom;
    public TimeSpan time;
    public string format;

    public Song(string filePath, string listName)
    {
        try
        {
            fileName = filePath;
            AudioFileReader fileReader = new AudioFileReader(fileName);
            name = ParseFileName();
            time = fileReader.TotalTime;
            fileReader.Dispose();
            listFrom = listName;
            format = ParseFormat();
        }
        catch
        {
            fileName = filePath;
            time = new TimeSpan(0);
            name = ParseFileName();
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