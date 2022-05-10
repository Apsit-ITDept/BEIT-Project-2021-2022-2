using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;
// using UnityEngine.XR.Management;

public class LoginViewController : MonoBehaviour
{
    public InputField emailField, registerEmailField, registerNameField;
    public InputField passwordField, registerPasswordField;
    public Text errorText;
    public GameObject loadingContainer;
    public ClientAPI clientAPI;

    public LeanWindow errorModal;
    public GameObject loginContainer, registerContainer;

    void Start()
    {
        // StopXR();
        // Auto Login if token is present
        // PlayerPrefs.DeleteKey("AUTH_TOKEN");
        string userToken = PlayerPrefs.GetString("AUTH_TOKEN", null);

        Debug.LogError("userToken: " + userToken);
        if (userToken == "")
        {
            // No auth Token found
            loadingContainer.SetActive(false);
            Debug.LogError("No auth found");
        }
        else if (userToken != "")
        {
            // Auth token found
            // Check with server
            StartCoroutine(clientAPI.CheckUser(userToken, OnCheckUserRequestComplete));
        }
    }

    // Login
    public void Login()
    {
        // Get email and password
        string email = emailField.text;
        string password = passwordField.text;
        Debug.Log(email);
        Debug.Log(password);

        Login loginData = new Login()
        {
            email = email,
            password = password
        };

        loadingContainer.SetActive(true);

        StartCoroutine(clientAPI.Login(loginData, OnLoginRequestComplete));
    }

    public void OnLoginRequestComplete(AuthToken authToken)
    {
        loadingContainer.SetActive(false);
        if (authToken.token != null)
        {
            Debug.Log(authToken.token);
            PlayerPrefs.SetString("AUTH_TOKEN", authToken.token);
            PlayerPrefs.SetString("AUTH_EMAIL", authToken.email);

            loadingContainer.SetActive(true);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main Menu");
        }
        else
        {
            // Show error modal
            string error = "<size=60>Oops!</size> \n \n" + authToken.error;
            //Show modal here
            errorText.text = error;
            errorModal.Toggle();
            Debug.Log(authToken.error);
        }
    }

    public void showLoginContainer()
    {
        loginContainer.SetActive(true);
        registerContainer.SetActive(false);
    }

    // Register
    public void Register()
    {
        // Get email and password
        string email = registerEmailField.text;
        string password = registerPasswordField.text;
        string name = registerNameField.text;
        Debug.Log(email);
        Debug.Log(password);

        Register registerData = new Register()
        {
            email = email,
            password = password,
            full_name = name
        };

        loadingContainer.SetActive(true);

        StartCoroutine(clientAPI.Register(registerData, OnRegisterRequestComplete));
    }

    public void OnRegisterRequestComplete(AuthToken authToken)
    {
        loadingContainer.SetActive(false);
        if (string.Equals(authToken.status, "success"))
        {
            loadingContainer.SetActive(false);
        }
        else
        {
            // Show error modal
            string error = "<size=60>Oops!</size> \n \n" + authToken.error;
            //Show modal here
            errorText.text = error;
            errorModal.Toggle();
            Debug.Log(authToken.error);
        }
    }
    public void showRegisterContainer()
    {
        loginContainer.SetActive(false);
        registerContainer.SetActive(true);
    }
    public void OnCheckUserRequestComplete(AuthToken authToken)
    {
        if (authToken.error == null)
        {
            Debug.Log(authToken.token);
            loadingContainer.SetActive(true);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main Menu");
        }
        else
        {
            loadingContainer.SetActive(false);
        }
    }

    // Login as Guest
    public void LoginAsGuest()
    {
        string deviceIdentifier = SystemInfo.deviceUniqueIdentifier;
        PlayerPrefs.SetString("AUTH_IS_GUEST", "true");
        PlayerPrefs.SetString("AUTH_GUEST_TOKEN", deviceIdentifier);
        loadingContainer.SetActive(true);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main Menu");
    }

    void StopXR()
    {
        // var xrManager = XRGeneralSettings.Instance.Manager;
        // if (!xrManager.isInitializationComplete)
        //     return; // Safety check
        // xrManager.StopSubsystems();
        // xrManager.DeinitializeLoader();
    }
}
