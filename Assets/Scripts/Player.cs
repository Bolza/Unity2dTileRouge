using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound1;

	private bool facingRight = true;
	private Animator animator;
	private int food;
	private GameManager gameManager;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();
		gameManager = Camera.main.GetComponent<Loader> ().gameManager;
		food = gameManager.playerFoodPoints;
		foodText.text = "Food: " + food;
//		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
		base.Start ();
	}

	private void OnDisable() {
		gameManager.playerFoodPoints = food;
	}

	// XXX more efficient here or in the actual trigger?
	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "Food") {
			food += pointsPerFood;
			other.gameObject.SetActive (false);
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
		}
		else if (other.tag == "Soda") {
			food += pointsPerSoda;
			other.gameObject.SetActive (false);
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
		}
		foodText.text = "Food: " + food;

	}

	private void CheckIfGameOver() {
		if (food <= 0) {
			SoundManager.instance.PlaySingle (gameOverSound1);
			SoundManager.instance.musicSource.Stop ();
			GameManager.instance.GameOver ();
		}
	}
	protected override void AttemptMove <T> (int xDir, int yDir) {
		base.AttemptMove <T> (xDir, yDir);
		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit)) {
			food--;
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}
		foodText.text = "Food: " + food;
		CheckIfGameOver ();
		GameManager.instance.playersTurn = false;
	}

	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal > 0)
			facingRight = true;
		else if (horizontal < 0) {
			facingRight = false;
		}
		Vector3 scale = transform.localScale;
		if (facingRight != scale.x > 0) {
			scale.x *= -1;
			transform.localScale = scale;
		}

		// Stop diagonal movement
		if (horizontal != 0) {
			vertical = 0;
		}

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);
		}
	}

	protected override void OnCantMove <T> (T component) {
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerAttack");
	}

	private void Restart() {	
		SceneManager.LoadScene (0);
	}

	public void LoseFood(int loss) {
		animator.SetTrigger("playerHit");
		food -= loss;
		foodText.text = "Food: " + food;
		CheckIfGameOver ();
	}
}
