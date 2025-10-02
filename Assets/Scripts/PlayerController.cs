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

    public float gravity = 9.81f; //�d��

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
        //�Q�[���X�e�[�^�X��playing�̎��̂ݍ��E�ɓ�������
        if (GameManager.gameState == GameState.playing)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) MoveToLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) MoveToRight();

        }

        //�����X�^������Life��0�Ȃ瓮�����~�߂�
        if (IsStun())
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            //�����܂ł̎��Ԃ��J�E���g
            recoverTime -= Time.deltaTime;

            //�_�ŏ���
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

        //�d�͕��̗͂��t���[���ǉ�
        moveDirection.y -= gravity * Time.deltaTime;

        //
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        //
        if (controller.isGrounded) moveDirection.y = 0;
    }
    //���̃��[���Ɉړ����J�n
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

    //�W�����v
    public void Jump()
    {

        //�n�ʂɐڐG���Ă����Y�����̗͂�ݒ�
        if (controller.isGrounded) moveDirection.y = speedJump;
    }

    //�̗͂����^�[��
    public int Life()
    {
        return life;
    }

    //�X�^�������`�F�b�N
    bool IsStun()
    {
        //recoverTime���쓮����Life��0�ɂȂ����ꍇ��Stun�t���O��ON
        bool stun = recoverTime > 0.0f || life <= 0;
        //�X�^���t���O��OFF�̏ꍇ�̓{�f�B���m���ɕ\��
        if (!stun) body.SetActive(true);
        //Stun�t���O�����^�[��
        return stun;

    }

    //�ڐG����
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        //�Ԃ��������肪Enemy�Ȃ�
        if (hit.gameObject.CompareTag("Enemy"))
        {
            //�̗͂��}�C�i�X
            life--;

            if (life <= 0)
            {
                GameManager.gameState = GameState.gameover;
                Instantiate(boms, transform.position, Quaternion.identity); //�����G�t�F�N�g�̔���
                Destroy(gameObject, 0.5f); //
            }
            //recoverTime�̎��Ԃ�ݒ�
            recoverTime = StunDuration;
            //�ڐG����Enemy���폜
            Destroy(hit.gameObject);
        }
    }

    //�_�ŏ���
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

