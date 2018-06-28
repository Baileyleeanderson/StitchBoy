using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Enemy : Character 
{

	private IEnemyState currentState;

	public GameObject Target { get; set; }

	[SerializeField]
	private float meleeRange;

	[SerializeField]
	private float shootRange;

	[SerializeField]
	private Transform leftEdge;

	[SerializeField]
	private Transform rightEdge;

	private bool immortal = false;

	// private bool toggleSound = false;
	public AudioSource laser;
	public AudioSource damageFx;
	public AudioSource deathFx;
	public AudioSource explodeFx;
	public AudioSource meleeFx;
	public AudioSource meleeWhoosh;

	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float immortalTime;

	public Rigidbody2D MyRigidbody { get; set; }

	public bool InMeleeRange
	{
		get{
			if (Target != null)
			{
				return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
			}
			else
			{
				return false;
			}
		}
	}
	
	public bool InShootRange
	{
		get{
			if (Target != null)
			{
				return Vector2.Distance(transform.position, Target.transform.position) <= shootRange;
			}
			else
			{
				return false;
			}
		}
	}

	public override bool IsDead
	{
		get
		{
			return health <= 0;
		}
	}

	// Use this for initialization
	public override void Start () 
	{
		
		base.Start();
		Movement.Instance.Dead += new DeadEventHandler(RemoveTarget); /*Movement is called Player in the tutorial, but my script is called movement */
		spriteRenderer = GetComponent<SpriteRenderer>();
		MyRigidbody = GetComponent<Rigidbody2D>();
		ChangeState(new IdleState());

	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if (!IsDead)
		{
			if (!TakingDamage)
			{
				currentState.Execute();	
			}
			LookAtTarget();
		}
	
	}

	private void LookAtTarget()
	{
		if (Target != null)
		{
			float xDir = Target.transform.position.x - transform.position.x;

			if (xDir < 0 && !facingRight ||xDir > 0 && facingRight)
			{
				ChangeDirection();
			}
		}
	}

	public void RemoveTarget()
	{
		Target = null;
		ChangeState(new PatrolState());
	}

	public void ChangeState(IEnemyState newState)
	{	
		if (currentState != null)
		{
			currentState.Exit();
		}
		currentState = newState;

		currentState.Enter(this);	
	
	}

	public void Move()
	{

		if (!Attack)
		{
			if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x) )
			{
			MyAnimator.SetFloat("speed", 1);

			transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
			}
		}
		else if (currentState is PatrolState)
		{
			ChangeDirection();
		}
	}

	public Vector2 GetDirection()
	{
		return facingRight ? Vector2.left : Vector2.right;
	}

	public override void OnTriggerEnter2D(Collider2D other)
	{
		base.OnTriggerEnter2D(other);
		currentState.OnTriggerEnter(other); 
	
	}

	private IEnumerator IndicateImmortal()
		{
			while (immortal)
			{
				spriteRenderer.enabled = false;
				yield return new WaitForSeconds(.1f);
				spriteRenderer.enabled = true;
				yield return new WaitForSeconds(1.8f);

			}
		}
	
	public override IEnumerator TakeDamage()
		{
			if(immortal){

			}
			
			else if (!immortal)
			{
				health -= 10;
				
				if (!IsDead)
				{
					MyAnimator.SetTrigger("damage");
					damageFx.Play();
					immortal = true;
					StartCoroutine(IndicateImmortal());
					
					yield return new WaitForSeconds(immortalTime);

					immortal = false;
				}
				else
				{
					immortal = true;
					StartCoroutine(IndicateImmortal());
					MyAnimator.SetTrigger("die");
					deathFx.Play();
					explodeFx.Play();
					yield return new WaitForSeconds(5f);
				}
			}
		}
	public override void Death()
	{	
		MyAnimator.SetTrigger("idle");
		MyAnimator.ResetTrigger("die");
		Destroy(gameObject);
		if(this.tag == "boss"){
			new WaitForSeconds(5f);
			SceneManager.LoadScene("StitchBoy");
		}
	}
	
}