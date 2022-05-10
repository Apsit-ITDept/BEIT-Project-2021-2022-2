using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.VR;
using UnityEngine.Video;

public class HistoryManager : MonoBehaviour
{
    public MainMenuController _mainMenuController;
    public GameObject vrCamera, swipeCamera;
 
    public GameObject VideoSphere;
    public GameObject SelectionSphere;
    public GameObject twoDUI;
    public GameObject vrUI;

    private bool isVideoPlaying = false;

    public VideoPlayer videoPlayer;
    public GameObject selectButtonGO;
    public GameObject exitButtonGO;
    public GameObject moreInfoTextGO;

    public string historyVideoName;
    public ClientAPI clientAPI;

    void OnEnable() {
        // Load asset bundle
        StartCoroutine(_mainMenuController.LoadVideoAssetBundle());
    }

    void OnDisable() {
        _mainMenuController.UnloadVideoAssetBundle();
    }

    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs.SetInt("VR_OR_SWIPE", 0); // Comment this when building
        if(_mainMenuController.getMode() == 0) {
            // VR Mode
            StartCoroutine(SwitchToVR("cardboard"));
        } else {
            // Swipe Mode
            StartCoroutine(SwitchToVR("None"));
        }
    }

    IEnumerator SwitchToVR(string deviceName) {
        string[] Devices = new string[] { "daydream", "cardboard" };
        UnityEngine.XR.XRSettings.LoadDeviceByName(deviceName);

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return new WaitForSeconds(0.1f);
        
        // Now it's ok to enable VR mode.
        if(deviceName == "cardboard")
        {
            swipeCamera.SetActive(false);
            vrCamera.SetActive(true);
            UnityEngine.XR.XRSettings.enabled = true;
            twoDUI.SetActive(false);
            vrUI.SetActive(true);

            // gvrEventSystem.GetComponent<StandaloneInputModule>().enabled = false;
            // gvrEventSystem.GetComponent<BaseInput>().enabled = false;
            // gvrEventSystem.GetComponent<GvrPointerInputModule>().enabled = true;
            // gvrEventSystem.GetComponent<EventSystem>().enabled = false;
            // gvrEventSystem.GetComponent<EventSystem>().enabled = true;

        } else {
            vrCamera.SetActive(false);
            swipeCamera.SetActive(true);
            UnityEngine.XR.XRSettings.enabled = false;
            // Enable 2d UI
            twoDUI.SetActive(true);
            vrUI.SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void PlayHistoryVideo() {
        isVideoPlaying = true;

        StartCoroutine(_mainMenuController.LoadVideoC(historyVideoName, (assetBundleRequest) => {
            // VideoClip clip = Resources.Load<VideoClip>(historyVideoName) as VideoClip;
            VideoClip clip = assetBundleRequest.asset as VideoClip;
            videoPlayer.clip = clip;
            VideoSphere.SetActive(true);

            SelectionSphere.SetActive(false);

            if(_mainMenuController.getMode() == 0) {
                // VR Mode
                vrUI.SetActive(false);
            } else {
                // Swipe Mode
                selectButtonGO.SetActive(false);
                exitButtonGO.SetActive(false);
                moreInfoTextGO.SetActive(false);
            }

            videoPlayer.Play();
            videoPlayer.loopPointReached += videoPlaybackEnded;
        }));
        
    }

    void videoPlaybackEnded(VideoPlayer vp)
    {   
        submitHistoryData();
        isVideoPlaying = false;
        VideoSphere.SetActive(false);
        SelectionSphere.SetActive(true);

        if(_mainMenuController.getMode() == 0) {
            // VR Mode
            vrUI.SetActive(true);
        } else {
            selectButtonGO.SetActive(true);
            exitButtonGO.SetActive(true);
            moreInfoTextGO.SetActive(true);
        }
    }

    public void submitHistoryData() {
        _mainMenuController.loadingContainer.SetActive(true);

        OtherInfoData historyData = new OtherInfoData();
        historyData.data = "history";

        // Call API
        StartCoroutine(clientAPI.SubmitOtherInfo(historyData, OnInfoSubmitted));
    }

    public void OnInfoSubmitted(AuthToken authToken) {
        if(authToken.status == "success") {
            Debug.Log(authToken.token);
            _mainMenuController.loadingContainer.SetActive(true);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main Menu");
        } else {
            _mainMenuController.loadingContainer.SetActive(true);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Login");
        }
    }

    public void exitingFromVR() {
        UnityEngine.XR.XRSettings.enabled = false;
    }
}
