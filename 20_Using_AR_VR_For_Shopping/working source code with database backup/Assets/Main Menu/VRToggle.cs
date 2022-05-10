using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRToggle : MonoBehaviour
{
    public bool sceneIsVR = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(VRToggleI());
    }

    IEnumerator VRToggleI() {
        if (sceneIsVR)
        {
            XRSettings.LoadDeviceByName("cardboard");
            yield return null;
            XRSettings.enabled = true;
        } else {
            XRSettings.LoadDeviceByName("None");
            yield return null;
            XRSettings.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
