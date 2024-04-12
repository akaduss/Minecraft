using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHotbar : MonoBehaviour
{
    private void Start()
    {
        Player.OnActiveBlockTypeChanged += UpdateActiveBlockType;
    }

    void UpdateActiveBlockType(int i)
    {
        Debug.Log("Active block type changed");
        if(transform.GetChild(i) != null)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                transform.GetChild(j).localScale = Vector3.one;
            }
            transform.GetChild(i).localScale = Vector3.one * 1.5f;
        }   
    }   

}
