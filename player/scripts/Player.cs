using Godot;
using System;
public class Player : KinematicBody2D
{
    // Constantes de Movimentação
    private const int ACCELERATION = 600;
    private const int MAX_SPEED = 130;
    private const int FRICTION = 1200;

    // Falso Eixo Z
    public float z = 0f;
    public int zfloor = 0;
    private float zspeed = -2.5f;
    private float zgrav = 0.25f;
    private bool zjump = false;

    // Variaveis
    private Vector2 motion = new Vector2(0, 0);
    public Vector2 Axis = new Vector2(0, 0);
    private float scaleShadow = 1f;
    private int altMax = 50;


    // Nodes
    public AnimatedSprite PlayerSprite;
    private Sprite ShadowSprite;
    private AnimationTree AnimTree;
    private AnimationNodeStateMachinePlayback AnimState; 

    public override void _Ready()
    {
        PlayerSprite = GetNode<AnimatedSprite>("PlayerSprite");
        ShadowSprite = GetNode<Sprite>("ShadowSprite");
        AnimTree = GetNode<AnimationTree>("AnimationTree");
        AnimState = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/playback");
        AnimTree.Active = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        //GD.Print("Posição do Sprite: " + PlayerSprite.Position.y);
        Jump();
        Movement(delta);
        Move();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);


        if (@event.IsActionPressed("ui_jump"))
        {
            if (z <= zfloor)
            {
                zjump = true;
            }
        }
    }

    public void Jump()
    {
        if (zjump == true)
        {
            z += zspeed;
        }
        if (z != zfloor)
        {
            z += zgrav;
            zgrav += 0.2f;
        }

        if (z + zspeed >= zfloor)
        {
            z = zfloor;
            zgrav = 0;
            PlayerSprite.Position = new Vector2(0, zfloor);
            zjump = false;
        }

        // Posição do Sprite do jogador 
        PlayerSprite.Position = new Vector2((GlobalPosition.x - Position.x), (GlobalPosition.y - Position.y) + z);

        GD.Print("SpritePos: " + PlayerSprite.Position + " PlayerPos" + Position);
    }

    public void Movement(float delta)
    {
        // Verifica se o Player está em movimento
        if (GetInputAxis() != new Vector2(0, 0)) 
        {
            AnimTree.Set("parameters/Idle/blend_position", GetInputAxis());
            AnimTree.Set("parameters/Walk/blend_position", GetInputAxis());
            AnimTree.Set("parameters/Jump/blend_position", GetInputAxis());
            AnimTree.Set("parameters/Fall/blend_position", GetInputAxis());
            AnimState.Travel("Walk");
            motion = motion.MoveToward(GetInputAxis() * MAX_SPEED, ACCELERATION * delta);
        }
        // Verifica se o Player está parado 
        else 
        {
            AnimState.Travel("Idle");
            motion = motion.MoveToward(new Vector2(0, 0), FRICTION * delta);
        }

        // Verifica se o player está pulando
        if (zjump == true)
        {
            AnimState.Travel("Jump");
        }

        Shadow();
    }

    public void Shadow()
    {
        ShadowSprite.Scale = new Vector2(scaleShadow, scaleShadow);
        scaleShadow = 1 - (Math.Abs(PlayerSprite.Position.y - zfloor)/altMax);
        ShadowSprite.Modulate =  Color.ColorN("White", scaleShadow);
        ShadowSprite.Position = new Vector2(0, 0 + zfloor);
    }

    public Vector2 GetInputAxis()
    {
        Axis.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        Axis.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        Axis = Axis.Normalized();
        return Axis;
    }

    public void Move()
    {
        motion = MoveAndSlide(motion);
    }
}
