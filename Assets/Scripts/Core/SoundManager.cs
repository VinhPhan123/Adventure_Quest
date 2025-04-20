// using UnityEditor.SearchService;
// using UnityEngine;

// public class SoundManager : MonoBehaviour
// {
//     public static SoundManager instance { get; private set; }
//     private AudioSource soundSource;
//     private AudioSource musicSource;

//     private void Awake()
//     {
//         soundSource = GetComponent<AudioSource>();
//         musicSource = transform.GetChild(0).GetComponent<AudioSource>();

//         //Keep this object even when we go to new scene
//         if (instance == null)
//         {
//             instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         //Destroy duplicate gameobjects
//         else if (instance != null && instance != this)
//             Destroy(gameObject);

//         //Assign initial volumes
//         ChangeMusicVolume(0);
//         ChangeSoundVolume(0);
//     }
//     public void PlaySound(AudioClip _sound)
//     {
//         soundSource.PlayOneShot(_sound);
//     }

//     public void ChangeSoundVolume(float _change)
//     {
//         ChangeSourceVolume(1, "soundVolume", _change, soundSource);
//     }
//     public void ChangeMusicVolume(float _change)
//     {
//         ChangeSourceVolume(0.3f, "musicVolume", _change, musicSource);
//     }

//     private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
//     {
//         //Get initial value of volume and change it
//         float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
//         currentVolume += change;

//         //Check if we reached the maximum or minimum value
//         if (currentVolume > 1)
//             currentVolume = 0;
//         else if (currentVolume < 0)
//             currentVolume = 1;

//         //Assign final value
//         float finalVolume = currentVolume * baseVolume;
//         source.volume = finalVolume;

//         //Save final value to player prefs
//         PlayerPrefs.SetFloat(volumeName, currentVolume);
//     }
// }


using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource soundSource;
    private AudioSource musicSource;
    
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip levelMusic;
    
    private string currentSceneName;
    private bool isMainMenu = false;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        musicSource = transform.GetChild(0).GetComponent<AudioSource>();

        //Keep this object even when we go to new scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Đăng ký sự kiện chuyển cảnh
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Lưu tên cảnh hiện tại
            currentSceneName = SceneManager.GetActiveScene().name;
            CheckIfMainMenu(currentSceneName);
        }
        //Destroy duplicate gameobjects
        else if (instance != null && instance != this)
            Destroy(gameObject);

        //Assign initial volumes
        ChangeMusicVolume(0);
        ChangeSoundVolume(0);
    }
    
    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi đối tượng bị hủy
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        bool wasMainMenu = isMainMenu;
        
        // Kiểm tra xem cảnh mới có phải là Main Menu không
        CheckIfMainMenu(newSceneName);
        
        // Nếu chuyển từ Main Menu sang Level hoặc ngược lại, thay đổi âm nhạc
        if (wasMainMenu != isMainMenu)
        {
            SwitchMusic();
        }
        
        currentSceneName = newSceneName;
    }
    
    private void CheckIfMainMenu(string sceneName)
    {
        // Điều chỉnh điều kiện này phù hợp với tên cảnh Main Menu của bạn
        isMainMenu = sceneName == "_MainMenu";
    }
    
    private void SwitchMusic()
    {
        // Dừng nhạc hiện tại
        musicSource.Stop();
        
        // Phát nhạc tương ứng với loại cảnh
        musicSource.clip = isMainMenu ? mainMenuMusic : levelMusic;
        musicSource.Play();
    }
    
    // Phương thức để gọi từ bên ngoài để chuyển đổi nhạc thủ công
    public void PlayMusicForScene(bool isMenu)
    {
        isMainMenu = isMenu;
        SwitchMusic();
    }

    public void PlaySound(AudioClip _sound)
    {
        soundSource.PlayOneShot(_sound);
    }

    public void ChangeSoundVolume(float _change)
    {
        ChangeSourceVolume(1, "soundVolume", _change, soundSource);
    }
    
    public void ChangeMusicVolume(float _change)
    {
        ChangeSourceVolume(0.3f, "musicVolume", _change, musicSource);
    }

    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        //Get initial value of volume and change it
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        //Check if we reached the maximum or minimum value
        if (currentVolume > 1)
            currentVolume = 0;
        else if (currentVolume < 0)
            currentVolume = 1;

        //Assign final value
        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;

        //Save final value to player prefs
        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }
}