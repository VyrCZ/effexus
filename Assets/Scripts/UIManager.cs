using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image crosshairNormal;
    public Image crosshairPoint;

    private Color normalCrosshairColor = Color.white;
    public Color highlightCrosshairColor = Color.white;

    public bool highlightedCrosshair; // isn't needed for the point crosshair
    public bool pointingCrosshair; // won't be highlighted

    public Color hasItemColor = Color.white;
    public Color missingItemColor = Color.white;
    public GameObject itemContainer;
    public bool[] hasItem = new bool[3]; // Items: ActivatorUp, ActivatorDown, TpCore

    public GameObject finishScreen;
    public bool finished { get; private set; }

    public GameObject pauseScreen;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public bool paused;

    //private bool[] prevItemStates = new bool[3];

    void Start()
    {
        normalCrosshairColor = crosshairNormal.color;
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.5f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 0.5f);
        //UpdateItemStates();
    }

    void Update(){
        if(highlightedCrosshair){
            crosshairNormal.color = highlightCrosshairColor;
        } else {
            crosshairNormal.color = normalCrosshairColor;
        }

        if(pointingCrosshair){
            crosshairPoint.gameObject.SetActive(true);
            crosshairNormal.gameObject.SetActive(false);
        } else {
            crosshairPoint.gameObject.SetActive(false);
            crosshairNormal.gameObject.SetActive(true);
        }

        /*if(prevItemStates != hasItem){
            UpdateItemStates();
            prevItemStates = hasItem;
        }*/
        UpdateItemStates();
        if(finished && Input.GetKeyDown(KeyCode.Return)){
            NextLevel();
        }
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)){
            Pause();
        }

        if(paused){
            Cursor.lockState = CursorLockMode.None;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void UpdateItemStates(){
        for(int i = 0; i < hasItem.Length; i++){
            if(hasItem[i]){
                itemContainer.transform.GetChild(i).GetComponent<Image>().color = hasItemColor;
            } else {
                itemContainer.transform.GetChild(i).GetComponent<Image>().color = missingItemColor;
            }
        }
    }

    public void Pause(){
        paused = !paused;
        pauseScreen.SetActive(paused);
        Time.timeScale = paused ? 0 : 1;
    }

    public void VolumeChanged(){
        float volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volume);
        FindObjectOfType<AudioManager>().SetVolume(volume);
    }

    public void SensitivityChanged(){
        float sensitivity = sensitivitySlider.value;
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        FindObjectOfType<PlayerMovement>().GetComponentInChildren<CameraController>().mouseSensitivity = sensitivity * 300;
    }

    public void NextLevel(){
        // unpause
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }

    public void Finish(){
        finished = true;
        finishScreen.SetActive(true);
        // show cursor and disable player movement
        paused = true;
        Time.timeScale = 0;
    }
}
