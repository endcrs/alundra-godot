using Godot;
using System;
public class Player : KinematicBody2D
{
    // Constantes de Movimentação
    private const int ACCELERATION = 600;
    private int MAX_SPEED = 130;
    private const int FRICTION = 2000;

    // Eixo Z
    public float z = 0f;
    public int zfloor = 0;
    private float zspeed = -3.2f;
    private float zgrav = 0.3f;
    private bool zjump = false;

    // Variavel Global
    private Global global;

    // Variaveis
    private Vector2 motion = new Vector2(0, 0);
    private Vector2 Axis = new Vector2(0, 0);
    private float scaleShadow = 1f;
    private int altMax = 50;

    // Nós
    private AnimatedSprite PlayerSprite;
    private Area2D PlayerDetect;
    private Sprite ShadowSprite;
    private AnimationTree AnimTree;
    private AnimationNodeStateMachinePlayback AnimState;

    private Block block;

    public override void _Ready()
    {
        // Global
        global = GetNode<Global>("/root/Global");

        // Player
        PlayerSprite = GetNode<AnimatedSprite>("PlayerSprite");
        PlayerSprite.Playing = true;
        PlayerDetect = GetNode<Area2D>("PlayerDetect");
        ShadowSprite = GetNode<Sprite>("ShadowSprite");

        // AnimTree
        AnimTree = GetNode<AnimationTree>("AnimationTree");
        AnimState = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/playback");
        AnimTree.Active = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        global.z = z;

        _CheckFloor();
        _Jump();
        _Movement(delta);
        _Move();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Pulo simples
        if (@event.IsActionPressed("ui_jump"))
        {
            if (z <= zfloor)
            {
                zjump = true;
            }
        }
    }

    public void _Jump()
    {
        // Adiciona o impulso do Pulo
        if (zjump == true)
        {
            z += zspeed;
        }

        // Adiciona Gravidade
        if (z != zfloor)
        {
            z += zgrav;
            zgrav += 0.32f;
        }

        // Verifica quando o Player finaliza o pulo
        if (z + zspeed >= zfloor)
        {
            z = zfloor;
            zgrav = 0;
            zjump = false;
        }

        // Posição do Sprite do jogador 
        PlayerSprite.Position = new Vector2((GlobalPosition.x - Position.x), (GlobalPosition.y - Position.y) + z);
    }

    public void _Movement(float delta)
    {
        // Verifica se o Player está em movimento
        if (_GetInputAxis() != new Vector2(0, 0)) 
        {
            AnimTree.Set("parameters/Idle/blend_position", _GetInputAxis());
            AnimTree.Set("parameters/Walk/blend_position", _GetInputAxis());
            AnimTree.Set("parameters/Jump/blend_position", _GetInputAxis());
            AnimTree.Set("parameters/Fall/blend_position", _GetInputAxis());
            AnimState.Travel("Walk");
            motion = motion.MoveToward(_GetInputAxis() * MAX_SPEED, ACCELERATION * delta);
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

        _Shadow();
    }

    public void _CheckFloor()
    {
        // Verificação de altura
        if (PlayerDetect.GetOverlappingAreas().Count == 0)
        {
            zfloor = 0;
        }
        else
        {
            block = (Block)PlayerDetect.GetOverlappingAreas()[0];
            zfloor = block.zBlock;
        }
    }

    public void _Shadow()
    {
        // Aplica um efeito de distancia com a sombra quando o player pula
        ShadowSprite.Scale = new Vector2(scaleShadow, scaleShadow);
        scaleShadow = 1 - (Math.Abs(PlayerSprite.Position.y - zfloor)/altMax);
        ShadowSprite.Modulate =  Color.ColorN("White", scaleShadow);
        ShadowSprite.Position = new Vector2(0, 0 + zfloor);
    }

    public Vector2 _GetInputAxis()
    {
        // Retorna o Valor em vetor baseado na direção da seta do teclado
        Axis.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        Axis.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        Axis = Axis.Normalized();
        return Axis;
    }

    public void _Move()
    {
        // Aplica o movimento do Player 
        motion = MoveAndSlide(motion);
    }
}
