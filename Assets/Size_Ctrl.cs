using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Size_Ctrl : MonoBehaviour
{
    private int nowScaleCoun;
    public ScOb_KeyData scri__obj;

    // Start is called before the first frame update
    void Start()
    {
        scri__obj.is_inArea = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((scri__obj.is_inArea)&&(nowScaleCoun < scri__obj.maxScaleCoun))
        {
            nowScaleCoun += 1;
            transform.localScale += new Vector3(scri__obj.scalePerFra, 0, scri__obj.scalePerFra);
        }
    }
}
