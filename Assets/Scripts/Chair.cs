using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dir
{
    left,
    right,
    down,
}
public class Chair : MonoBehaviour
{
    public int empty;
    public Dir dir;
    private void Start()
    {
        empty = 0;
    }
}
