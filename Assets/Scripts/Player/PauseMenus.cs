using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenus : MonoBehaviour
{
    public AudioClips sfx;
    public Canvas map;
    public Canvas pause;
    public PlayerMove player;

    private void Update()
    {
        Map();
        PauseGame();
    }
    void Map()
    {
        if (map != null)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if (!map.gameObject.activeSelf)
                {
                    sfx.PlayOneShot("Map");
                    map.gameObject.SetActive(true);
                }
                else
                {
                    sfx.PlayOneShot("Map");
                    map.gameObject.SetActive(false);
                }
            }
        }
    }

    void PauseGame()
    {
        if (pause != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!pause.gameObject.activeSelf)
                {
                    pause.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    sfx.PlayOneShot("Menu");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    player.GetComponent<MouseLook>().enabled = false;
                }
                else
                {
                    pause.gameObject.SetActive(false);
                    Time.timeScale = 1f;
                    sfx.PlayOneShot("Menu");
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    player.GetComponent<MouseLook>().enabled = true;
                }
            }
        }
    }


}
