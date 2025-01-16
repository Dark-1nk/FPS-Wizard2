using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [System.Serializable]
    public class DoorEntry
    {
        public GameObject door;
        public bool isRed;
        public bool isGreen;
        public bool isPink;
    }

    public List<DoorEntry> doors;

    public void OpenMe(string color)
    {
        foreach (DoorEntry entry in doors)
        {
            if ((color == "Red" && entry.isRed) ||
                (color == "Pink" && entry.isPink) ||
                (color == "Green" && entry.isGreen))
            {
                if (entry.door != null) // Ensure the door object is assigned
                {
                    entry.door.SetActive(false); // Disable the specific door object
                }
                else
                {
                    Debug.LogWarning($"Door is not assigned in entry for color: {color}");
                }
            }
        }
    }
}
