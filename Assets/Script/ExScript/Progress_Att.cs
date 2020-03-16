using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress_Att : MonoBehaviour
{
    Image image;
    private void Awake()
    {
        image = this.GetComponent<Image>();
    }
    private void Update()
    {
        transform.Rotate(Vector3.forward, -10);
    }
}
