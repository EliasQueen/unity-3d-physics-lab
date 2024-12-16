using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CubicPieBehaviour : InteractionBehaviourBase
{
    [Header("Objetos")]
    public Cubo1 Cubo1;
    public Cubo2 Cubo2;

    [Header("Propriedades")]
    public int Precision;

    [Header("Contador")]
    public TextMeshProUGUI Contador;
    public TextMeshProUGUI PI;

    void Start()
    {
        PropertiesHandler();
    }

    void PropertiesHandler()
    {
        Precision = PlayerPrefs.HasKey("decimal_places") ? int.Parse(PlayerPrefs.GetString("decimal_places")) : 1;
    }

    public override void Interagir()
    {
        Cubo1.collisionCounter = 0;
        Cubo1.StartExperiment();
        Cubo2.StartExperiment();
    }
}
