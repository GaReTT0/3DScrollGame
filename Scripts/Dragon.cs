using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴール地点であるDragonクラス
/// </summary>
public class Dragon : MonoBehaviour
{
	//アニメーター
	[SerializeField] private Animator animator;

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }

	//コライダーを持つ物体が範囲内に入った時に呼び出される
	private void OnTriggerEnter(Collider other)
	{
		switch (other.gameObject.tag)
		{
			case "Player":
				GameManager.instance.GameJudge(true);
				break;

			default:
				break;
		}
		Debug.Log("ok");
	}
}
