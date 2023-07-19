using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UI_Behaviour : MonoBehaviour
{
    [SerializeField] private FloatSO lvl;
    [SerializeField] private int[] activeOnLvls;
    [SerializeField] private GameObject[] objectsToActive;

    [SerializeField] private Text[] textChangeColor;
    [SerializeField] private Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < activeOnLvls.Length; i++)
        {
            if (lvl.Value == activeOnLvls[i])
            {
                for (int j = 0; j < objectsToActive.Length; j++)
                {
                    objectsToActive[j].SetActive(true);
                }
            }
        }

        if (lvl.Value == 2)
        {
            for (int i = 0; i < textChangeColor.Length; i++)
            {
                textChangeColor[i].color = Color.white;
            }
        }
        else
        {
            for (int i = 0; i < textChangeColor.Length; i++)
            {
                textChangeColor[i].color = defaultColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}