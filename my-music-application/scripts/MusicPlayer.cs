using System;
using NAudio.Wave;
public class MusicsPlayer
{
    WaveOutEvent waveOut;
    AudioFileReader fileReader;
    public bool isStop;
    public bool isPlaying;
    float volume;

    public MusicsPlayer()
    {
        waveOut = new WaveOutEvent();
        fileReader = null;
        volume = 1f;
        isStop = true;
        isPlaying = false;
        waveOut.Volume = volume;
    }

    public void ReadySong(Song song)
    {
        fileReader?.Dispose();
        waveOut?.Dispose();
        fileReader = new AudioFileReader(song.fileName);
        waveOut.Init(fileReader);
    }

    public bool IsStop()
    {
        if (waveOut.PlaybackState == PlaybackState.Stopped)
        {
            return true;
        }
        return false;
    }

    public bool IsPlayOver()
    {
        if (waveOut.PlaybackState == PlaybackState.Stopped && isPlaying)
        {
            isPlaying = false;
            isStop = true;
            return true;
        }
        return false;
    }

    public void Play()
    {
        waveOut.Play();
        isStop = false;
        isPlaying = true;
    }

    public void Pause()
    {
        if (isStop) return;

        waveOut.Pause();
        isPlaying = !isPlaying;
    }

    public void Stop()
    {
        waveOut.Stop();
        waveOut.Dispose();
        fileReader.Seek(0,System.IO.SeekOrigin.Begin);
        fileReader.Dispose();
        fileReader = null;
        isPlaying = false;
        isStop = true;
    }

    public void SetVolume(float _v)
    {
        waveOut.Volume = _v;
        volume = _v;
    }

    public float GetVolume()
    {
        return waveOut.Volume;
    }

    public void SetPostion(float amount)
    {
        fileReader.Position = (long)amount;
    }

    public void SkipNext(int _v)
    {
        fileReader.Skip(_v);
    }

    public void SkipPre(int _v)
    {
        fileReader.Skip(-_v);
    }

    public double GetPostion()
    {
        if (fileReader == null) return 0;
        return fileReader.Position;
    }

    public void Flush()
    {
        fileReader.Flush();
    }

    public double GetLength()
    {
        return fileReader.Length;
    }

    public long GetPostionTest()
    {
        return fileReader.Position;
    }

    public TimeSpan GetCurrentTime()
    {
        if (fileReader == null) return TimeSpan.Zero;

        return fileReader.CurrentTime;
    }

    public TimeSpan GetCurrentTimeNoMillisecond()
    {
        if (fileReader == null) return TimeSpan.Zero;
        return new TimeSpan(fileReader.CurrentTime.Hours, fileReader.CurrentTime.Minutes, fileReader.CurrentTime.Seconds);
    }

    public TimeSpan GetTotalTime()
    {
        if (fileReader == null) return TimeSpan.Zero;

        return fileReader.TotalTime;
    }
}