using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    // Update metodu her karede tuş kontrolü yapacaktır
    void Update()
    {
        // '.' (nokta) tuşuna basıldığında kontrol
        if (Input.GetKeyDown(KeyCode.O))
        {
            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        // 1. Sahneyi Yükle
        // "MainMenu" sizin Ana Menü sahnenizin adı olmalıdır.
        SceneManager.LoadScene("MainMenu");
        
        // 2. Oyunu Duraklatma (Opsiyonel ama Önerilir)
        // Eğer menüye geçiş yapmadan önce bir miktar beklemek istiyorsanız
        // Time.timeScale = 0f; komutuyla oyunu duraklatabilirsiniz, 
        // ancak SceneManager.LoadScene çağrısı oyunu otomatik olarak durdurur ve yeni sahneyi yükler.
    }
}