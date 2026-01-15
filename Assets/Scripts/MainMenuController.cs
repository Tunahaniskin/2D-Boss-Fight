using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO; // Dosya okuma işlemleri için
using TMPro; // TextMeshPro'yu tanıması için


public class MainMenuController : MonoBehaviour
{
    public AudioMixer gameMixer; 
    public Slider musicSlider; 
    public Slider audioSlider;

    [Header("AI Ayarları")]
    public TMP_Text aiInfoText;
    
  
    private const float MIN_VOLUME_LEVEL = 0; 

    
    void Start()
    {
        Enemy_sc.usePreTrainedAI = false; // Varsayılan olarak eğitimsiz başla
        Enemy_sc.loadedBrainData = "";
        if (aiInfoText != null) aiInfoText.text = "Mod: Rastgele (Eğitimsiz)";

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

    public void LoadAIData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "enemy_weights.json");

        if (File.Exists(path))
        {
            
            string jsonContent = File.ReadAllText(path);
            Enemy_sc.loadedBrainData = jsonContent;
            Enemy_sc.usePreTrainedAI = true; 

            Debug.Log("AI Dosyası Yüklendi!");
            if (aiInfoText != null) 
            {
                aiInfoText.text = "AI Yüklendi! (Akıllı Mod)";
                aiInfoText.color = Color.green;
            }
        }
        else
        {
            Debug.LogError("Dosya Bulunamadı!");
            if (aiInfoText != null) 
            {
                aiInfoText.text = "Dosya Yok!";
                aiInfoText.color = Color.red;
            }
        }
    }
 
}