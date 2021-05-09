using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChanger : MonoBehaviour
{
    public int Value;
    private ARObjectPlacement Placement;
    public void ChangeValue(bool ifselected)
    {
        if (ifselected)
        Placement.CurrentObject = Value;
    }

    // Start is called before the first frame update
    void Start()
    {
        Placement = FindObjectOfType<ARObjectPlacement>(); //Selects the object placement object

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
