using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using DentedPixel;

public class MainMenuController : MonoBehaviour
{
    public Dropdown videoControlChoiceDropdown;
    List<string> VRDropOptions = new List<string> { "VR", "Swipe", "Gyroscope" };
    List<string> nonVRDropOptions = new List<string> { "Swipe" };
    int storedChoice = 0;
    bool isUserPrefSet = false;
    bool gyroAvailable = false;
    public GameObject loadingContainer;

    public Text DebugText;
    public ProgressBar progressBar;
    public GameObject progressBarGO;
    public GameObject statusTextGO;
    public ClientAPI clientAPI;

    public VideoPlayer vp;

    public GameObject checkingContentDialog;
    public AssetBundle myLoadedAssetBundle;

    public GameObject welcomeSplashScreen;

    // Start is called before the first frame update
    void Start()
    {
        welcomeSplashScreen.SetActive(true);
        CanvasGroup welcomeSplashScreenCG = welcomeSplashScreen.GetComponent<CanvasGroup>();

        string welcomeScreenShown = PlayerPrefs.GetString("AEDAR_WELCOME_SPLASH_SCREEN");
        if (!(string.Compare(welcomeScreenShown, "true") == 0))
        {
            LeanTween.alphaCanvas(welcomeSplashScreenCG, 0f, 0.6f).setDelay(3f).setOnComplete(() =>
            {
                welcomeSplashScreen.SetActive(false);
                PlayerPrefs.SetString("AEDAR_WELCOME_SPLASH_SCREEN", "true");
            });
        }
        else
        {
            welcomeSplashScreen.SetActive(false);
        }

        string videosPath = Path.Combine(Application.persistentDataPath, "videos");

        // Prevent phone from sleeping
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // DirectoryInfo dirInfo = new DirectoryInfo(videosPath);
        // FileInfo[] filesInfo = dirInfo.GetFiles();
        // Debug.Log(Application.persistentDataPath);

        // if(filesInfo.Length >= 11) {
        //     Debug.Log(filesInfo.Length);
        // } else {
        //     // Directory.CreateDirectory (videosPath);
        //     // Start Downloading Assets

        // }

        try
        {
            statusTextGO.SetActive(false);
            progressBarGO.SetActive(true);
            progressBar.BarValue = 0;

            // If already downloaded then don't download videos again
//             if (!File.Exists(Path.Combine(Application.persistentDataPath, "androidvideos")))
//             {
// #if UNITY_IOS
//                 StartCoroutine(clientAPI.DownloadAssets("ios", OnCheckDownloadComplete));
// #endif

// #if UNITY_ANDROID
//                 StartCoroutine(clientAPI.DownloadAssets("android", OnCheckDownloadComplete));
// #endif
//             }
//             else
//             {
//                 checkingContentDialog.SetActive(false);
//             }
            checkingContentDialog.SetActive(false);

            // Check PlayerPrefs
            isUserPrefSet = PlayerPrefs.HasKey("VR_CHOICE");
            Debug.Log(isUserPrefSet);

            // Check for gyro options
            if (SystemInfo.supportsGyroscope)
            {
                //Gyro is available
                videoControlChoiceDropdown.ClearOptions();
                videoControlChoiceDropdown.AddOptions(VRDropOptions);
                gyroAvailable = true;
                // videoControlChoiceDropdown.value = getUserVideoChoice();
            }
            else
            {
                // Gyro not available
                // Disable VR option
                videoControlChoiceDropdown.ClearOptions();
                videoControlChoiceDropdown.AddOptions(nonVRDropOptions);
                gyroAvailable = false;
                // videoControlChoiceDropdown.value = getUserVideoChoice();
            }

            if (isUserPrefSet)
            {
                storedChoice = PlayerPrefs.GetInt("VR_CHOICE");
                Debug.Log("Stored Choice: " + storedChoice);
                videoControlChoiceDropdown.value = getUserVideoChoice();
            }
            // Set the dropdown options to reflect the choice
            videoControlChoiceDropdown.value = getUserVideoChoice();

        }
        catch (Exception e)
        {
            Debug.LogError("Error");
        }
    }

    // Check Download Progress
    public void OnCheckDownloadComplete(float downloadProgress)
    {
        if (downloadProgress < 100.0f)
        {
            progressBar.BarValue = (float)Math.Round(downloadProgress, 2);
            // Debug.Log("Downloading... " + downloadProgress);
        }
        else
        {
            // Debug.Log("Completed Downloading... " + downloadProgress);
            progressBar.BarValue = 100f;
            checkingContentDialog.SetActive(false);
        }
    }

    public void LoadVideo()
    {
        StartCoroutine(LoadVideoC("Role01Helen", (assetBundleRequest) =>
        {
            // VideoClip clip = Resources.Load<VideoClip>(videoName) as VideoClip;
            VideoClip clip = assetBundleRequest.asset as VideoClip;
            Debug.Log(clip.originalPath);
        }));
    }

    public IEnumerator LoadVideoAssetBundle()
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.persistentDataPath, "androidvideos"));
        yield return bundleLoadRequest;

        myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        Debug.Log("Done Loading Asset Bundle");
    }

    public void UnloadVideoAssetBundle()
    {
        myLoadedAssetBundle.Unload(true);
    }

    public IEnumerator LoadVideoC(string videoName, Action<AssetBundleRequest> callbackAssetBundle)
    {
        var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<VideoClip>(videoName);
        yield return assetLoadRequest;
        callbackAssetBundle(assetLoadRequest);
        // Debug.Log(assetLoadRequest.GetType());

        // VideoClip videoClip = assetLoadRequest.asset as VideoClip;
        // Debug.Log(videoClip);
        // vp.clip = videoClip;
        // vp.Play();

        // myLoadedAssetBundle.Unload(false);
    }

    // Check user video preference
    public int getUserVideoChoice()
    {
        Debug.Log("Stored Choice: " + storedChoice);

        Debug.Log("VR_MODE: " + PlayerPrefs.GetInt("VR_CHOICE"));

        if (!isUserPrefSet && gyroAvailable && storedChoice == 0)
        {
            // Select SWIPE as default
            PlayerPrefs.SetInt("VR_OR_SWIPE", 1);
            DebugText.text = "VR_MODE: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 1;
        }
        else if (storedChoice == 0 && gyroAvailable)
        {
            // VR option
            PlayerPrefs.SetInt("VR_OR_SWIPE", 0);
            DebugText.text = "VR_MODE: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 0;
        }
        else if (storedChoice == 0 && !gyroAvailable)
        {
            // Swipe option
            PlayerPrefs.SetInt("VR_OR_SWIPE", 1);
            DebugText.text = "VR_MODE1: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 0;
        }
        else if (storedChoice == 1 && gyroAvailable)
        {
            // Swipe action
            PlayerPrefs.SetInt("VR_OR_SWIPE", 1);
            DebugText.text = "VR_MODE2: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 1;
        }
        else if (storedChoice == 1 && !gyroAvailable)
        {
            // Swipe action
            PlayerPrefs.SetInt("VR_OR_SWIPE", 1);
            DebugText.text = "VR_MODE3: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 0;
        }
        else if (storedChoice == 2 && gyroAvailable)
        {
            // Gyroscope option
            PlayerPrefs.SetInt("VR_OR_SWIPE", 2);
            DebugText.text = "VR_MODE3: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 2;
        }
        else if (storedChoice == 2 && !gyroAvailable)
        {
            // Swipe action
            PlayerPrefs.SetInt("VR_OR_SWIPE", 1);
            DebugText.text = "VR_MODE3: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 0;
        }
        else
        {
            PlayerPrefs.SetInt("VR_OR_SWIPE", 0);
            DebugText.text = "VR_MODE: " + PlayerPrefs.GetInt("VR_OR_SWIPE");

            return 0;
        }

        DebugText.text = "GYROAVAILABLE: " + gyroAvailable + " ISUSERPREFSET: " + isUserPrefSet;

    }

    public int getMode()
    {
        int vrOrSwipe = PlayerPrefs.GetInt("VR_OR_SWIPE");
        // Debug.Log("VR OR SWIPE: " + vrOrSwipe);
        return vrOrSwipe;
    }

    public void videoControlChoice()
    {
        PlayerPrefs.SetInt("VR_CHOICE", videoControlChoiceDropdown.value);
        storedChoice = videoControlChoiceDropdown.value;
        getUserVideoChoice();
        Debug.Log(videoControlChoiceDropdown.value);
    }

    public void SwitchScene(string sceneName)
    {
        loadingContainer.SetActive(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void Logout()
    {
        loadingContainer.SetActive(true);
        PlayerPrefs.DeleteKey("AUTH_TOKEN");
        PlayerPrefs.DeleteKey("AUTH_IS_GUEST");
        PlayerPrefs.DeleteKey("AUTH_GUEST_TOKEN");
        SwitchScene("Login");
    }

    public void ClearAssetCache()
    {
        bool success = Caching.ClearCache();
        Debug.Log(success);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("AEDAR_WELCOME_SPLASH_SCREEN");
    }
}
