using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAccessFrom : MonoBehaviour
{
    const string PATH = "MyAccessForm/MyAccessFromCanvas";

    private void Awake()
    {
        Instantiate(Resources.Load(PATH) as GameObject);
    }
}