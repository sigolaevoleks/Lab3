using UnityEngine;
using UnityEngine.UI;

public class CellData : MonoBehaviour
{
    public int setID; // Required for Eller's Algorithm 
    public GameObject rightWall; // Reference to your RightWall child 
    public GameObject bottomWall; // Reference to your BottomWall child 

    // Helper to turn walls on/off
    public void SetRightWall(bool active) => rightWall.SetActive(active);
    public void SetBottomWall(bool active) => bottomWall.SetActive(active);
}