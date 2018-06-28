using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void DeadEventHandler();

public class Movement : Character 
{
	private static Movement instance;
	private int lives = 0;
	public event DeadEventHandler Dead;

	[SerializeField]
	public Stat healthStat;
	public static Movement Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<Movement>();
			}
			return instance;
		}
	}

	[SerializeField]
	private Transform[] groundPoints;

	[SerializeField]
	private float groundRadius;

	[SerializeField]
	private LayerMask whatIsGround;

	[SerializeField]
	private bool airControl;

	[SerializeField]
	private float jumpForce;

	private bool immortal = false;

	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float immortalTime;

	public AudioSource gunShot;
	public AudioSource playerDie;
	public AudioSource swordFx;
	public AudioSource jumpFx;
	public AudioSource lightning;
	public AudioSource spinFx;
	public AudioSource thud;
	public AudioSource runFx;
	public AudioSource gruntFx;
	public AudioSource yippee;
	public AudioSource hooray;
	public Rigidbody2D MyRigidbody { get; set; }

	public bool OnGround { get; set; }
	
	public bool Jump { get; set; }

	public bool Stab { get; set; }

	public bool Slide { get; set; }

	public override bool IsDead
	{
		get
		{	
			if (healthStat.CurrentVal <= 0)
			{
				
				OnDead();
			}
			
			return healthStat.CurrentVal <= 0;
		}
	}

	private Vector2 startPos;

	public override void Start () 
	{
		base.Start();

		startPos = transform.position;
		spriteRenderer = GetComponent<SpriteRenderer>();
		MyRigidbody = GetComponent<Rigidbody2D>();
		healthStat.Initialize();
		
	}
	
	void Update()
	{
		if (!TakingDamage && !IsDead)
		{
			if (transform.position.y <= -40f) /*respawn if failling */
			{
				Death();
			}
			
		}
		HandleInput();
	}

	void FixedUpdate () 
	{
		OnGround = IsGrounded();
		float horizontal = Input.GetAxis("Horizontal");
		HandleMovement(horizontal);
		Flip(horizontal);
	}

	public void OnDead()
	{
		if (Dead != null)
		{
			Dead();
		}
	}

	private void HandleMovement(float horizontal)
	{
		if (MyRigidbody.velocity.y < 0)
		{
			MyAnimator.SetBool("land", true);
		}
		if (!Attack && !Slide && (OnGround || airControl))
		{
			MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
		}
		if (Jump && MyRigidbody.velocity.y == 0)
		{
			MyRigidbody.AddForce(new Vector2(0, jumpForce));
		}
		MyAnimator.SetFloat("speed",Mathf.Abs(horizontal));
	}
			

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MyAnimator.SetTrigger("jump");
			jumpFx.Play();
			runFx.Stop();
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			MyAnimator.SetTrigger("attack");
			if(OnGround){
				gunShot.Play();
				runFx.Stop();
			}
			if(!OnGround){
				swordFx.Play();
				jumpFx.Play();
				runFx.Stop();
			}
		}
		if (Input.GetKeyDown(KeyCode.Semicolon))
		{
			MyAnimator.SetTrigger("slide");
			spinFx.Play();
			runFx.Stop();
		}
		if (Input.GetKeyDown(KeyCode.Quote))
		{
			MyAnimator.SetTrigger("stab");
			lightning.Play();
			runFx.Stop();
		}
		if (Input.GetKeyDown(KeyCode.A) && OnGround){
			runFx.Play();
			if (!Input.GetKeyDown(KeyCode.A)){
				runFx.Stop();
			}
		}
		if (Input.GetKeyDown(KeyCode.D) && OnGround){
			runFx.Play();
			if (!Input.GetKeyDown(KeyCode.D)){
				runFx.Stop();
			}
		}
		
	}

		private void Flip(float horizontal)
	{
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
		{
			ChangeDirection();
		}
	}

		private bool IsGrounded()
		{
			if (MyRigidbody.velocity.y <= 0)
			{
				foreach (Transform point in groundPoints)
				{
					Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position,groundRadius,whatIsGround);

					for (int i = 0; i < colliders.Length; i++)
					{
						if (colliders[i].gameObject != gameObject)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		private void HandleLayers()
		{
			if (!OnGround)
			{
				MyAnimator.SetLayerWeight(1,1);
			}
			else
			{
				MyAnimator.SetLayerWeight(1,0);
			}
		}

		public override void ShootGun(int value)
		{
			if (OnGround && value == 0)
			{
				base.ShootGun(value);	
			}
		}

		private IEnumerator IndicateImmortal()
		{
			while (immortal)
			{
				spriteRenderer.enabled = false;
				yield return new WaitForSeconds(.1f);
				spriteRenderer.enabled = true;
				yield return new WaitForSeconds(.1f);
			}
		}
		public override IEnumerator TakeDamage()
		{
			if (!immortal)
			{
				healthStat.CurrentVal -= 10;
				gruntFx.Play();

				if (!IsDead)
				{
					MyAnimator.SetTrigger("damage");
					immortal = true;
					StartCoroutine(IndicateImmortal());

					yield return new WaitForSeconds(immortalTime);

					immortal = false;
				}
				else
				{
					playerDie.Play();
					MyAnimator.SetLayerWeight(1,0);
					MyAnimator.SetTrigger("die");
				}
			}
		}
		public override void Death()
		{
			lives += 1;
			if(lives == 1){
				MyAnimator.SetTrigger("idle");
				healthStat.CurrentVal = healthStat.MaxVal;
				transform.position = startPos;
				GameObject.FindWithTag("live1").SetActive(false);
			}
			if(lives == 2){
				MyAnimator.SetTrigger("idle");
				healthStat.CurrentVal = healthStat.MaxVal;
				transform.position = startPos;
				GameObject.FindWithTag("live2").SetActive(false);
			}
			if(lives == 3){
				MyAnimator.SetTrigger("idle");
				healthStat.CurrentVal = healthStat.MaxVal;
				transform.position = startPos;
				GameObject.FindWithTag("live3").SetActive(false);
				SceneManager.LoadScene("StitchBoy");
			}
		}
}