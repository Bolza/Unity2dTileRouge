using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {
	public GameManager gameManager;
	public int pixelPerUnit = 32;
	private int boardSize;
	// Use this for initialization
	void Start () {
		if (GameManager.instance == null) {
			Instantiate (gameManager);
		}
		boardSize = GameManager.instance.boardScript.rows + 2;
		float hUnits = boardSize * 0.5f;
		Camera.main.orthographicSize = hUnits;

		float viewHeight = hUnits - 1.5f;
		float viewWidth = (boardSize / 2f) - 0.5f;
		Camera.main.transform.position = new Vector3(viewWidth , viewHeight, -1);
	}
}
