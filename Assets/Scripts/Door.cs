using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int GroupId = 1;

    bool DefaultOpen = false;

    public bool isOpen {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        // TODO: tint sprite so that matching GroupIds are the same color
        isOpen = DefaultOpen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set(bool state) {
        isOpen = state != DefaultOpen;
    }
}
