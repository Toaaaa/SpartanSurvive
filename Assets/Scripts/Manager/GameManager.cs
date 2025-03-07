using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogError("GameManager instance not found!");
                }
            }
            return _instance;
        }
    }


    public Player player;
    public CameraRaycaster cameraRaycaster;



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

    }
}

// public 인터페이스
public interface IDamageable
{
    void TakeDamage(float damage);
}
