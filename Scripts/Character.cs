using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーとエネミークラスの親クラス
/// </summary>
public class Character : MonoBehaviour
{
	//キャラクターの移動速度
	protected float characterSpeed;

	// Start is called before the first frame update
	private void Start() { }

	// Update is called once per frame
	private void Update() { }

	protected virtual void Move() { }
	protected virtual void Attack() { }
	protected virtual void Damage() { }

}
