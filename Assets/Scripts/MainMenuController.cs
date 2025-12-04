using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    public AudioMixer gameMixer; 
    public Slider musicSlider; 
    public Slider audioSlider;
    
  
    private const float MIN_VOLUME_LEVEL = 0; 

    
    void Start()
    {
       
        if (musicSlider == null || audioSlider == null || gameMixer == null)
        {
            Debug.LogError("Ses ayarlarının düzgün çalışması için Slider veya Mixer referansları eksik!");
            return;
        }

        SetMusicVolume(musicSlider.value); 
        
        SetAudioVolume(audioSlider.value);
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void SetMusicVolume(float volume)
    {
       
        if (volume <= MIN_VOLUME_LEVEL) 
        {
           
            gameMixer.SetFloat("MusicVolume", -80f); 
        }
        else
        {
          
            gameMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
        }
    }

   
    public void SetAudioVolume(float volume)
    {
        if (volume <= MIN_VOLUME_LEVEL)
        {
            gameMixer.SetFloat("AudioVolume", -80f);
        }
        else
        {
            gameMixer.SetFloat("AudioVolume", Mathf.Log10(volume) * 20f);
        }
    }
 
}