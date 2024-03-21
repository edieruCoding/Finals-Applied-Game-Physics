using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action<Vector3> StartingPosEvent; 
    public static event Action RestartEvent; 
    public int _levelOverride;
    
    public GameObject levelCompletePanel;
    public GameObject pausePanel;
    public GameObject gameCompletePanel;
    
    private int _currentLevel = 1;
    private List<GameObject> _levelsObjects;
    private bool _paused;

    AudioSource audioData;
    
    private void OnEnable()
    {
        CupCollider.CupColliderEvent += LevelDone;
    }
    
    private void OnDisable()
    {
        CupCollider.CupColliderEvent -= LevelDone;
    }

    private void Update()
    {
        //pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_paused)
            {
                Resume();
                _paused = false;
            }
            else
            {
                _paused = true;
                Pause();
            }
        }
    }

    private void Start()
    {
        _levelsObjects = new List<GameObject>();
        audioData = GetComponent<AudioSource>();

        foreach (Transform child in transform)
        {
            _levelsObjects.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        
        //debug
        if (_levelOverride <= 0)
        {
            _levelOverride = 1 ;
        }
        
        if (_levelOverride > _levelsObjects.Count)
        {
            _levelOverride = _levelsObjects.Count ;
        }
        
        _currentLevel = _levelOverride;
        
        //start
        LoadFirstLevel();
    }

    private void LoadFirstLevel()
    {
        _levelsObjects[_currentLevel - 1].SetActive(true);

        Transform t = _levelsObjects[_currentLevel - 1].transform.GetChild(0);
        
        levelCompletePanel.SetActive(false);
        
        StartingPosEvent?.Invoke(t.position);
        RestartButton();
    }

    //used in button in unity
    public void MoveToNextLevel()
    {
        _levelsObjects[_currentLevel - 1].SetActive(false);
        
        _currentLevel++;
        
        _levelsObjects[_currentLevel - 1].SetActive(true);

        Transform t = _levelsObjects[_currentLevel - 1].transform.GetChild(0);
        
        levelCompletePanel.SetActive(false);
        
        StartingPosEvent?.Invoke(t.position);
    }

    public void ShowLevelCompletePanel()
    {
        levelCompletePanel.SetActive(true);
    }
    
    public void ShowGameCompletePanel()
    {
        gameCompletePanel.SetActive(true);
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        RestartEvent?.Invoke();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    
    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Mainmenu");
    }

    private void LevelDone()
    {
        if (_currentLevel ==  _levelsObjects.Count)
        {
            ShowGameCompletePanel();
        }
        else
        {
            ShowLevelCompletePanel();
        }
        
        audioData.Play(0);
    }
    
}
//ANDRES