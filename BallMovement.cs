using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SceneManagement;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private GameObject _board;                         // 板のオブジェクト 
    [SerializeField] private GameObject _player;                    // プレイヤーオブジェクト
    [SerializeField] private float _maxG = 0.1f;                        // 角度によって変わる重力の最大値
    [SerializeField] private float _minG = 0.05f;                        // 角度によって変わる重力の最低値
    [SerializeField, Range(0, 1)] float _friction = 0.03f;              // ボードとボールの間の摩擦
    [SerializeField] private float _bombPower = 3;
    [SerializeField] private GameObject _ragdoll;
    [SerializeField] private GameOverCamera _cameraScript;
    [SerializeField] private Transform[] _rootBone;
    [SerializeField] private GameObject _bombEffect;
    [SerializeField] private GameTimer _timer;
    [SerializeField] private GameObject _boardPrefab;

    [SerializeField] private GameObject _gameoverSE;


    private Rigidbody _rb;                                              // 物理演算
    private PlayerBoardMover _boardMover;
    private BoardAngleCheck _boardScript;                                 // 板の角度を取得する為のスクリプト
    private GameObject[] _players;                                      // プレイヤーたちを取得するための配列
    private bool _onBoard;                                                   // ボードに乗っているか。
    private float _prevRotate;                                               // 最初のプレイヤーの向き
    private GameObject _goal;
    private bool _gameEnd;
    private int _remainingLife;
    private GameObject _playerObject;
    private bool _dead;

    // 菅原慶太が追加 2026/01/24 板角度リセット用
    public bool Dead => _dead;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();                                
        _boardMover = _player.GetComponent<PlayerBoardMover>();
        _boardScript = _board.GetComponent<BoardAngleCheck>();
        _prevRotate = _player.transform.eulerAngles.y;
        _players = GameObject.FindGameObjectsWithTag("Player");         // プレイヤー配列にタグで検索して格納する

        transform.parent = null;
        _goal = GameObject.Find("Goal");
        _gameEnd = false;

        // テスト用に菅原が追加 ---> 63行目も変更
        //GetBoardVelocity();

        _remainingLife = 2;

        _playerObject = GameObject.Find("PlayerAndBall");

        _dead = false;

    }

    // テスト用に菅原が追加 ---> 63行目も変更
    //public Vector3 GetBoardVelocity()
    //{
    //    if (_boardMover != null) return _boardMover.GetVelocity();

    //    var wasd = _player.GetComponent<TestPlayerMover>();
    //    if (wasd != null) return wasd.GetVelocity();

    //    return Vector3.zero;
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity = Vector3.Lerp(_rb.velocity,_boardMover.GetVelocity(),_friction) + Gravity();

        // テスト用に菅原が追加
        // _rb.velocity = Vector3.Lerp(_rb.velocity, GetBoardVelocity(), _friction) + Gravity();
    }
    /// <summary>
    /// 板の角度を取得した上でその値に応じて重力を決定する
    /// </summary>
    private Vector3 Gravity()
    {
        var tanX = Mathf.Abs(_boardScript.GetTanX());
        var tanZ = Mathf.Abs(_boardScript.GetTanZ());

        if (tanX > 30)
        {
            tanX = Mathf.Lerp(0, 30,Mathf.Abs((tanX - 360) / 30)) ;
        }
        if(tanZ > 30)
        {
            tanZ = Mathf.Lerp(0, 30, Mathf.Abs((tanZ - 360) / 30));
        }
                                             // 横方向 0～0.5                               縦方向 0～0.5                           縦横 足して0～1;
        var gravity = Mathf.Lerp(_minG, _maxG,tanX / 60 + tanZ / 60);

        var force = new Vector3(0, -gravity, 0) / 10;
        return force;

    }
   

    /// <summary>
    /// ボールが Player タグ以外のオブジェクトにぶつかった時ゲームオーバーになる
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _onBoard = true;
        }
        if(collision.gameObject.tag != "Player")
        {
            if(_dead == false)
            {

                
                Bomb();

                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "SafeArea" && _onBoard == true)
        {
            _onBoard = false;

            GetComponent<GameOverCamera>().GameOver();
        }
    }

    private void Bomb()
    {

        var board = _board.transform;
        _gameoverSE.SetActive(true);
        Instantiate(_bombEffect, transform.position, Quaternion.identity);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] ragdolls = { null, null, null, null };


        int i = 0;
        foreach (GameObject p in players)
        {
            //var controller = p.GetComponent<PlayerInputCntroller>();
            //var ik = p.GetComponent<BoardIk>();
            //var anim = p.GetComponent<Animator>();
            var input = p.GetComponent<PlayerInput>();
            //var collider = p.GetComponent<CapsuleCollider>();
            //var boxcollider = p.GetComponent<BoxCollider>();
            //var control = p.GetComponent<CharacterController>();
            //var follow = p.GetComponent<PlayerPlatformFollow>();
            //var mover = p.GetComponent<PlayerBoardMover>();
            GameObject rag;
            //if(mover != null)
            //{
            //    Destroy(mover);
            //}
            //if (boxcollider != null)
            //{
            //    boxcollider.enabled = false;
            //}
            //if (follow != null)
            //{
            //    Destroy(follow);
            //}
            //if (control != null)
            //{
            //    Destroy(control);
            //}


            //if (controller != null)
            //{
            //    Destroy(controller);
            //}
            //if (ik != null)
            //{
            //    Destroy(ik);
            //}
            //if (collider != null)
            //{
            //    collider.enabled = true;
            //}
            if (input != null)
            {
                rag = Instantiate(_ragdoll, p.transform.position, p.transform.rotation);
                int playerColNum = 0;
                switch (p.GetComponent<PlayerInputCntroller>().Side)
                {
                    case PlayerInputCntroller.BoardSide.Front:
                        playerColNum = 1;
                        break;
                    case PlayerInputCntroller.BoardSide.Right:
                        playerColNum = 2;
                        break;
                    case PlayerInputCntroller.BoardSide.Left:
                        playerColNum = 3;
                        break;
                    case PlayerInputCntroller.BoardSide.Back:
                        playerColNum = 0;
                        break;

                }
                rag.GetComponent<RagDollMaterialChanger>().MaterialChange(playerColNum);


                foreach (Transform t in _rootBone)
                {
                    if (t.parent.parent.parent.parent.gameObject == p)
                    {
                        rag.GetComponent<RagdollSetup>().Setup(t);
                        break;
                    }
                }
                ragdolls[i] = rag;
                Destroy(rag, 2.0f);
                i++;
                input.enabled = true;
            }


        }
        foreach (var r in _playerObject.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        foreach (var c in _playerObject.GetComponentsInChildren<Collider>())
        { 
            c.enabled = false;
        }
        foreach (GameObject p in ragdolls)
        {
            if (p == null) continue;
            var forceVector = (p.transform.position - transform.position + Vector3.up * 1.25f) * _bombPower;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f);
            foreach (Collider collider in colliders)
            {
                var crb = collider.GetComponent<Rigidbody>();
                if(crb != null)
                {
                    crb.AddForce(forceVector, ForceMode.Impulse);
                    crb.AddTorque(forceVector, ForceMode.Impulse);
                }
            }
            
        }
        GameObject dammy = Instantiate(_boardPrefab, board.position, board.rotation);
        var force = (dammy.transform.position - transform.position + Vector3.up * 1.25f) * _bombPower;

        dammy.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        Destroy(dammy, 2.0f);

        GetComponent<Renderer>().enabled = false;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _dead = true;
        _remainingLife -= 1;
    }

    public int GetRemainingLife()
    {
        return _remainingLife;
    }

    public void SetActivePlayers()
    {
        _dead = false;

        foreach (var r in _playerObject.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }

        foreach (var c in _playerObject.GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }
        GetComponent<Renderer>().enabled = true;
        this.gameObject.transform.position = _board.transform.position + new Vector3(0, 4, 0);

    }
}
