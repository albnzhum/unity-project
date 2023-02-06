/** © 2018 NULLcode Studio. All Rights Reserved. https://null-code.ru/ **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField] private float smooth = 3.5f;
    [SerializeField] private Transform player;

    void LateUpdate ()
    {
        if (smooth <= 0)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), smooth * Time.deltaTime);
        }
	}
}
