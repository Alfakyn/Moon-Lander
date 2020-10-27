using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinColliderBehaviour : MonoBehaviour
{
    public TextMeshPro score_multiplier;

    private int multiplier;

    // Start is called before the first frame update
    void Start()
    {
        initializeMultipliers();
    }

    public void initializeMultipliers()
    {
        multiplier = Random.Range(1, 4) + Random.Range(1, 4);
        score_multiplier.text = multiplier.ToString() + "x";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getMultiplier()
    {
        return multiplier;
    }
}
