using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {
	public int playerDamage;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	public AudioClip attackSound1;
	public AudioClip attackSound2;

	// Use this for initialization
	protected override void Start () {
		GameManager.instance.AddEnemy (this);
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		if (skipMove) {
			skipMove = false;
			return;
		} 
	
		base.AttemptMove <T> (xDir, yDir);
		skipMove = true;
	}

	public void MoveEnemy() {
		int xDir = 0;
		int yDir = 0;

		// isnt easier to moveToward ?
		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			yDir = target.position.y > transform.position.y ? 1 : -1;
		} 
		else {
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}
		AttemptMove<Player> (xDir, yDir);
	}

	protected override void OnCantMove <T> (T component) {
		Player hitPlayer = component as Player;
		animator.SetTrigger ("enemyAttack");
		SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		hitPlayer.LoseFood(playerDamage);
	}
}
