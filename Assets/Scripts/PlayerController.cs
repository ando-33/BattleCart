using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 6.0f;
    const float StunDuration = 0.5f;
    float recoverTime = 0.0f;

    public int life = 10;

    CharacterController controller;

    Vector3 moveDirection = Vector3.zero;
    int targetLane;

    public float gravity = 9.81f; //重力

    public float speedZ = 10; //
    public float accelerationZ = 8; //

    public float speedX = 10; //

    public float speedJump = 10; //

    public GameObject body;

    public GameObject boms;






    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        //ゲームステータスがplayingの時のみ左右に動かせる
        if (GameManager.gameState == GameState.playing)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) MoveToLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) MoveToRight();

        }

        //もしスタン中かLifeが0なら動きを止める
        if (IsStun())
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            //復活までの時間をカウント
            recoverTime -= Time.deltaTime;

            //点滅処理
            Blinking();
        }
        else
        {
            //
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

            //
            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
            moveDirection.x = ratioX * speedX;
        }

        //重力文の力をフレーム追加
        moveDirection.y -= gravity * Time.deltaTime;

        //
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        //
        if (controller.isGrounded) moveDirection.y = 0;
    }
    //左のレーンに移動を開始
    public void MoveToLeft()
    {

        if (controller.isGrounded && targetLane > MinLane)
            targetLane--;
    }

    //
    public void MoveToRight()
    {

        if (controller.isGrounded && targetLane < MaxLane)
            targetLane++;
    }

    //ジャンプ
    public void Jump()
    {

        //地面に接触していればY方向の力を設定
        if (controller.isGrounded) moveDirection.y = speedJump;
    }

    //体力をリターン
    public int Life()
    {
        return life;
    }

    //スタン中かチェック
    bool IsStun()
    {
        //recoverTimeが作動中かLifeが0になった場合はStunフラグがON
        bool stun = recoverTime > 0.0f || life <= 0;
        //スタンフラグがOFFの場合はボディを確実に表示
        if (!stun) body.SetActive(true);
        //Stunフラグをリターン
        return stun;

    }

    //接触判定
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        //ぶつかった相手がEnemyなら
        if (hit.gameObject.CompareTag("Enemy"))
        {
            //体力をマイナス
            life--;

            if (life <= 0)
            {
                GameManager.gameState = GameState.gameover;
                Instantiate(boms, transform.position, Quaternion.identity); //爆発エフェクトの発生
                Destroy(gameObject, 0.5f); //
            }
            //recoverTimeの時間を設定
            recoverTime = StunDuration;
            //接触したEnemyを削除
            Destroy(hit.gameObject);
        }
    }

    //点滅処理
    void Blinking()
    {
        //
        float val = Mathf.Sin(Time.time * 50);
        //
        if (val >= 0) body.SetActive(ture);
        //
        else body.SetActive(false);

    }
}

