using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameBillboard : MonoBehaviour
{
    Camera maincam;
    private void Update()
    {
        if(maincam == null)
        {
            maincam = FindObjectOfType<Camera>();

        }

        if (maincam == null) {
            return;
        }
        transform.LookAt(maincam.transform.position);
        transform.Rotate(Vector3.up * 180);
    }
}
