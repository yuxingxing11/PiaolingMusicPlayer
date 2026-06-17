using Godot;
using System;

public partial class ClickParticles2d : CpuParticles2D
{
    void OnClickParticles2dFinished()
    {
        QueueFree();
    }
}
