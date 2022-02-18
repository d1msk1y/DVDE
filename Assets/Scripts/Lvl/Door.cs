using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float doorOpenSpeed;
    private bool _isOpen;

    private Vector3 _doorVelocity;
    private void Update()
    {
        //---Debug---//
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(CloseTheDoor());
        if (Input.GetKeyDown(KeyCode.G))
            StartCoroutine(OpenTheDoor());
        //---Debug---//

    }

    public IEnumerator CloseTheDoor()
    {
        if (!_isOpen)
            yield return null;
        _isOpen = false;
        while (transform.localScale.y < 1 && !_isOpen)
        {
            transform.localScale += new Vector3(1, 1, 1) * doorOpenSpeed/2;
            yield return null;
        }
    }

    public IEnumerator OpenTheDoor()
    {
        if (_isOpen)
            yield return null;
        _isOpen = true;
        while (transform.localScale.y > 0 && _isOpen)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref _doorVelocity ,doorOpenSpeed);
            yield return null;
        }
    }

}
