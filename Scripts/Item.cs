using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ポーションクラスとマジックスクロールクラスの親クラス
/// </summary>
public class Item : MonoBehaviour
{

	// Start is called before the first frame update
	private void Start() { }
	// Update is called once per frame
	private void Update() { }

	protected virtual void Use() { }
	protected virtual void ItemDestroy() { }
}
