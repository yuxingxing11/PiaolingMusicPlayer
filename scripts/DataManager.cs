using System.IO;
using System.Collections.Generic;

public class DataManager
{

    public void SaveList(ListManager listManager)
    {
        SaveAllList(listManager);
    }

    private void SaveAllList(ListManager listManager)
    {
        string[] lists = listManager.GetListNames();
        using (StreamWriter sw = new StreamWriter("save/lists.dat"))
        {
            for (int i = 0; i < lists.Length; ++i)
            {
                sw.WriteLine(lists[i]);
            }
        }

        for (int i = 0; i < lists.Length; ++i)
        {
            string[] songPaths = listManager.GetSongsPathWithList(lists[i]);
            SaveListSongs(lists[i], songPaths);
        }
    }

    private void SaveListSongs(string listName, string[] songPaths)
    {
        using (StreamWriter sw = new StreamWriter("save/" + listName + ".list"))
        {
            for (int i = 0; i < songPaths.Length; ++i)
            {
                sw.WriteLine(songPaths[i]);
            }
        }
    }

    public void LoadList(ListManager listManager)
    {
        List<string> lists = GetStringsWithFile("save/lists.dat");
        foreach (string s in lists)
        {
            if (!listManager.HasList(s))
            {
                listManager.AddList(s);
            }
            List<string> songPath = GetStringsWithFile("save/"+s+".list");
            listManager.AddSongs(s, songPath.ToArray());
        }
    }

    private List<string> GetStringsWithFile(string fileName)
    {
        List<string> lists = new List<string>();
        try
        {
            string line;
            using (StreamReader sr = new StreamReader(fileName))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lists.Add(line);
                }
            }
        }
        catch
        {
            using (new StreamWriter(fileName)){}
        }
        
        return lists;
    }
}