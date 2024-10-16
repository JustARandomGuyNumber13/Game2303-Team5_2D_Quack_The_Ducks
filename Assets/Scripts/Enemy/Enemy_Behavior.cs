using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Behavior : MonoBehaviour
{
    Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void DeactivateEnemy()
    { 
        this.gameObject.SetActive(false);
    }
    public void ReactivateEnemy()
    {
        this.gameObject.SetActive(true);
    }
    public void SetNewPosition(Vector2 newPos, Vector3 newScaleX)
    { 
        _transform.position = newPos;
        _transform.localScale = newScaleX;
    }
}
