using Godot;
using System;
public class Player : KinematicBody2D
{
    //Constantes
    private const int ACCELERATION = 500;
    private const int MAX_SPEED = 100;
    private const int FRICTION = 500;
    //Variáveis
    private Vector2 velocity = new Vector2(0, 0);
    private Vector2 input_vector = new Vector2(0, 0); 

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        input_vector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        input_vector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        input_vector = input_vector.Normalized();

        if (input_vector != new Vector2(0, 0)) {
            velocity = velocity.MoveToward(input_vector * MAX_SPEED, ACCELERATION * delta);
        } else {
            velocity = velocity.MoveToward(new Vector2(0, 0), FRICTION * delta);
        }

        velocity = MoveAndSlide(velocity);
    }

}
