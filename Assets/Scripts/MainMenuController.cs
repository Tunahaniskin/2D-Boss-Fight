using UnityEngine.Networking; // İNTERNETTEN DOSYA ÇEKMEK İÇİN GEREKLİ
using System.Collections; // COROUTINE İÇİN GEREKLİ
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

        StartCoroutine(LoadAIDataRoutine());
    }

    IEnumerator LoadAIDataRoutine()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "enemy_weights.json");

        Debug.Log("Dosya aranıyor: " + path); 

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {

                Debug.LogError("Hata Oluştu: " + www.error);
                if (aiInfoText != null) 
                {
                    aiInfoText.text = "Dosya Bulunamadı!";
                    aiInfoText.color = Color.red;
                }
            }
            else
            {

                string jsonContent = www.downloadHandler.text;

                Enemy_sc.loadedBrainData = jsonContent;
                Enemy_sc.usePreTrainedAI = true;

                Debug.Log("Başarılı! İçerik: " + jsonContent); 
                
                if (aiInfoText != null) 
                {
                    aiInfoText.text = "AI Yüklendi! (WebGL)";
                    aiInfoText.color = Color.green;
                }
            }
        }
    }
 
}