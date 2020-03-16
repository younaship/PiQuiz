using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour
{
    public RawImage imageA, imageB; // L R
    public Texture[] sprites;

    Vector3 basePos;
    // Start is called before the first frame update
    void Start()
    {
        basePos = imageB.rectTransform.position;
    }

    bool isA = true;
    int i = 0;
    void Change()
    {
        if (isA) imageA.texture = sprites[i];
        else imageB.texture = sprites[i];
        isA = !isA;
        i++;
        if (i >= sprites.Length) i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
