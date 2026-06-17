using Godot;
using System;

public partial class ParticlesManager : Node2D
{
    [Export] PackedScene ClickParticles2DScene;
    [Export] CpuParticles2D mouseParticles;
    [Export] CpuParticles2D pageParticles;

    public override void _Ready()
    {
        mouseParticles.Emitting = true;
    }

    public override void _Process(double delta)
    {
        mouseParticles.GlobalPosition = GetGlobalMousePosition();
        ReSetSize();
    }

    public void ReSetSize()
    {
        if(DisplayServer.WindowGetSize().X != pageParticles.EmissionRectExtents.X)
        {
            pageParticles.EmissionRectExtents = new Vector2(DisplayServer.WindowGetSize().X, 10);
            pageParticles.Position = new Vector2(DisplayServer.WindowGetSize().X / 2, 0);
        }
    }


    public void CreateNewClickParticles()
    {
        CpuParticles2D p = ClickParticles2DScene.Instantiate<CpuParticles2D>();
        GetTree().CurrentScene.AddChild(p);
        p.GlobalPosition = GetGlobalMousePosition();
        p.Emitting = true;
    }
}
