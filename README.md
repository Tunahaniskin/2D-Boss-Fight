# 🎮 2D Boss Fight & AI Project 🚀

![Unity](https://img.shields.io/badge/Unity-2022%2B-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue?style=flat&logo=csharp)
![Platform](https://img.shields.io/badge/Platform-WebGL%20%7C%20PC-green?style=flat)
![AI](https://img.shields.io/badge/AI-Reinforcement%20Learning-red?style=flat)

Unity oyun motoru kullanılarak geliştirilmiş, **Reinforcement Learning (Pekiştirmeli Öğrenme)** destekli bir 2D Boss Dövüş oyunudur. Bu projede düşman karakteri (AI), oyuncunun hareketlerine tepki vermeyi ve stratejik saldırmayı Q-Learning algoritması ile öğrenmiştir.

---

## 🕹️ Oyunu Tarayıcıda Oyna

Oyun **WebGL** formatında derlenmiştir. İndirme yapmadan doğrudan tarayıcı üzerinden oynayabilirsiniz:

### 👉 [ itch.io Üzerinden Oyna ](https://perhaskell.itch.io/2d-boss-figth) 👈

---

## ✨ Temel Özellikler

### 🧠 Yapay Zeka (AI) ve Q-Learning
* **Akıllı Düşman:** Boss karakteri rastgele hareket etmez. Oyuncuya olan mesafeye, yöne ve duruma göre karar verir.
* **Eğitim Modu:** Ana menüdeki **"Yapay Zeka Yükle"** butonu, önceden eğitilmiş ağırlık dosyasını (`enemy_weights.json`) oyuna yükler.
* **Dinamik Öğrenme:** Düşman; menzile girmeyi, boşa vurmamayı ve oyuncuyu takip etmeyi ödül/ceza sistemiyle öğrenmiştir.

### ⚔️ Oynanış Mekanikleri
* **Can ve Hasar Sistemi:** Oyuncu ve Düşman için görsel Can Barları (Health Bars).
* **Game Over Sistemi:** Taraflardan biri öldüğünde oyun durur, kazanan ilan edilir ve "Tekrar Oyna" seçeneği sunulur.
* **Yetenekler:** Yürüme, Zıplama, Kılıç Saldırısı ve Dash (Atılma).

### 🛠️ Teknik Özellikler
* **WebGL Desteği:** `UnityWebRequest` kullanılarak tarayıcı ortamında dosya okuma işlemleri (AI verisi) sorunsuz çalışır.
* **Ses Yönetimi:** Arka plan müziği ve efektler Audio Mixer üzerinden kontrol edilebilir.

---

## 🎮 Kontroller

| Eylem | Tuş Kombinasyonu |
| :--- | :--- |
| **Yürüme** | `A` (Sola) / `D` (Sağa) |
| **Zıplama** | `Space` veya `W` |
| **Saldırı** | `Mouse Sol Tık` 🖱️ |
| **Dash (Atılma)** | `Sol Shift` (Hareket ederken) 💨 |
| **Menüye Dön** | Oyun Bitti ekranında buton ile |

---

## 🧠 Yapay Zeka Mimarisi (AI Architecture)

Projede **Reinforcement Learning (Pekiştirmeli Öğrenme)** yöntemlerinden biri olan **Q-Learning** kullanılmıştır. Ajan (Düşman), ortamdan aldığı geri bildirimlere (Ödül/Ceza) göre `Q-Table` üzerindeki değerleri günceller.



### 🎯 Ödül ve Ceza Sistemi (Reward Function)

+ [BAŞARILI]  Oyuncuya kılıç ile hasar verirse (+2.0 Puan)
+ [NAVİGASYON] Uzaktayken oyuncuya doğru yürürse (+0.1 Puan)
+ [STRATEJİ]  Platform farkı varken doğru yerde zıplarsa (+0.5 Puan)

- [HATA]      Saldırıyı ıskalarsa (Boşa vurursa) (-0.5 Puan)
- [HATA]      Ters yöne (arkasına) saldırırsa (-0.5 Puan)
- [PASİF]     Saldırı menzilindeyken boş boş yürürse (-0.2 Puan)
