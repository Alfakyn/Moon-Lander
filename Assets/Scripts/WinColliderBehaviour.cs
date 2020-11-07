using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinColliderBehaviour : MonoBehaviour
{
    public TextMeshPro score_multiplier;

    private int multiplier;
    private GameObject earth_win_collider;

    // Start is called before the first frame update
    void Start()
    {
        earth_win_collider = GameObject.Find("/WinColliders/Square9_1_earth/Size9_1");
        initializeMultipliers();
    }

    public void initializeMultipliers()
    {
        multiplier = Random.Range(1, 4) + Random.Range(1, 4);
        earth_win_collider.GetComponent<WinColliderBehaviour>().multiplier = 20;
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
