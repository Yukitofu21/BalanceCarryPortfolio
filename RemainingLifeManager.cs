using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainingLifeManager : MonoBehaviour
{

    private Transform _playerTransform;
    private float _remaingTime;
    private int _remainingLife;

    private void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("DontDestroyObject");

        if(objects .Length > 1 )
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        _remainingLife = 2;

    }

    public void SetPlayerPosition(Transform transform)
    {
        _playerTransform = transform;
    }

    public void SetTime(float remaingTime)
    {
        _remaingTime = remaingTime;
    }
    
    public int GetRemainingLife()
    {
        return _remainingLife;
    }

    public void SubRemainingLife()
    {
        _remainingLife -= 1;
    }


    public void ChangedScene()
    {
        if (_remainingLife >= 2)
        {
            GameObject playerObject = GameObject.Find("PlayerAndBall");
            if (playerObject != null)
            {
                playerObject.transform.position = _playerTransform.position;
                playerObject.transform.rotation = _playerTransform.rotation;
            }

            GameTimer timer = GameObject.Find("Timer").GetComponent<GameTimer>();
            timer.SetRemaingTime(_remaingTime);

        }
    }


}
