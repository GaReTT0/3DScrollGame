using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

/// <summary>
/// プレイヤー関連の処理を行うプレイヤークラス
/// </summary>
public class Player : Character
{
	// プレイヤーの状態の型の列挙型
	public enum PLAYER_STATE
	{
		WAIT,
		WALK,
		JUMP,
		ATTACK,
		DAMAGE,
		DEAD
	}

	//プレイヤーの状態
	private PLAYER_STATE pState;
	//一つ前のプレイヤーの状態
	private PLAYER_STATE oldState;
	//CharacterControllerクラスを使用
	private CharacterController characterController;
	//X軸方向に移動した距離
	private float moveX;
	//移動した量を格納する
	private Vector3 moveDirection;
	//キャラクターの向き(trueは画面右向き)
	private int characterDirection = 1;
	//ジャンプしたベクトルと重力を格納する
	private Vector3 jumpDirection;
	//キャラクター全員におけるプレイヤーの番号
	private int playerNum = 0;
	//地面の当たり判定
	private bool isGround;
	//レイを飛ばす距離
	[SerializeField] private float distance = 0.1f;
	//レイが何かに当たった時の情報
	private RaycastHit hit;
	//キャラクターにかかる重力
	private float characterGravity;
	//加速度計算用の位置情報
	private Vector3 oldPosition;
	//現在の加速度
	private Vector3 velocity;
	//最大のHP
	private float maxHp;
	//現在のHP
	private float currentHp;
	//使った攻撃スキルの番号
	private int attackNum;
	//アニメーター
	[SerializeField] private Animator animator;
	//設定したフラグ
	private const string animation_isWalk = "Walk";
	private const string animation_isAttack = "Attack";
	private const string animation_isDamage = "Damage";
	private const string animation_isDead = "Dead";



	// Start is called before the first frame update
	private void Start()
	{
		characterSpeed = GameManager.instance.GetMoveSpeed(playerNum);
		characterController = GetComponent<CharacterController>();
		characterGravity = GameManager.instance.GetGravity();
		GameManager.instance.GetHpBar().value = 1;
		maxHp = GameManager.instance.GetHp(playerNum);
		currentHp = maxHp;
	}

	// Update is called once per frame
	void Update()
	{
		//接地判定
		GroundJudge();
		if (isGround)
		{
			//キャラクターが移動
			Move();
			//キャラクターの向きを変更
			RotationCharacter();
			//ジャンプキーが押された場合
			Jump();
			//攻撃キーが押された場合
			Attack();
		}
		else
		{
			JumpMove();
			//キャラクターの向きを変更
			RotationCharacter();
		}
		//キャラクターにかかる重力を計算
		CalcGravity();
		//移動量を反映
		CalcMoveDistance();
		//プレイヤーの現在位置を格納
		PlayerPresentLocation();
	}

	//接地判定
	private void GroundJudge()
	{
		Vector3 rayPosition = this.transform.position + new Vector3(0.0f, 0.1f, 0.0f);
		Ray ray = new Ray(rayPosition, Vector3.down);
		if (Physics.Raycast(ray, out hit, distance))
		{
			if (hit.collider.tag == "Ground")
			{
				isGround = true;
			}
			else
			{
				isGround = false;
			}
		}
		else
		{
			isGround = false;
		}
		Debug.DrawRay(rayPosition, Vector3.down * distance, Color.red);
	}

	//プレイヤーの移動を行うメソッド
	protected override void Move()
	{
		//方向キーを入力しているかどうか
		if (Input.GetAxis("Horizontal") != 0 && pState != PLAYER_STATE.DAMAGE)
		{
			pState = PLAYER_STATE.WALK;
			moveX = Input.GetAxis("Horizontal") * characterSpeed * Time.deltaTime;
			ChangeAnimation(pState);
		}
		else if (Input.GetAxis("Horizontal") == 0 && pState != PLAYER_STATE.DAMAGE)
		{
			pState = PLAYER_STATE.WAIT;
			moveX = 0;
			ChangeAnimation(pState);
		}
	}

	//対応したキーを押すとジャンプする
	private void Jump()
	{
		if (Input.GetKeyDown(GameManager.instance.GetJumpKey()))//  もし、スペースキーがおされたら、
		{
			oldState = pState;
			pState = PLAYER_STATE.JUMP;
			jumpDirection.y = GameManager.instance.GetJumpSpeed(playerNum);//  y座標をジャンプ力の分だけ動かす
			if (GameManager.instance.GetIsSkill()[0])
			{
				jumpDirection.y = GameManager.instance.GetJumpSpeed(playerNum) + GameManager.instance.GetPlusJumpPower();
			}
			moveX = Input.GetAxis("Horizontal") * characterSpeed * Time.deltaTime * GameManager.instance.GetJumpControlPower();
		}
	}

	//ジャンプ中の横移動
	private void JumpMove()
	{
		//方向キーを入力しているかどうか
		if (Input.GetAxis("Horizontal") != 0)
		{
			pState = PLAYER_STATE.WAIT;
			moveX = Input.GetAxis("Horizontal") * characterSpeed * Time.deltaTime * GameManager.instance.GetJumpControlPower();
			ChangeAnimation(PLAYER_STATE.WAIT);
		}
		else
		{
			pState = PLAYER_STATE.WAIT;
			moveX = 0;
			ChangeAnimation(PLAYER_STATE.WAIT);
		}
	}


	//重力を計算する
	private void CalcGravity()
	{
		//キャラクターにかける重力が過剰にjumpDirection.yに加算されることを防ぐ
		if (jumpDirection.y >= characterGravity)
		{
			jumpDirection.y += characterGravity * Time.deltaTime; //常にy座標を重力の分だけ動かす(重力処理)
		}
	}

	//キャラクターを回転させる
	private void RotationCharacter()
	{
		Quaternion rotation = gameObject.transform.rotation;
		if (rotation.y > 0 && moveX < 0 || rotation.y < 0 && moveX > 0)
		{
			characterDirection *= -1;
			rotation.y *= -1;
			gameObject.transform.rotation = rotation;
		}
	}

	//キャラクターの移動量を反映
	private void CalcMoveDistance()
	{
		//移動量を格納
		moveDirection = new Vector3(moveX, 0, 0);
		//CharacterControllerをmoveDirectionの方向に動かす
		characterController.Move(moveDirection);
		//CharacterControllerをjumpDirectionの方向に動かす
		characterController.Move(jumpDirection * Time.deltaTime);
	}


	//プレイヤーが攻撃を行う
	protected override void Attack()
	{
		if (Input.GetKeyDown(GameManager.instance.GetAttackKey(0)) && pState != PLAYER_STATE.ATTACK)
		{
			Debug.Log("attack1");
			pState = PLAYER_STATE.ATTACK;
			attackNum = 0;
			ChangeAnimation(pState);
			StartCoroutine("AttackAnimationTime");
		}
		else if (Input.GetKeyDown(GameManager.instance.GetAttackKey(1)) && GameManager.instance.GetIsSkill()[1] && pState != PLAYER_STATE.ATTACK)
		{
			Debug.Log("attack2");
			pState = PLAYER_STATE.ATTACK;
			attackNum = 1;
			ChangeAnimation(pState);
			StartCoroutine("AttackAnimationTime");
		}
		else if (Input.GetKeyDown(GameManager.instance.GetAttackKey(2)) && GameManager.instance.GetIsSkill()[2] && pState != PLAYER_STATE.ATTACK)
		{
			Debug.Log("attack3");
			pState = PLAYER_STATE.ATTACK;
			attackNum = 2;
			ChangeAnimation(pState);
			StartCoroutine("AttackAnimationTime");
		}
		else if (Input.GetKeyDown(GameManager.instance.GetAttackKey(3)) && GameManager.instance.GetIsSkill()[3] && pState != PLAYER_STATE.ATTACK)
		{
			Debug.Log("attack4");
			pState = PLAYER_STATE.ATTACK;
			attackNum = 3;
			ChangeAnimation(pState);
			StartCoroutine("AttackAnimationTime");
		}

	}

	//攻撃のアニメーションに合わせてパーティクルを表示する
	private IEnumerator AttackAnimationTime()
	{
		yield return new WaitForSeconds(0.8f);
		MakeFireParticle(attackNum);
		GameManager.instance.PlaySe(1);
		yield return new WaitForSeconds(0.2f);
		pState = PLAYER_STATE.WAIT;
		ChangeAnimation(pState);
	}

	//パーティクルを生成する
	private void MakeFireParticle(int particleNum)
	{
		Debug.Log("炎生成");
		if (characterDirection == 1)
		{
			Instantiate(GameManager.instance.GetFireParticle(particleNum), this.transform.position + new Vector3(GameManager.instance.GetFireParticleDistance(), 2, 0), Quaternion.identity);
		}
		else
		{
			Instantiate(GameManager.instance.GetFireParticle(particleNum), this.transform.position + new Vector3(-GameManager.instance.GetFireParticleDistance(), 2, 0), Quaternion.identity);
		}
	}


	//コライダーを持つ物体が範囲内に入った時に呼び出される
	private void OnTriggerEnter(Collider other)
	{
		switch (other.gameObject.tag)
		{
			case "DropOut":
				GameManager.instance.GameJudge(false);
				break;

			case "Gimmick":
				ChangeStage();
				break;

			default:
				break;
		}
	}

	//ドラゴンのいるステージにプレイヤーを移動させる
	private void ChangeStage()
	{
		Vector3 bossLocation = new Vector3(758, 241, 0);
		this.gameObject.transform.position = bossLocation;
	}

	//コライダーを持つオブジェクト同士が触れた時に呼び出される
	private void OnCollisionEnter(Collision other)
	{
		switch (other.gameObject.tag)
		{
			case "Enemy":
				Damage();
				break;

			case "Gimmick":

				break;

			default:
				break;
		}
	}

	//ダメージを与えられたときに呼び出される
	protected override void Damage()
	{
		pState = PLAYER_STATE.DAMAGE;
		ChangeAnimation(pState);
		GameManager.instance.CalcScore(-50);
		currentHp = GameManager.instance.GetHp(0);
		currentHp -= GameManager.instance.GetEnemyAttackPower();
		GameManager.instance.GetHpBar().value = currentHp / maxHp;
		GameManager.instance.SetHp(playerNum, currentHp);
		GameManager.instance.PlaySe(4);
		if (currentHp <= 0)
		{
			GameManager.instance.GameJudge(false);
		}
	}

	//Damageのアニメーションが終わった際に呼び出される
	private void FinishDamage()
	{
		oldState = pState;
		pState = PLAYER_STATE.WAIT;
		ChangeAnimation(pState);
		Debug.Log("ok");
	}

	//プレイヤーの状態によってアニメーションを切り替える
	private void ChangeAnimation(PLAYER_STATE state)
	{
		if (oldState != pState)
		{
			switch (pState)
			{
				case PLAYER_STATE.WAIT:
					this.animator.SetBool(animation_isWalk, false);
					this.animator.SetBool(animation_isAttack, false);
					this.animator.SetBool(animation_isDamage, false);
					this.animator.SetBool(animation_isDead, false);
					break;
				case PLAYER_STATE.WALK:
					this.animator.SetBool(animation_isWalk, true);
					this.animator.SetBool(animation_isAttack, false);
					this.animator.SetBool(animation_isDamage, false);
					this.animator.SetBool(animation_isDead, false);
					break;
				case PLAYER_STATE.ATTACK:
					this.animator.SetBool(animation_isWalk, false);
					this.animator.SetBool(animation_isAttack, true);
					this.animator.SetBool(animation_isDamage, false);
					this.animator.SetBool(animation_isDead, false);
					break;
				case PLAYER_STATE.DAMAGE:
					this.animator.SetBool(animation_isWalk, false);
					this.animator.SetBool(animation_isAttack, false);
					this.animator.SetBool(animation_isDamage, true);
					this.animator.SetBool(animation_isDead, false);
					break;
				case PLAYER_STATE.DEAD:
					this.animator.SetBool(animation_isWalk, false);
					this.animator.SetBool(animation_isAttack, false);
					this.animator.SetBool(animation_isDamage, false);
					this.animator.SetBool(animation_isDead, true);
					break;
			}
			oldState = pState;
		}
	}

	//プレイヤーの現在位置を変数に入れる
	private void PlayerPresentLocation()
	{
		GameManager.instance.SetPlayerLocation(this.transform.position);
	}

}
