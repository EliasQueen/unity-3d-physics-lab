using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBehaviourBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interagir()
    {
        // Lógica de interação aqui
        Debug.Log("Interagindo com: " + gameObject.name + " às " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
    }
}
