using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinColliderBehaviour : MonoBehaviour
{
    public int multiplier;

    // Start is called before the first frame update
    void Start()
    {
        multiplier = Random.Range(1, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
