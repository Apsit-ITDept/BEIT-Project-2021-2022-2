using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardboardNumericKeyboard : MonoBehaviour
{
    public TMP_Text currentText;

    public void KeyPressed(string key) {
        currentText.text += key;
    }
}
