using Godot;
using System;
using System.Security.Cryptography;

public partial class Player : CharacterBody2D
{
	[Export]public float speed = 1f; 
	[Export] public float Friction = 1f;
    [Export]public float jumpVelocity = -400.0f;
    [Export]private float maxSpeed;
    [Export] private float gravity;

    [Export]private AnimatedSprite2D animatedSprite;
    [Export] private float dash;
    private bool isDashing;
	private bool canDash = true;
	private Timer dashTimer;
	private Timer dashCooldownTimer;
    [Export] private float dashTime = 0.87f;
    [Export] private float dashcooldown = 1.5f;

    public override void _Ready()
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
		// Add the gravity.
		if (!IsOnFloor())
		{
            if (velocity.Y < 0)
            {
				velocity.Y += gravity * (float)delta;
            }
			else
			{
                velocity.Y += (gravity * 2) * (float)delta;

            }
        }

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = jumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("a", "d", "w", "s");
		if (direction != Vector2.Zero && !isDashing)
		{
			velocity.X = Mathf.MoveToward(velocity.X, maxSpeed * direction.X,speed * (float)delta);
			velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
			animatedSprite.FlipH = direction.X < 0 ? true : false;
		}
		else if (!isDashing) 
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction);
		}

		if (Input.IsActionJustPressed("Shift") && canDash || isDashing)
		{
            velocity.X += dash * (float)delta * (int)direction.X;
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
}
