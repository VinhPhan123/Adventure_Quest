using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    
    private static SceneLoader _instance;
    public static SceneLoader Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("SceneLoader không tồn tại!");
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Ẩn loading screen khi khởi động
        if (loadingScreen)
            loadingScreen.SetActive(false);
    }
    
    public void LoadScene(string sceneName)
    {
        // Đảm bảo timeScale được reset
        Time.timeScale = 1f;
        
        // Hiển thị loading screen
        if (loadingScreen)
            loadingScreen.SetActive(true);
            
        // Bắt đầu quá trình load bất đồng bộ
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
    
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Đợi một frame để UI cập nhật
        yield return null;
        
        // Dọn dẹp bộ nhớ
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        
        // Bắt đầu tải scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        // Đợi cho đến khi load xong 90%
        while (operation.progress < 0.9f)
        {
            yield return null;
        }
        
        // Đợi thêm một chút để đảm bảo loading screen hiển thị đủ lâu
        yield return new WaitForSeconds(0.2f);
        
        // Hoàn thành việc load scene
        operation.allowSceneActivation = true;
        
        // Ẩn loading screen khi scene mới đã hoạt động
        while (!operation.isDone)
        {
            yield return null;
        }
        
        if (loadingScreen)
            loadingScreen.SetActive(false);
    }
}