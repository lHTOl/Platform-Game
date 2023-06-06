using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterScript Controller;
    private bool InputEnabled;

    private void Awake()
    {
        Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterScript>();
        InputEnabled = true;
    }
    private void Start()
    {
        Controller.InputStartOrStop += Controller_InputStartOrStop;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InputEnabled)
            switch (name)
            {
                case "Right":
                    Controller.SetMoveInput(1f);
                    break;

                case "Left":
                    Controller.SetMoveInput(-1f);
                    break;
            }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Controller.SetMoveInput(0f);
    }

    private void Controller_InputStartOrStop(object sender, CharacterScript.InputBool e)
    {
        Controller.SetMoveInput(0f);
        InputEnabled = e.InputEnabled;
    }
}
