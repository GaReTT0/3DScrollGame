using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのHPを回復させるポーションクラス
/// </summary>
public class Portion : Item
{
	//使用した際の効果
	protected override void Use()
	{
		if (GameManager.instance.GetHp(0) != GameManager.instance.GetMaxHp())
		{
			GameManager.instance.SetHp(0, GameManager.instance.GetHp(0) + GameManager.instance.GetHealPoint());
			GameManager.instance.GetHpBar().value = GameManager.instance.GetHp(0) / GameManager.instance.GetMaxHp();
			Debug.Log(GameManager.instance.GetHp(0));
		}
		else
		{
			Debug.Log("HP満タンだよ");
		}
		GameManager.instance.PlaySe(3);
		ItemDestroy();
	}

	//アイテムを破壊する
	protected override void ItemDestroy()
	{
		Destroy(this.gameObject);
	}

	//コライダーを持つオブジェクト同士が触れた時に呼び出される
	private void OnCollisionEnter(Collision other)
	{
		switch (other.gameObject.tag)
		{
			case "Player":
				this.Use();
				break;
			case "DropOut":
				this.ItemDestroy();
				break;
			case "Ground":

				break;
		}
	}

}
