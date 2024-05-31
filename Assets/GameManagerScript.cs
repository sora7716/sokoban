using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;//�v���C���[
    public GameObject moveBoxPrefab;//�������{�b�N�X
    public GameObject boxPrefab;//�����Ȃ��{�b�N�X
    /// <summary>�ו����i�[����ꏊ�̃v���n�u</summary>
    public GameObject storePrefab;
    /// <summary>�N���A�[�������Ƃ������e�L�X�g�� GameObject</summary>
    public GameObject clearText;
    int[,,] map; // �}�b�v�̌��f�[�^�i�����j
    GameObject[,] field;    // map �����ɂ����I�u�W�F�N�g�̊i�[��
    public GameObject particlePrefab;//�p�[�e�B�N��
   //�X�e�[�W�̃i���o�[
   [SerializeField]
    private int stageNumber;
    //�V�[����؂�ւ���p�̎���
    float switchTime = 0.0f;
    public float setTime=3.0f;//�V�[���؂�ւ������Ăق�������
    public AudioSource audioSource;
    public AudioClip walkSE;
    public AudioClip resetSE;

    /// <summary>
    /// �X�e�[�W�̃��Z�b�g
    /// </summary>
    private void Reset()
    {
        if (stageNumber == 0)
        {
            SceneManager.LoadScene("Title");
        }
        else if (stageNumber == 1)
        {
            SceneManager.LoadScene("GameScene1");
        }
        else if (stageNumber == 2)
        {
            SceneManager.LoadScene("GameScene2");
        }
        else if (stageNumber == 3)
        {
            SceneManager.LoadScene("GameScene3");
        }
        else if (stageNumber == 4)
        {
            SceneManager.LoadScene("Title");
        }
    }

    /// <summary>
    ///�p�[�e�B�N���̐���
    /// </summary>
    /// <param name="moveTo">�����ꏊ</param>
    /// <param name="direction">�ǂ̕����ɏo�Ăق�����</param>
    void ParticleSpawn(Vector2 moveTo,Vector2 direction)
    {
        for(int i = 0; i < 10; i++)
        {
            GameObject instance =
                      Instantiate(particlePrefab,
                      new Vector3(moveTo.x+(1.0f*direction.x), -1 * moveTo.y+(1.0f*direction.y), 0),
                      Quaternion.identity);
        }
    }
    bool IsClear()
    {
        // �i�[�ꏊ�ꗗ�̃f�[�^�����
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(2); x++)
            {
                if (map[stageNumber,y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }   // �i�[�ꏊ�ł���ꍇ
            }
        }
        
        // �i�[�ꏊ�ɔ������邩���ׂ�
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];   // �S�[���̍��W�ɉ������邩�Ƃ��Ă���

            if (f == null || f.tag != "Box")
            {
                return false;
            }   // �i�[�ꏊ�ɔ����Ȃ��A�Ƃ����P�[�X����ł�����΃N���A���ĂȂ��Ɣ��肷��
        }

        return true;    // ���ׂĂ̊i�[�ꏊ�ɔ�������ꍇ
    }

    /// <summary>
    /// number �𓮂���
    /// </summary>
    /// <param name="number">����������</param>
    /// <param name="moveFrom">�ړ����C���f�b�N�X</param>
    /// <param name="moveTo">�ړ���C���f�b�N�X</param>
    /// <returns></returns>
    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo,Vector2 direction)
    {
        // �����Ȃ��ꍇ�� false ��Ԃ�
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0))
        {
            return false;
        }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1))
        {
            return false;
        }

        if (map[stageNumber, moveTo.y, moveTo.x] == 4)
        {
            return false;
        }

        if (field[moveTo.y, moveTo.x] != null
            && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;    // �ړ��������v�Z����
            bool success = MoveNumber(moveTo, moveTo + velocity,direction);
            if (!success)
                return false;
        }   // �ړ���ɔ��������ꍇ�̏���

        // �v���C���[�E���̋��ʏ���
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        // �I�u�W�F�N�g�̃V�[����̍��W�𓮂���
        //field[moveTo.y, moveTo.x].transform.position =
        //    new Vector3(moveTo.x, -1 * moveTo.y, 0);
        // �v���C���[or���̃I�u�W�F�N�g����AMove�R���|�[�l���g���Ƃ��Ă���
        Move move = field[moveTo.y, moveTo.x].GetComponent<Move>();
        // Move�R���|�[�l���g�ɑ΂��āA�����Ɩ��߂���
        move.MoveTo(new Vector3(moveTo.x, -1 * moveTo.y, 0));
        ParticleSpawn(moveTo,direction);
        return true;
    }

    /// <summary>
    /// �v���C���[�̍��W�𒲂ׂĎ擾����
    /// ���jGetPlayerPosition 
    /// </summary>
    /// <returns>�v���C���[�̍��W</returns>
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] != null
                    && field[y, x].tag == "Player")
                {
                    // �v���C���[��������
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);  // ������Ȃ�����
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        //�N���A�t�H���g���\��
        if (sceneName == "Title"||sceneName=="End")
        {
            clearText.SetActive(true);
        }
        else
        {

            clearText.SetActive(false);
        }
        map = new int[,,]
        {
          {//�^�C�g��
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 0, 0, 0 ,0, 0, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 1, 0, 2, 0, 3, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },

          },

          {//�Q�[��1
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0 ,0, 0, 0, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 2, 0, 3, 0, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 2, 0, 0, 0, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 1, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 3, 0, 2, 0, 0 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },

          },
          {//�Q�[��2
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 2, 0, 0, 0, 3, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 0, 0, 0 ,0, 0, 0, 0, 0, 0, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 ,3 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 0, 0, 4, 4, 4, 0, 4, 0, 0, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 0, 0, 0, 0, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 0, 0, 4, 0, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 0, 4, 3, 4, 4, 4, 0, 0, 0, 4, 4, 4, 4, 0 ,0 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 0, 0, 0, 4, 4, 4, 4, 0, 0, 2, 0, 0, 0, 0, 0 ,0 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 0, 2, 0, 4, 4, 4, 4, 0, 0, 0, 0, 4, 4, 4, 4 ,0 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 0, 0, 0, 4, 4, 4, 4, 4, 0, 0, 0, 0, 4, 4, 4 ,0 ,0 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 4, 3, 0, 0, 0 ,0 ,0 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },

          },
          {//�Q�[��3
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 3, 0, 2, 0 ,0, 0, 0, 0, 4, 4, 4, 4, 4, 3 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,3 ,4, 4, 4, 4, },
            { 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 0, 0, 0, 0, 0, 2, 0, 0, 4, 4, 4, 4, 4, 0 ,0 ,0 ,4 ,4 ,4 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 0, 0, 0, 2, 0, 0, 0, 0, 4, 4, 0, 0, 4, 0 ,0 ,0 ,4 ,4 ,4 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0 ,4 ,4 ,4 ,4 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 2, 0, 0, 0, 0, 2, 0, 1, 4, 4, 0, 4, 4, 0 ,0 ,0 ,4 ,4 ,4 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 0, 0, 0, 2, 0, 0, 0, 0, 4, 4, 0, 4, 4, 0 ,0 ,0 ,4 ,4 ,4 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 3, 0, 0, 0, 2, 0, 0, 0, 4, 4, 0, 4, 4, 0 ,0 ,0 ,0 ,2 ,0 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 3, 0, 4, 0, 0, 4, 4, 4, 4, 0, 4, 4, 3 ,2 ,0 ,0 ,0 ,0 ,0 ,0 ,3 ,4, 4, 4, 4, },
            { 4, 4, 3, 0, 0, 4, 0, 0, 0, 4, 4, 4, 4, 0, 0, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 0, 0, 3, 0, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },

          },
          {//�G���h
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4 ,4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 0, 0, 0, 0, 0, 0 ,0 ,0 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 2, 0, 1, 0, 0, 0 ,2 ,0 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0 ,0 ,3 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, 4, },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4 ,4, 4, 4, 4, },

          },
        };  // 0: �����Ȃ�, 1: �v���C���[, 2: ��

        field = new GameObject
        [
            map.GetLength(1),
            map.GetLength(2)
        ];  // map �̍s��Ɠ������ڂ̔z��������ЂƂ����

        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(2); x++)
            {
                if (map[stageNumber, y, x] == 1)
                {
                    GameObject instance =
                        Instantiate(playerPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                    field[y, x] = instance; // �v���C���[��ۑ����Ă���
                    // break;  // �v���C���[�͂P�����Ȃ̂Ŕ�����
                }   // �v���C���[���o��
                else if (map[stageNumber, y, x] == 2)
                {
                    GameObject instance =
                        Instantiate(moveBoxPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                    field[y, x] = instance; // ����ۑ����Ă���
                }   // �����o��
                else if (map[stageNumber, y, x] == 3)
                {
                    GameObject instance =
                        Instantiate(storePrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                }   // �i�[�ꏊ���o��
                else if (map[stageNumber, y, x] == 4)
                {
                    Instantiate(boxPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                }
                Instantiate(boxPrefab,
                        new Vector3(x, -1 * y, 1),
                        Quaternion.identity);
            }
        }
        
    }


    void Clear()
    {
        
        if (IsClear())
        {
            clearText.SetActive(true);
            switchTime += Time.deltaTime;
           
        }
      
    }

    void SceneChange()
    {
       
        if (switchTime >= setTime)
        {
            if (stageNumber == 0)
            {
                SceneManager.LoadScene("GameScene1");
                switchTime = 0.0f;
            }
            else if (stageNumber == 1)
            {
                SceneManager.LoadScene("GameScene2");
                switchTime = 0.0f;
            }
            else if (stageNumber == 2)
            {
                SceneManager.LoadScene("GameScene3");
                switchTime = 0.0f;
            }
            else if (stageNumber == 3)
            {
                SceneManager.LoadScene("End");
                switchTime = 0.0f;
            }
            else if (stageNumber == 4)
            {
                SceneManager.LoadScene("Title");
                switchTime = 0.0f;
            }
        }
       
    }
    void Update()
    {
        Clear();
        SceneChange();
        if (switchTime <= 0.0f)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                var playerPosition = GetPlayerIndex();
                MoveNumber(playerPosition, new Vector2Int(playerPosition.x + 1, playerPosition.y), new Vector2(-1, 0));    // ���Ɉړ�
                audioSource.PlayOneShot(walkSE);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                var playerPosition = GetPlayerIndex();
                MoveNumber(playerPosition, new Vector2Int(playerPosition.x - 1, playerPosition.y), new Vector2(1, 0));    // ���Ɉړ�
                audioSource.PlayOneShot(walkSE);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var playerPosition = GetPlayerIndex();
                MoveNumber(playerPosition, new Vector2Int(playerPosition.x, playerPosition.y - 1), new Vector2(0, -1));    // ���Ɉړ�
                audioSource.PlayOneShot(walkSE);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var playerPosition = GetPlayerIndex();
                MoveNumber(playerPosition, new Vector2Int(playerPosition.x, playerPosition.y + 1), new Vector2(0, 1));    // ���Ɉړ�
                audioSource.PlayOneShot(walkSE);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
                audioSource.PlayOneShot(resetSE);
            }
        }

    }

}