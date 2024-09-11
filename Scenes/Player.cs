using Godot;
using System;
using System.Security.Cryptography;
using static Godot.TextServer;

public partial class Player : CharacterBody2D
{

    [Export] private AnimatedSprite2D animatedSprite;
    Vector2 direction;

    #region WalkStuff
    [ExportCategory("WalkStuff")]
    [Export]public float speed = 1f;
    [Export] private float maxSpeed;
    [Export] public float Friction = 1f;
	#endregion


	[ExportCategory("JumpStuff")]
	[Export] private float timeToPeak = 0.4f;
	[Export] private float timeToDecend = 0.4f;
    [Export] private float JumpHeight = 100;

    public float jumpVelocity;
	private float ascendgravity;
	private float fallgravity;



    #region Dash Stuff
    [ExportCategory("DashStuff")]
    private float DashVelocity;

    [Export] private float dashDistance = 200;
    [Export] private float dashTime = 0.87f;
    [Export] private float dashcooldown = 1.5f;
   
    private bool isDashing;
    private bool canDash = true;
    private Timer dashTimer;
    private Timer dashCooldownTimer;

    #endregion

    private float previousX;

    public override void _Ready()
    {
        DashVelocity = dashDistance / dashTime;
		// gravity = 2 * height * time^2
        ascendgravity = (2 * JumpHeight) / (timeToPeak * timeToPeak);
        fallgravity = (2 * JumpHeight) / (timeToDecend * timeToDecend);

        jumpVelocity = (-2 * JumpHeight) / timeToPeak;
        InitDashEvents();
		
    }

    private void InitDashEvents()
    {
        CreateTimer(ref dashCooldownTimer, dashcooldown);
        dashCooldownTimer.Timeout += () =>
        {
            canDash = true;
            dashCooldownTimer.Stop();
        };
        CreateTimer(ref dashTimer, dashTime);
        dashTimer.Timeout += () => {
            isDashing = false;
            canDash = false;
            dashCooldownTimer.Start();
            dashTimer.Stop();
        };
        animatedSprite.AnimationFinished += () =>
        {
            animatedSprite.Play("default");
        };
    }

	private void CreateTimer(ref Timer timer, float waitTime)
	{
        timer = new Timer();
        timer.WaitTime = waitTime;
        AddChild(timer);
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
        
        


		direction = Input.GetVector("a", "d", "w", "s");
		if (direction != Vector2.Zero && !isDashing)
		{
            animatedSprite.Play("Walk");
			velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction.X,speed * (float)delta);
			velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
            PlayerFlip();
            previousX = direction.X;
		}
		else if (!isDashing) 
		{
            animatedSprite.Play("Idle");

            velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction);
		}
       
        Gravity(ref velocity, (float)delta);

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = jumpVelocity;
        }

        if (Input.IsActionJustPressed("Shift") && canDash)
		{
            if (previousX == 0) {
                previousX = 1;
            }
            velocity.X = DashVelocity * previousX;
			isDashing = true;
			
        }
		if (Input.IsActionJustPressed("Shift") && canDash)
		{
			dashTimer.Start();
		}


        Velocity = velocity;
		MoveAndSlide();

		if (Input.IsActionPressed("Deflect")) {
			animatedSprite.Play("Deflect");
		}
        if (Input.IsActionPressed("Slap"))
        {
            animatedSprite.Play("Slap");
        }
    }

	private void Gravity(ref Vector2 velocity, float delta)
	{
        if (!IsOnFloor() && !isDashing)
        {
            if (velocity.Y < 0)
            {
                velocity.Y += ascendgravity * (float)delta;
            }
            else
            {
                velocity.Y += fallgravity * (float)delta;

            }
        }
    }

    private void PlayerFlip()
    {
        animatedSprite.FlipH = direction.X < 0 ? true : false;

    }

    
}
