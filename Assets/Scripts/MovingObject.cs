using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {
	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2d;
	private float inverseMoveTime;
	// Use this for initialization
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2d = GetComponent<Rigidbody2D> ();

		// computationally is more efficient to multiply than divide
		inverseMoveTime = 1f / moveTime;
	}

	protected IEnumerator SmoothMovement (Vector3 end) {
		// sqrMagnitude is more efficient than Magnitude to calculate the distance
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		//float Epsilon is a very small number, almost zero
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPos = Vector3.MoveTowards (rb2d.position, end, inverseMoveTime * Time.deltaTime);
			// XXX any better way to move the object?
			//rb2d.MovePosition (newPos);
			transform.position = newPos;
			// remaining distance
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			// wait a frame before continuing the loop
			yield return null;

		}
	}

	protected abstract void OnCantMove <T> (T Component) 
		where T : Component;

	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		// when we cast a ray we dont want our own collider in the middle
		boxCollider.enabled = false;
		// checking for blockers on the way
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir)
		where T : Component {
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit); 
		if (hit.transform == null)
			return;
			
		T hitComponent = hit.transform.GetComponent<T> ();
		if (!canMove && hitComponent) {
			OnCantMove (hitComponent);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
