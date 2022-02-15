using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// クリックした時の処理を持つButtonManagerクラス
/// </summary>
public class ButtonManager : MonoBehaviour
{
	// Start is called before the first frame update
	void Start() { }
	// Update is called once per frame
	void Update() { }

	//アタッチしているボタンがクリックした時に呼び出される
	public void OnClick(int sceneNum)
	{
		switch (sceneNum)
		{
			case 0:
				SceneManager.LoadScene("TitleScene");
				break;
			case 1:
				SceneManager.LoadScene("GameScene");
				break;
			case 2:
				SceneManager.LoadScene("GameClearScene");
				break;
		}
	}
}
