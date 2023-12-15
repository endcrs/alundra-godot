using Godot;
using System;

public class Block : Area2D
{
    [Export] public int zBlock;
    public CollisionShape2D CollisionWall;

    // Variavel Global
    private Global global;

    public override void _Ready()
    {
        // Global
        global = GetNode<Global>("/root/Global");

        CollisionWall = GetNode<CollisionShape2D>("Wall/CollisionWall");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        if (global.z <= zBlock + 4)
        {
            CollisionWall.Disabled = true;
        }
        else
        {
            CollisionWall.Disabled = false;
        }
        
    }
}
