using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// パーティクルにアタッチするParticleManagerクラス
/// </summary>
public class ParticleManager : MonoBehaviour
{
	//パーティクル再生後、オブジェクトを破壊したいパーティクル
	[SerializeField] private ParticleSystem particle;
	void Start() { }

	//パーティクル再生後、オブジェクトを破壊
	void Update()
	{
		if (particle.isStopped) //パーティクルが終了したか判別
		{
			Destroy(this.gameObject);//パーティクル用ゲームオブジェクトを削除
		}
	}
}
