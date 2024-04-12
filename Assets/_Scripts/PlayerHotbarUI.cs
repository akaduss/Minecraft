using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHotbarUI : MonoBehaviour
{
    private void Start()
    {
        Signals.OnActiveBlockTypeChanged += UpdateActiveBlockType;
    }

    void UpdateActiveBlockType(int index)
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        }
        transform.GetChild(index).GetComponent<RectTransform>().localScale = new Vector2(1.5f, 1.5f);
    }
}
