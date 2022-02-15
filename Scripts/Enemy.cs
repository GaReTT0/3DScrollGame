using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミー関連の処理を行うEnemyクラス
/// </summary>
public class Enemy : Character
{
	// エネミーの状態の型の列挙型
	public enum ENEMY_STATE
	{
		WAIT,
		RUN,
		WALK,
	}

	//エネミーの状態
	private ENEMY_STATE eState;
	//一つ前のエネミーの状態
	private ENEMY_STATE oldeState;
	//CharacterControllerクラスを使用
	private CharacterController enemyController;
	//X軸方向に移動した距離
	private float moveX;
	//Y軸方向に移動した距離
	private float moveY;
	//移動した量を格納する
	private Vector3 moveDirection;
	//ジャンプしたベクトルと重力を格納する
	private Vector3 jumpDirection;
	//移動速度を制限
	private float speedControl = 0.8f;
	//キャラクターの向き(trueは画面右向き)
	private int characterDirection = 1;
	//キャラクター全員におけるプレイヤーの番号
	private int playerNum = 1;
	//キャラクターにかかる重力
	private float enemyGravity;
	//動き始めた時間
	private float moveStartTime;
	//接地判定
	private bool isGround;
	//レイが何かに当たった時の情報
	private RaycastHit hit;
	//レイを飛ばす距離
	[SerializeField] private float distance;
	//アニメーター
	[SerializeField] private Animator animator;
	//設定したフラグ
	private const string animation_isWalk = "Walk";
	private const string animation_isRun = "Run";



	// Start is called before the first frame update
	void Start()
	{
		enemyController = GetComponent<CharacterController>();
		enemyGravity = GameManager.instance.GetGravity();
		eState = ENEMY_STATE.WALK;
		ChangeAnimation(eState);
	}

	// Update is called once per frame
	void Update()
	{
		GroundJudge();
		RotationCharacter();
		Move();
		CalcGravity();
		CalcMoveDistance();
	}

	//接地判定
	private void GroundJudge()
	{
		Vector3 rayPosition = this.transform.position + new Vector3(0.0f, -0.1f, 0.0f);
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
	}

	//プレイヤーが索敵範囲内に入っていない時に呼び出され続ける
	protected override void Move()
	{
		if (eState != ENEMY_STATE.RUN && isGround)
		{
			eState = ENEMY_STATE.WALK;
			ChangeAnimation(eState);
			moveStartTime += Time.deltaTime;
			if (moveStartTime < 5.0f)
			{
				moveX = GameManager.instance.GetMoveSpeed(playerNum) * Time.deltaTime;
			}
			else if (moveStartTime >= 5.0f && moveStartTime < 10.0f)
			{
				moveX = (-1) * GameManager.instance.GetMoveSpeed(playerNum) * Time.deltaTime;
			}
			else
			{
				moveStartTime = 0;
			}
		}
	}

	protected override void Attack() { }

	//ダメージを与えられた時に呼び出される
	protected override void Damage()
	{
		GameManager.instance.CalcScore(100);
		Destroy(this.gameObject);
	}

	//プレイヤーを追いかける
	private void ChasePlayer()
	{
		if (isGround)
		{
			eState = ENEMY_STATE.RUN;
			ChangeAnimation(eState);
			if (GameManager.instance.GetPlayerLocation().x - this.gameObject.transform.position.x < 0)
			{
				moveX = (-1) * GameManager.instance.GetMoveSpeed(playerNum) * Time.deltaTime * speedControl;
			}
			else if (GameManager.instance.GetPlayerLocation().x - this.gameObject.transform.position.x > 0)
			{
				moveX = GameManager.instance.GetMoveSpeed(playerNum) * Time.deltaTime * speedControl;
			}
			else { }
		}
	}

	//パーティクルの当たり判定を受けた時に呼び出される
	private void OnParticleCollision(GameObject other)
	{
		Damage();
	}

	//キャラクターを回転させる
	private void RotationCharacter()
	{
		Vector3 scale = gameObject.transform.localScale;
		if (scale.x > 0 && moveX < 0 || scale.x < 0 && moveX > 0)
		{
			characterDirection *= -1;
			scale.x *= -1;
			scale.z *= -1;
			gameObject.transform.localScale = scale;
		}
	}

	//重力を計算する
	private void CalcGravity()
	{
		//キャラクターにかける重力が過剰にjumpDirection.yに加算されることを防ぐ
		if (jumpDirection.y >= enemyGravity)
		{
			jumpDirection.y += enemyGravity * Time.deltaTime; //常にy座標を重力の分だけ動かす(重力処理)
		}
	}

	//キャラクターの移動量を反映
	private void CalcMoveDistance()
	{
		//移動量を格納
		moveDirection = new Vector3(moveX, 0, 0);
		//CharacterControllerをmoveDirectionの方向に動かす
		enemyController.Move(moveDirection);
		//CharacterControllerをjumpDirectionの方向に動かす
		enemyController.Move(jumpDirection * Time.deltaTime);
	}

	//コライダーを持つ物体が範囲内に入った時に呼び出される
	private void OnTriggerEnter(Collider other)
	{
		switch (other.gameObject.tag)
		{
			case "Player":
				ChasePlayer();
				break;

			default:
				break;
		}
	}

	//コライダーを持つ物体が範囲内に入っている時に呼び出される
	private void OnTriggerStay(Collider other)
	{
		switch (other.gameObject.tag)
		{
			case "Player":
				ChasePlayer();
				break;

			default:
				break;
		}
	}

	//コライダーを持つ物体が範囲外に出た時に呼び出される
	private void OnTriggerExit(Collider other)
	{
		switch (other.gameObject.tag)
		{
			case "Player":
				eState = ENEMY_STATE.WALK;
				Move();
				break;

			default:
				break;
		}
	}

	//プレイヤーの状態によってアニメーションを切り替える
	private void ChangeAnimation(ENEMY_STATE state)
	{
		if (oldeState != eState)
		{
			switch (eState)
			{
				case ENEMY_STATE.WAIT:
					this.animator.SetBool(animation_isWalk, false);
					this.animator.SetBool(animation_isRun, false);
					break;
				case ENEMY_STATE.WALK:
					this.animator.SetBool(animation_isWalk, true);
					this.animator.SetBool(animation_isRun, false);
					break;
				case ENEMY_STATE.RUN:
					this.animator.SetBool(animation_isWalk, false);
					this.animator.SetBool(animation_isRun, true);
					break;
			}
			oldeState = eState;
		}
	}
}
