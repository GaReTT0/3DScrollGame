using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラがプレイヤーを追従するCameraRotateクラス
/// </summary>
public class CameraRotate : MonoBehaviour
{
	//追従するプレイヤー
	[SerializeField] private GameObject player;
	//プレイヤーとカメラの距離
	[SerializeField] private Vector3 cameraDistance;
	// Start is called before the first frame update
	void Start()
	{
		cameraDistance = transform.position - player.transform.position;
	}
	// Update is called once per frame
	void LateUpdate()
	{
		transform.position = player.transform.position + cameraDistance;
	}
}
