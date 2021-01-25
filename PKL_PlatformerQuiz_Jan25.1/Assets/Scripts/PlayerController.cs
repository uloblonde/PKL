using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
	//start() variables
	private Rigidbody2D rb;
	private Animator anim;
	private Collider2D coll;

	//FSM
	private	enum State {idle, running, jumping, falling, hurt}
	private State state = State.idle;

	//inspector variables
	[SerializeField] private LayerMask ground;
	[SerializeField] private float speed = 8f;
	[SerializeField] private float jumpForce = 14f;
	[SerializeField] private float hurtForce = 10f;
	[SerializeField] private int cherries = 0;
	[SerializeField] private int health = 3;
	[SerializeField] private TextMeshProUGUI cherryText;
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private AudioSource cherry;
	[SerializeField] private AudioSource footstep;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		coll = GetComponent<Collider2D>();
		healthText.text = health.ToString();
	}

	private void Update()
	{
		if(state != State.hurt)
		{
			Movement();
		}

		AnimationState();
		anim.SetInteger("state", (int)state); //set animation based on Enumerator state
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Collectable")
		{
			cherry.Play();
			Destroy(collision.gameObject);
			cherries += 1;
			cherryText.text = cherries.ToString();
		}
		if(collision.tag == "PowerJump")
        {
			Destroy(collision.gameObject);
			jumpForce = 20f;
			GetComponent<SpriteRenderer>().color = Color.blue;
			StartCoroutine(ResetPowerJump());
        }
		if (collision.tag == "PowerSpeed")
		{
			Destroy(collision.gameObject);
			speed = 12f;
			GetComponent<SpriteRenderer>().color = Color.yellow;
			StartCoroutine(ResetPowerSpeed());
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			Enemy enemy = other.gameObject.GetComponent<Enemy>();
			if(state == State.falling)
			{
				enemy.JumpedOn();
				Jump();
			}
			else
            {
                state = State.hurt;
                Health();
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy is to my right therefore I should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //enemy is to my left therefore I should be damaged and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }

		if (other.gameObject.tag == "Boss")
		{
			Boss boss = other.gameObject.GetComponent<Boss>();
				state = State.hurt;
				Health();
				if (other.gameObject.transform.position.x > transform.position.x)
				{
					//boss is to my right therefore I should be damaged and move left
					rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
				}
				else
				{
					//boss is to my left therefore I should be damaged and move right
					rb.velocity = new Vector2(hurtForce, rb.velocity.y);
				}
			
		}

	}

    private void Health()
    {
        health -= 1;
        healthText.text = health.ToString();
        if(health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Movement()
	{
		float hDirection = CrossPlatformInputManager.GetAxis("Horizontal");	

		//gerak kiri
		if(hDirection < 0)
		{
			rb.velocity = new Vector2(-speed, rb.velocity.y);
			transform.localScale = new Vector2(-1, 1);
		}
		
		//gerak kanan
		else if(hDirection > 0)
		{
			rb.velocity = new Vector2(speed, rb.velocity.y);
			transform.localScale = new Vector2(1, 1);
		}

		//gerak lompat
		if(CrossPlatformInputManager.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
		{
			RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 1.3f, ground);
			if(hit.collider != null)
			{
			Jump();
			}
		}
	}

	private void Jump()
	{
		rb.velocity = new Vector2(rb.velocity.x, jumpForce);
		state = State.jumping;
	}

	private void AnimationState()
	{
		if(state == State.jumping)
		{
			if(rb.velocity.y < .1f)
			{
				state = State.falling;
			}
		}
		else if(state == State.falling)
		{
			if(coll.IsTouchingLayers(ground))
			{
				state = State.idle;
			}
		}
		else if(state == State.hurt)
		{
			if(Mathf.Abs(rb.velocity.x) < .1f)
			{
				state = State.idle;
			}
		}
		else if(Mathf.Abs(rb.velocity.x) > 2f)
		{
			//moving
			state = State.running;
		}
		else
		{
			state = State.idle;
		}
	}

	private void Footstep()
	{
		footstep.Play();
	}

	private IEnumerator ResetPowerJump()
    {
		yield return new WaitForSeconds(10);
		jumpForce = 14f;
		GetComponent<SpriteRenderer>().color = Color.white;
    }

	private IEnumerator ResetPowerSpeed()
	{
		yield return new WaitForSeconds(10);
		speed = 8f;
		GetComponent<SpriteRenderer>().color = Color.white;
	}
}