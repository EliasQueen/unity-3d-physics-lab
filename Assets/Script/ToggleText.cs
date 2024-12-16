using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ToggleText : MonoBehaviour
{
    public void ChangeText(Boolean toggleState)
    {
        gameObject.GetComponent<Text>().text = toggleState ? "Ativado" : "Desativado";
    }
}
