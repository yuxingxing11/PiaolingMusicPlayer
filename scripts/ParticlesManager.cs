using Godot;
using System;

public partial class ParticlesManager : Node2D
{
    [Export] PackedScene ClickParticles2DScene;
    [Export] CpuParticles2D mouseParticles;

    public override void _Ready()
    {
        mouseParticles.Emitting = true;
    }

    public override void _Process(double delta)
    {
        mouseParticles.GlobalPosition = GetGlobalMousePosition();
    }


    public void CreateNewClickParticles()
    {
        CpuParticles2D p = ClickParticles2DScene.Instantiate<CpuParticles2D>();
        AddChild(p);
        p.GlobalPosition = GetGlobalMousePosition();
        p.Emitting = true;
    }
}
