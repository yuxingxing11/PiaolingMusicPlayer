using Godot;

public enum PanelType{SongType, ListType}
public partial class MouseRightPanel : PanelContainer
{
    string list;
    Song song;
    PanelType panelType;

    public void SetMouseRightPanel(string _list, Song _song, PanelType _panelType)
    {
        list = _list;
        song = _song;
        panelType = _panelType;
    }

    void On_Delete_Button_Pressed()
    {
        if (panelType == PanelType.ListType)
        {
            GlobalEvents.EventDeleteList?.Invoke(this, new ListName_EventArgs() { listName = list });
        }
        else if (panelType == PanelType.SongType)
        {
            GlobalEvents.EventDeleteSong?.Invoke(this, new SongInfo_EventArgs() { song = this.song });
        }
        QueueFree();
    }
    
    void On_Delete_Button_Mouse_Exited()
    {
        QueueFree();
    }
}
