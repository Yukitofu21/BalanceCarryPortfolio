using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverCamera : MonoBehaviour
{
    [SerializeField] private Transform[] _transforms = { null,null,null };
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _bgm;

    [SerializeField] private Canvas _ui;
    [SerializeField] private Animator _animator;

    [SerializeField] private Image _heart1;
    [SerializeField] private Image _heart2;
    [SerializeField] private Image _heart3;

    [SerializeField] private PlayerInputCntroller[] _controllers;

    [SerializeField] private Image _countDownUI;
    [SerializeField] private Sprite[] _countDownImages;

    private GameObject _camera;
    private CinemachineVirtualCamera _virtualCamera;
    private bool dead;
    private Vector3 _firstCameraPos;
    private Quaternion _firstCameraRot;
    private CinemachineBrain _brain;
    private Camera _cameraComponent;
    private Fades _fade;


    private void Awake()
    {
        dead = false;
        _camera = GameObject.Find("Camera");
        _firstCameraPos = _camera.transform.position;
        _firstCameraRot = _camera.transform.rotation;
        _brain = _camera.GetComponent<CinemachineBrain>();
        _virtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        _cameraComponent = _camera.GetComponent<Camera>();
        _canvas.enabled = false;
        _fade = _canvas.GetComponent<Fades>();
        


    }
    public void GameOver()
    {
        _bgm.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            _ui.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        for(int i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].enabled = false;
        }
        

        StartCoroutine("CameraSwap");
    }


    IEnumerator CameraSwap()
    {

        var remainingLife = gameObject.GetComponent<BallMovement>().GetRemainingLife();

        


        if (remainingLife >0)
        {
            Debug.Log(remainingLife);

            yield return new WaitForSeconds(1.0f);

            if (remainingLife == 2)
            {
                _animator.SetBool("Damage1", true);
            }
            else if(remainingLife == 1)
            {
                _animator.SetBool("Damage2", true);
            }

                yield return new WaitForSeconds(3.0f);

            if (remainingLife == 2)
            {
                Destroy(_heart3.gameObject);
            }
            else if (remainingLife == 1)
            {
                Destroy(_heart2.gameObject);
            }


            StartCoroutine("RestartCountDown");

        }
        else
        {
            _brain.enabled = false;
            Time.timeScale = 0.3f;

            yield return new WaitForSeconds(0.15f);

            Time.timeScale = 0.08f;
            dead = true;

            yield return new WaitForSeconds(0.05f);

            for (int i = 0; i < _transforms.Length; i++)
            {
                _cameraComponent.fieldOfView = 30 + i * 5;
                _camera.transform.position = _transforms[i].position;
                _camera.transform.LookAt(this.transform.position);
                yield return new WaitForSeconds(0.05f + i / 50);
            }

            Time.timeScale = 0.5f;
            _cameraComponent.fieldOfView = 60;
            _camera.transform.position = _firstCameraPos;
            _camera.transform.rotation = _firstCameraRot;
            _virtualCamera.Follow = null;
            _virtualCamera.LookAt = GameObject.Find("PlayerBoard").transform;
            _brain.enabled = true;
            _canvas.enabled = true;

            yield return new WaitForSeconds(0.5f);

            Time.timeScale = 1;

            _animator.SetBool("Damage3", true);

            yield return new WaitForSeconds(3.0f);
            Destroy(_heart1.gameObject);
            _fade.FadeIn("GameOver");
        }


            yield break;

    }

    IEnumerator RestartCountDown()
    {
        _countDownUI.gameObject.SetActive(true);

        _countDownUI.sprite = _countDownImages[0];

        gameObject.GetComponent<BallMovement>().SetActivePlayers();



        for (int i = 0; i < _countDownImages.Length;i++)
        {

            var scale = 0.0f;
            var alpha = 0.0f;
            _countDownUI.sprite = _countDownImages[i];

            while (scale < 3.0f)
            {
                scale += 0.09f;
                alpha += 0.03f;

                _countDownUI.color = new Color(0, 0, 0, alpha);
                _countDownUI.rectTransform.localScale = Vector3.one * scale;

                yield return new WaitForEndOfFrame();
            }
            while(scale > 0)
            {
                scale -= 0.09f;
                alpha -= 0.03f;

                _countDownUI.color = new Color(0, 0, 0, alpha);
                _countDownUI.rectTransform.localScale = Vector3.one * scale;

                yield return new WaitForEndOfFrame();
            }


        }


        _countDownUI.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();





        for (int i = 0; i < 3; i++)
        {
            _ui.gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < _controllers.Length; i++)
        {
            _controllers[i].enabled = true;
        }
        _bgm.SetActive(true);


        yield break;
    }
}
