using Godot;
using System;

public partial class ListInstance : Label
{

    [Export] PackedScene MouseRightPanel;
    public bool isCurrentList;
    public string listName;
    LabelSettings labelSettings;
    bool isMouseEnter;
    Color currentListColor = Colors.SkyBlue;
    
    public override void _Ready()
    {
        labelSettings = new LabelSettings();
        labelSettings.FontSize = 20;
        labelSettings.FontColor = Colors.White;
        this.LabelSettings = labelSettings;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (isMouseEnter)
        {
            if (@event is InputEventMouseButton inputEventMouse)
            {
                if (inputEventMouse.IsPressed() )
                {
                    if(inputEventMouse.ButtonMask == MouseButtonMask.Left)
                    {
                        GlobalEvents.EventChangeList?.Invoke(this, new ListName_EventArgs() { listName = Text });
                    }
                    else if(inputEventMouse.ButtonMask == MouseButtonMask.Right)
                    {
                        if (listName == "默认列表" || listName == "我喜欢") return;

                        MouseRightPanel mr = MouseRightPanel.Instantiate<MouseRightPanel>();
                        GetTree().CurrentScene.AddChild(mr);
                        mr.SetMouseRightPanel(listName, null, PanelType.ListType);
                        mr.GlobalPosition = GetGlobalMousePosition() + new Vector2(-10, -10);
                    }
                }
            }
        }
    }

    public void SetCurrentList()
    {
        isCurrentList = true;
        this.LabelSettings.FontColor = currentListColor;
    }
    
    public void CancelCurrentList()
    {
        isCurrentList = false;
        this.LabelSettings.FontColor = Colors.White;
    }

    void On_Mouse_Entered()
    {
        this.LabelSettings.FontColor = Colors.DimGray;
        isMouseEnter = true;
    }

    void On_Mouse_Exited()
    {
        if (!isCurrentList)
            this.LabelSettings.FontColor = Colors.White;
        else
            this.LabelSettings.FontColor = currentListColor;
            isMouseEnter = false;
    }
}
