using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    [SerializeField] private Player_Behavior[] _playersInThisScene;
    [SerializeField] private Scene_Manager _sceneManager;

    private Transform _transform;


    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public void PlayerDieEvent(Player_Behavior player)
    {
        bool isGameOver = true;
        foreach (Player_Behavior p in _playersInThisScene)
            if (p._health > 0)
                isGameOver = false;

        if (isGameOver)
        {
            Invoke("GoToMainMenuScene", 3f);
        }
        else
        {
            StartCoroutine(RespawnPlayerCoroutine(player));
        }
    }

    private IEnumerator RespawnPlayerCoroutine(Player_Behavior p)
    {
        yield return new WaitForSeconds(2f);
        if (p._health > 0)
        {
            p.transform.position = _transform.position;
            p.ReactivatePlayer();
        }
    }
    private void GoToMainMenuScene()
    {
        _sceneManager.GoToScene(0);
    }
}
