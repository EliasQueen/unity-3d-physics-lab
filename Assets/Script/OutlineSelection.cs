using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{
    public float InteractionRange = 5.0f;
    private Transform highlight;
    private RaycastHit raycastHit;
    private GameObject interactiveObject;

    public Player Player;

    void Start() {
        Player = GetComponentInParent<Player>();
    }

    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit, InteractionRange)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            interactiveObject = raycastHit.collider.gameObject;
            if (highlight.CompareTag("Selectable"))
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.black;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 10.0f;
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                InteractionBehaviourBase interactionBehaviourBase = interactiveObject.GetComponent<InteractionBehaviourBase>();
                interactionBehaviourBase.Interagir();
                // Player.Teste();
            }
        }
    }

}
