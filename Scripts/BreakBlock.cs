using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージが壊れるギミックのBreakBlockクラス
/// </summary>
public class BreakBlock : StageGimmick
{
	//爆発のパーティクル
	[SerializeField] private GameObject breakParticleObject;

	//ステージが壊された時に呼び出される
	protected override void Gimmick()
	{
		//爆発するアニメーション
		Instantiate(breakParticleObject, this.transform.position + new Vector3(0, 0, 0), Quaternion.identity);
		GameManager.instance.PlaySe(2);
		Destroy(this.gameObject);
	}

	//パーティクルが当たった時に呼び出される
	private void OnParticleCollision(GameObject other)
	{
		Gimmick();
	}
}
