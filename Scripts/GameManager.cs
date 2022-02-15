using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// SingletonのGameManagerクラス
/// </summary>
public class GameManager : MonoBehaviour
{

	//GameManagerのインスタンス
	public static GameManager instance;

	//スコア
	private float score;
	//スコアを表示するテキスト
	[SerializeField] private Text scoreText;
	//ゲーム開始時に表示される画面
	[SerializeField] private GameObject startCanvas;
	//ゲームオーバー時に表示される画面
	[SerializeField] private GameObject gameOverCanvas;

	//Characterクラスで用いる変数
	//体力
	[SerializeField] private float[] hp;
	//最大HP
	private float maxHp;
	//移動スピード
	[SerializeField] private float[] moveSpeed;
	//ジャンプスピード
	[SerializeField] private float[] jumpSpeed;
	//重力
	[SerializeField] private float gravity = 20.0f;
	//ジャンプ中の横移動を制御する力
	[SerializeField] private float jumpControlPower = 0.2f;
	//ジャンプスキルが解放された時に加算されるジャンプ力
	[SerializeField] private float plusJumpPower = 7.0f;
	//ジャンプキー
	[SerializeField] private KeyCode jumpKey;
	//攻撃キー
	[SerializeField] private KeyCode[] attackKey;
	//プレイヤーの攻撃力
	[SerializeField] private float attackPower;
	//敵の攻撃力
	[SerializeField] private float enemyAttackPower;
	//プレイヤーから炎のパーティクルが表示される距離
	[SerializeField] private float fireParticleDistance = 10.0f;
	//炎のパーティクル
	[SerializeField] private GameObject[] fireParticleObject;
	//HpBar
	[SerializeField] private Slider hpBar;
	//プレイヤーの現在位置
	private Vector3 playerLocation;

	//Itemクラスで使用される変数

	//ポーションの回復量
	[SerializeField] private float healPoint;
	//解放されているスキル
	[SerializeField] private bool[] isSkills;


	//Music関連
	//SEの配列
	[SerializeField] private AudioClip[] se;

	//AudioSourceを取得
	[SerializeField] private AudioSource SeSource;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		maxHp = hp[0];
		StartCoroutine("DisplayStartCanvas");
		DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	private void Update() { }

	//ゲームクリアかオーバーを判定する
	public void GameJudge(bool clear)
	{
		if (clear)
		{
			Debug.Log("game clear!!");
			SceneManager.LoadScene("GameClearScene");
		}
		else
		{
			Debug.Log("game over");
			gameOverCanvas.SetActive(true);
			StartCoroutine("TitleScene");
		}
	}

	//タイトルシーンに遷移
	private IEnumerator TitleScene()
	{
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene("TitleScene");
	}

	//スコアを計算
	public void CalcScore(float incrementScore)
	{
		this.score += incrementScore;
		scoreText.text = score.ToString();
	}

	//スタート画面
	private IEnumerator DisplayStartCanvas()
	{
		yield return new WaitForSeconds(3f);
		startCanvas.SetActive(false);
	}

	//SEを再生する
	public void PlaySe(int seNum)
	{
		switch (seNum)
		{
			case 1:
				SeSource.PlayOneShot(se[0]);
				break;
			case 2:
				SeSource.PlayOneShot(se[1]);
				break;
			case 3:
				SeSource.PlayOneShot(se[2]);
				break;
			case 4:
				SeSource.PlayOneShot(se[3]);
				break;
		}
	}

	//各変数のゲッター
	public float GetScore() { return score; }
	public float GetHp(int num) { return hp[num]; }
	public float GetMaxHp() { return maxHp; }
	public Slider GetHpBar() { return hpBar; }
	public Vector3 GetPlayerLocation() { return playerLocation; }
	public float GetMoveSpeed(int num) { return moveSpeed[num]; }
	public float GetGravity() { return gravity; }
	public float GetJumpSpeed(int num) { return jumpSpeed[num]; }
	public float GetJumpControlPower() { return jumpControlPower; }
	public float GetPlusJumpPower() { return plusJumpPower; }
	public KeyCode GetJumpKey() { return jumpKey; }
	public KeyCode GetAttackKey(int num) { return attackKey[num]; }
	public float GetAttackPower() { return attackPower; }
	public float GetEnemyAttackPower() { return enemyAttackPower; }
	public GameObject GetFireParticle(int num) { return fireParticleObject[num]; }
	public float GetFireParticleDistance() { return fireParticleDistance; }
	public float GetHealPoint() { return healPoint; }
	public bool[] GetIsSkill() { return isSkills; }


	//キャラクターの番号と格納したい数値を引数に持つ
	public void SetHp(int num, float newHp) { hp[num] = newHp; }

	//スキルの番号と取得しているかどうかのフラグを引数に持つ
	public void SetIsSkill(int num, bool frag) { isSkills[num] = frag; }

	//プレイヤーの現在位置を引数に持つ
	public void SetPlayerLocation(Vector3 location) { playerLocation = location; }

}
