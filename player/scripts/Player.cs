using Godot;
using System;
public class Player : KinematicBody2D
{
    //Constantes
    private const int ACCELERATION = 600;
    private const int MAX_SPEED = 130;
    private const int FRICTION = 1000;
    private const int SPEED = 80;

    private const int GRAVITY = 25;
    private const int JUMP_FORCE = -200; 
    private Vector2 UP = new Vector2(0, -1);
    //Variáveis
    private Vector2 motion = new Vector2(0, 0);
    private Vector2 input_vector = new Vector2(0, 0);


    private int z = 0;
    private int velz = 0;
    private Vector2 AlundraMotion;

    //Nodes
    private KinematicBody2D Alundra;
    private AnimationTree AnimTree;
    private AnimationNodeStateMachinePlayback AnimState; 

    public override void _Ready()
    {
        Alundra = GetNode<KinematicBody2D>("Alundra");
        AnimTree = GetNode<AnimationTree>("AnimationTree");
        AnimState = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/playback");
        AnimTree.Active = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Jump();
        Movement(delta);
        Move();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Player Pula
        if (Alundra.Position.y == 0)
        {   
            if (@event.IsActionPressed("ui_jump"))
            {
                velz = JUMP_FORCE; 
            }
        }
    }

    public void Movement(float delta)
    {
        input_vector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        input_vector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        input_vector = input_vector.Normalized();

        //Verifica se o Player está em movimento
        if (input_vector != new Vector2(0, 0)) 
        {
            AnimTree.Set("parameters/Idle/blend_position", input_vector);
            AnimTree.Set("parameters/Walk/blend_position", input_vector);
            AnimState.Travel("Walk");
            motion = motion.MoveToward(input_vector * MAX_SPEED, ACCELERATION * delta);
        } 
        else 
        {
            AnimState.Travel("Idle");
            motion = motion.MoveToward(new Vector2(0, 0), FRICTION * delta);
        }
    }

    public void Jump()
    {
        z = velz;
        AlundraMotion = new Vector2(0, 0 + z);

        // Aplica gravidade caso o player esteja pulando  
        if (Alundra.Position.y < 0){
            velz += GRAVITY;
        }
        
        // Verifica se o player está no chão
        if (Alundra.Position.y > 0)
        {
            velz = 0;
            z = 0;
            AlundraMotion = new Vector2(0, 0 + z);  
            Alundra.Position = new Vector2(0,  0);
        }

    }

    public void Move()
    {
        motion = MoveAndSlide(motion, UP);
        AlundraMotion = Alundra.MoveAndSlide(AlundraMotion, UP);
    }
}
