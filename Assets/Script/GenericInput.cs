using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenericInput : MonoBehaviour
{
    public string Name;
    public bool isBool;

    public string GetValue() {
        if(isBool) {
            return gameObject.GetComponentInChildren<Toggle>().isOn.ToString();
        } else {
            return gameObject.GetComponentsInChildren<TextMeshProUGUI>()[2].text;
        }
    }
}
