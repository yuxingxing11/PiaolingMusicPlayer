using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Godot;

public static class LrcParser
{
    /// <summary>
    /// 解析lrc文件，返回 时间(TimeSpan) → 歌词 的字典
    /// </summary>
    /// <param name="filePath">lrc文件路径</param>
    /// <returns>Dictionary<TimeSpan, string></returns>
    public static Dictionary<TimeSpan, string> ParseLrc(string filePath)
    {
        Dictionary<TimeSpan, string> lrcDict = new Dictionary<TimeSpan, string>();

        if (!File.Exists(filePath))
        {
            GD.Print("not exist");
            return null;
        }
            

        // 正则匹配 [00:12.34] 时间标签
        Regex timeTagRegex = new Regex(@"\[(\d{2}):(\d{2})\.(\d{2})\]", RegexOptions.Compiled);

        // 逐行读取
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string trimLine = line.Trim();
            if (string.IsNullOrEmpty(trimLine))
                continue;

            // 匹配所有时间标签
            MatchCollection matches = timeTagRegex.Matches(trimLine);
            if (matches.Count == 0)
                continue;

            // 提取该行纯歌词（去掉所有[xx:xx.xx]）
            string lyricText = timeTagRegex.Replace(trimLine, "").Trim();

            // 遍历每个时间戳，存入字典
            foreach (Match match in matches)
            {
                int minute = int.Parse(match.Groups[1].Value);
                int second = int.Parse(match.Groups[2].Value);
                int centiSec = int.Parse(match.Groups[3].Value);

                // 构建TimeSpan：分钟 + 秒 + 百分秒转毫秒
                TimeSpan time = TimeSpan.FromMinutes(minute)
                                + TimeSpan.FromSeconds(second)
                                + TimeSpan.FromMilliseconds(centiSec * 10);

                // 相同时间戳覆盖，如需去重可自行判断ContainsKey
                lrcDict[time] = lyricText;
            }
        }

        return lrcDict;
    }
}