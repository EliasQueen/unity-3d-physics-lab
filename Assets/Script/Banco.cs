using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class Banco
{
    public static List<Cena> GetMenuExperimentos(bool skipLast = false)
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "experimentos.json");

        if (!File.Exists(jsonFilePath))
        {
            throw new Exception("Arquivo não encontrado.");
        }

        string text = File.ReadAllText(jsonFilePath);
        MenuExperimentos menuExperimentos = JsonConvert.DeserializeObject<MenuExperimentos>(text);

        if (skipLast && menuExperimentos.Cenas.Count > 0)
        {
            return menuExperimentos.Cenas.Take(menuExperimentos.Cenas.Count - 1).ToList();
        }

        return menuExperimentos.Cenas;
    }

    public static Cena GetCenaById(int id)
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "experimentos.json");

        if (!File.Exists(jsonFilePath))
        {
            throw new Exception("Arquivo não encontrado.");
        }

        string text = File.ReadAllText(jsonFilePath);
        MenuExperimentos menuExperimentos = JsonConvert.DeserializeObject<MenuExperimentos>(text);

        foreach (Cena cena in menuExperimentos.Cenas)
        {
            if(cena.Id == id)
                return cena;
        }

        throw new Exception("Cena não encontrada.");
    }
}

public class MenuExperimentos
{
    public List<Cena> Cenas { get; set; }
}

public class Cena
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public List<Parameter> Parameters { get; set; }
}

public enum ParameterType
{
    Boolean,
    Double,
    Float,
    Integer
}

public class Parameter
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public ParameterType Type { get; set; }
    public string Default { get; set; }
}

public class EnvironmentLoader : MonoBehaviour
{
    private static EnvironmentLoader instance;

    public Dictionary<string, object> dados = new Dictionary<string, object>();

    public static EnvironmentLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EnvironmentLoader>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EnvironmentLoader");
                    instance = obj.AddComponent<EnvironmentLoader>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}