using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public PointType pointType;
    public List<Point> list;
    public string ground;
    [SerializeField]
    public string ladder;

  
}
public enum PointType
{
    NoramlPoint,
    LadderPoint,
    JumpPoint
}