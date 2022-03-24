using Godot;
using System;
public class Player : KinematicBody2D
{
    private Vector2 velocity = new Vector2(0, 0);
    private Vector2 input_vector = new Vector2(0, 0); 

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        input_vector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        input_vector.x = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

        if (input_vector != new Vector2(0, 0)) {
            velocity = input_vector;
        } else {
            velocity = new Vector2(0, 0);
        }

        MoveAndCollide(velocity);
    }

}
