using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private List<Cena> cenas;
    public List<GameObject> Buttons;
    public GameObject Left;
    public GameObject Right;

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cenas = Banco.GetMenuExperimentos(true);

        if(cenas.Count <= 4)
        {
            Left.SetActive(false);
            Right.SetActive(false);
        }

        ListAndAssignButtons();
    }

    void ListAndAssignButtons()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            if(cenas.Count <= i)
            {
                Buttons[i].SetActive(false);
                continue;
            }

            // Encontre a componente Text no filho do botão
            TextMeshProUGUI buttonText = Buttons[i].GetComponentInChildren<TextMeshProUGUI>();

            // Verifique se a componente Text foi encontrada
            if (buttonText != null)
            {
                buttonText.text = cenas[i].Title;
            }
            else
            {
                Debug.LogError("Componente Text não encontrado no filho do botão.");
            }

            // Use uma variável local para evitar o closure de variáveis
            int index = i;
            Buttons[i].GetComponent<Button>().onClick.AddListener(() => Redirect(cenas[index].Id));
        }
    }

    void Redirect(int sceneId) {
        PlayerPrefs.SetInt("sceneId", sceneId);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MenuDescription", LoadSceneMode.Single);
    }
}
