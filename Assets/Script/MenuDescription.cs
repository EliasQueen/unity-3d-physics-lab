using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuDescription : MonoBehaviour
{
    public GameObject InputBoolean;
    public GameObject InputDouble;
    public GameObject InputFloat;
    public GameObject InputInteger;

    public RawImage Icon;

    private Cena scene;
    private TextMeshProUGUI titulo;
    private TextMeshProUGUI descricao;
    private GameObject parametros;
    private GameObject button;

    void Start() {
        int sceneId = PlayerPrefs.HasKey("sceneId") ? PlayerPrefs.GetInt("sceneId") : -1;
        
        scene = Banco.GetCenaById(sceneId);
        titulo = GameObject.FindGameObjectWithTag("Title").GetComponent<TextMeshProUGUI>();
        descricao = GameObject.FindGameObjectWithTag("Description").GetComponent<TextMeshProUGUI>();
        parametros = GameObject.FindGameObjectWithTag("Parameters");
        button = GameObject.FindGameObjectsWithTag("Button")[0];

        MenuHandler();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }

    void MenuHandler() {
        PlayerPrefs.DeleteAll();

        titulo.text = scene.Title;
        descricao.text = scene.Description;

        IconHandler();
        ParameterHandler();

        button.GetComponent<Button>().onClick.AddListener(() => HandleRedirect(scene.Name));
    }

    void IconHandler()
    {
        if (string.IsNullOrEmpty(scene?.Image))
        {
            Debug.LogError("Nome da imagem não está definido.");
            return;
        }
        
        try
        {
            Texture2D texture = GetTexture(scene.Image);
            Icon.texture = texture;
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"Arquivo de textura não encontrado: {scene.Image}\n{ex}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao carregar a textura: {ex}");
        }
    }

    Texture2D GetTexture(string imageName)
    {
        UnityEngine.Object[] textures = Resources.LoadAll("experimentos", typeof(Texture2D));

        foreach (UnityEngine.Object texture in textures)
        {
            if (texture.name.Equals(imageName, StringComparison.OrdinalIgnoreCase))
            {
                return (Texture2D) texture;
            }
        }

        throw new FileNotFoundException();
    }

    void ParameterHandler() {
        List<Parameter> parameters = scene.Parameters;

        if (parameters == null || parameters.Count == 0) {
            Debug.Log("Parâmetros não encontrados.");
            return;
        }

        foreach (Parameter param in parameters) {
            GameObject prefabToInstantiate = null;

            switch (param.Type) {
                case ParameterType.Boolean:
                    prefabToInstantiate = InputBoolean;
                    break;
                case ParameterType.Double:
                    prefabToInstantiate = InputDouble;
                    break;
                case ParameterType.Float:
                    prefabToInstantiate = InputFloat;
                    break;
                case ParameterType.Integer:
                    prefabToInstantiate = InputInteger;
                    break;
            }

            GameObject parameter = Instantiate(prefabToInstantiate);
            parameter.transform.SetParent(parametros.transform, false);
            parameter.GetComponent<GenericInput>().Name = param.Name;
            parameter.GetComponentsInChildren<TextMeshProUGUI>()[0].text = param.DisplayName;

            if(param.Type == ParameterType.Boolean) {
                parameter.GetComponentInChildren<Toggle>().isOn = Boolean.Parse(param.Default);
            }

            parameter.SetActive(true);
        }
    }

    void HandleRedirect(string sceneName) {
        SaveParametersToPlayerPrefs();
        Redirect(sceneName);
    }

    void SaveParametersToPlayerPrefs() {
        foreach (Transform child in parametros.transform) {
            var genericInput = child.GetComponent<GenericInput>();
            if (genericInput != null) {
                string paramName = genericInput.Name;
                string paramValue = RemoveInvisibleCharacters(genericInput.GetValue());

                if(paramValue == String.Empty)
                    continue;

                PlayerPrefs.SetString(paramName, paramValue);
            }
        }
        PlayerPrefs.Save();
    }

    void Redirect(string sceneName) {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    string RemoveInvisibleCharacters(string input) {
        // Define a regex para remover caracteres invisíveis
        string pattern = @"[ \t\n\r\u2423\u200B\uFEFF\u200C\u200D\u00AD\u2423\u2009\u00B7\u202F\u1680\u2003\u2002\u0085\u25A1\u25A3\u3000]";
        return Regex.Replace(input, pattern, string.Empty);
    }
}
