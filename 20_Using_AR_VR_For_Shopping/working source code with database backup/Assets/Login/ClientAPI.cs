using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.Networking;
// using UnityEngine.Video;

[Serializable]
public class Product
{
    public string _id;
    public string product_name;
    public string description;
    public string price;
}

[Serializable]
public class Products
{
    public string status;
    public List<Product> products;
}

public class ClientAPI : MonoBehaviour
{
    public string loginURL;
    public string registerURL;
    public string checkUserURL;
    public string submitScenarioURL;
    public string submitOtherInfoURL;
    public string androidAssetsURL;
    public string iosAssetsURL;
    public string allProductsURL;
    public string placeOrderURL;

    public uint androidCRC;
    public uint iosCRC;

    public string token;

    // public VideoPlayer vp;

    void Start()
    {
        token = PlayerPrefs.GetString("AUTH_TOKEN");
        Debug.Log(token);
    }

    public IEnumerator Login(Login login, System.Action<AuthToken> loginCallback)
    {
        var jsonData = JsonUtility.ToJson(login);
        Debug.Log(jsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(loginURL, jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    // result = "{\"result\":" + result + "}";
                    var authToken = JsonUtility.FromJson<AuthToken>(result);

                    loginCallback(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }

    // Register
    public IEnumerator Register(Register register, System.Action<AuthToken> loginCallback)
    {
        var jsonData = JsonUtility.ToJson(register);
        Debug.Log(jsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(registerURL, jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    // result = "{\"result\":" + result + "}";
                    var authToken = JsonUtility.FromJson<AuthToken>(result);

                    loginCallback(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }

    // Get all products
    public IEnumerator GetAllProducts(System.Action<Products> getAllProductsCompleted)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(allProductsURL))
        {
            www.SetRequestHeader("content-type", "application/json");

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    // result = "{\"result\":" + result + "}";
                    var authToken = JsonUtility.FromJson<Products>(result);

                    getAllProductsCompleted(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }

    public IEnumerator CheckUser(string userToken, System.Action<AuthToken> checkUserCallback)
    {
        Debug.Log(userToken);

        using (UnityWebRequest www = UnityWebRequest.Get(checkUserURL))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.SetRequestHeader("Authorization", userToken);

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    var authToken = JsonUtility.FromJson<AuthToken>(result);

                    checkUserCallback(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }

    public IEnumerator SubmitScenario(RoomData roomData, System.Action<AuthToken> submitScenarioCallback, string userToken)
    {
        string jsonData = JsonUtility.ToJson(roomData);
        Debug.Log(jsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(submitScenarioURL, jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            if (userToken != "")
            {
                www.SetRequestHeader("Authorization", userToken);
            }

            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    // result = "{\"result\":" + result + "}";
                    // Debug.LogWarning(result);

                    var authToken = JsonUtility.FromJson<AuthToken>(result);

                    submitScenarioCallback(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }

    public IEnumerator SubmitOtherInfo(OtherInfoData otherInfoData, System.Action<AuthToken> submitOtherInfoCallback)
    {
        string jsonData = JsonUtility.ToJson(otherInfoData);
        Debug.Log(jsonData);
        using (UnityWebRequest www = UnityWebRequest.Post(submitOtherInfoURL, jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.SetRequestHeader("Authorization", token);
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    var authToken = JsonUtility.FromJson<AuthToken>(result);

                    submitOtherInfoCallback(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }

    public IEnumerator DownloadAssets(string Platform, System.Action<float> downloadCallback)
    {
        Debug.Log("Downloading for Platform: " + Platform);

        string assetDownloadURL;
        float downloadDataProgress;
        uint crc;

        if (Platform == "android")
        {
            assetDownloadURL = androidAssetsURL;
            crc = androidCRC;
        }
        else
        {
            assetDownloadURL = iosAssetsURL;
            crc = iosCRC;
        }

        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetDownloadURL))
        {
            var resultFile = Path.Combine(Application.persistentDataPath, "androidvideos");
            var dh = new DownloadHandlerFile(resultFile);
            dh.removeFileOnAbort = true;
            www.downloadHandler = dh;

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();

            if (operation.webRequest.isNetworkError || operation.webRequest.isHttpError)
            {
                Debug.Log(operation.webRequest.error);
            }
            else
            {
                Debug.Log(operation.isDone);
                while (!operation.isDone)
                {
                    downloadDataProgress = operation.webRequest.downloadProgress * 100.0f;
                    Debug.Log(downloadDataProgress);
                    downloadCallback(downloadDataProgress);
                    yield return null;
                }

                downloadCallback(100f);
                // AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                // var assetLoadRequest = bundle.LoadAssetAsync("Role01Helen", typeof(VideoClip));
                // yield return assetLoadRequest;

                // var getAllAssetNames = bundle.GetAllAssetNames();
                // yield return getAllAssetNames;

                // Debug.Log(getAllAssetNames);
                // GameObject obj = assetLoadRequest.asset as GameObject;

                // VideoClip videoClip =  assetLoadRequest.asset as VideoClip;
                // vp.clip = videoClip;
                // vp.Play();
                Debug.Log("Done");
            }
        }
    }

    public IEnumerator PlaceOrder(Order otherInfoData, System.Action<AuthToken> submitOtherInfoCallback)
    {
        string jsonData = JsonUtility.ToJson(otherInfoData);
        Debug.Log(jsonData);
        using (UnityWebRequest www = UnityWebRequest.Post(placeOrderURL, jsonData))
        {
            www.SetRequestHeader("content-type", "application/json");
            www.SetRequestHeader("Authorization", token);
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    var authToken = JsonUtility.FromJson<AuthToken>(result);

                    submitOtherInfoCallback(authToken);
                }
                else
                {
                    Debug.Log("Error!");
                }
            }
        }
    }
}
