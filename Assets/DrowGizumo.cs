using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrowGizumo : MonoBehaviour
{
    public ScOb_KeyData scri__obj;

    void OnDrawGizmos()
    {
        #region --[Color_Declarations]--
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        #endregion

        if (scri__obj.is_inArea) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y - scri__obj.groundedOffset, transform.position.z), new Vector3(0.4f * 8 * transform.localScale.x, 3, 0.4f * 8 * transform.localScale.z));
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("T_Red"))
        {
            scri__obj.is_inArea = true;
            Debug.Log("now_PlayerIn");
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("T_Red"))
        {
            scri__obj.is_inArea = false;
            Debug.Log("now_PlayerOut");
        }

    }
}
