using Godot;
using System;

public class Block : Area2D
{
    public int zBlock = -16;

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

    }

    public void _on_Block_body_entered(Player kb)
    {
        kb.zfloor = zBlock;
        //kb.PlayerSprite.Position = new Vector2(kb.PlayerSprite.Position.x, kb.zfloor.y);    
    }

    public void _on_Block_body_exited(Player kb)
    {
        kb.zfloor = 0;
    }
}
