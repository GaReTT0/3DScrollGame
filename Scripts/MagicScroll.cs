using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのスキルを解放するMagicScrollクラス
/// </summary>
public class MagicScroll : Item
{
	//使用した際の効果
	protected override void Use()
	{
		//UnlockSkill
		for (int i = 0; i < GameManager.instance.GetIsSkill().Length; i++)
		{
			if (!GameManager.instance.GetIsSkill()[i])
			{
				GameManager.instance.SetIsSkill(i, true);
				break;
			}
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
