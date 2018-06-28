using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
	
	private Movement player;

	[SerializeField]
	protected Transform bulletPos;

	[SerializeField]
	protected float movementSpeed;

	protected bool facingRight;

	[SerializeField]
	private GameObject bulletPrefab;

	[SerializeField]
	protected int health;

	[SerializeField]
	private EdgeCollider2D lightningCollider;

	[SerializeField]
	private EdgeCollider2D spinCollider;

	[SerializeField]
	private EdgeCollider2D swordCollider;

	[SerializeField]
	public List<string> damageSources;

	public abstract bool IsDead { get; }

	public bool Attack { get; set; }

	public bool TakingDamage { get; set; }

	public Animator MyAnimator { get; private set; }

	public EdgeCollider2D SwordCollider
	{
		get
		{
			return swordCollider;
		}
	}
	public EdgeCollider2D LightningCollider
	{
		get
		{
			return lightningCollider;
		}
	}
	public EdgeCollider2D SpinCollider
	{
		get
		{
			return spinCollider;
		}
	}

	public virtual void Start () 
	{
		facingRight = true;
		MyAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public abstract IEnumerator TakeDamage();

	public abstract void Death();

	public void ChangeDirection()
	{
		facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;

			transform.localScale = theScale;
	}

	public virtual void ShootGun(int value)
	{
		if (facingRight)
				{
					GameObject tmp = (GameObject)Instantiate(bulletPrefab, bulletPos.position, Quaternion.Euler(new Vector3(0,0,-90)));
					tmp.GetComponent<BulletScript>().Initialize(Vector2.right);
				}
				else
				{
					GameObject tmp = (GameObject)Instantiate(bulletPrefab, bulletPos.position, Quaternion.Euler(new Vector3(0,0,+90)));
					tmp.GetComponent<BulletScript>().Initialize(Vector2.left);
				}
		
	}
	public virtual void AcidSpit(int value)
	{
		if (facingRight)
				{
					GameObject tmp = (GameObject)Instantiate(bulletPrefab, bulletPos.position, Quaternion.Euler(new Vector3(0,0,0)));
					tmp.GetComponent<AcidScript>().Initialize(Vector2.left);
				}
				else
				{
					GameObject tmp = (GameObject)Instantiate(bulletPrefab, bulletPos.position, Quaternion.Euler(new Vector3(0,0,180)));
					tmp.GetComponent<AcidScript>().Initialize(Vector2.right);
				}
		
	}
	public void MeleeAttack()
	{
		LightningCollider.enabled = true;
	}
	public void SpinAttack()
	{
		SpinCollider.enabled = true;
	}
	public void SwordAttack()
	{
		SwordCollider.enabled = true;
	}

	public virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (damageSources.Contains(other.tag))
		{
			StartCoroutine(TakeDamage());
		}
		if (other.tag == "Collectable"){
				Debug.Log("health ");
				other.gameObject.SetActive (false);
				this.transform.GetComponent<Movement>().healthStat.CurrentVal += 20;
				this.transform.GetComponent<Movement>().yippee.Play();
			}
		if (other.tag == "Keg"){
				Debug.Log("health ");
				other.gameObject.SetActive (false);
				this.transform.GetComponent<Movement>().healthStat.CurrentVal += 50;
				this.transform.GetComponent<Movement>().hooray.Play();
			}
	}

}
