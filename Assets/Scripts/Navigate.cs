using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigate
{
    private Dictionary<string, List<Point>> groundMap;
    private Dictionary<string, List<Point>> ladderMap;
    private int pointNum = 0;

    // Start is called before the first frame update
    public Navigate()
    {
        groundMap = new Dictionary<string, List<Point>>();
        ladderMap = new Dictionary<string, List<Point>>();

        GameObject grounds = GameObject.Find("Grid/Grounds");
        if(grounds.transform.childCount > 0)
        {
            foreach (Transform groundTrans in grounds.transform)
            {
                groundMap[groundTrans.name] = new List<Point>();
                foreach(Transform pointTrans in groundTrans)
                {
                    Point point = pointTrans.GetComponent<Point>();
                    point.ground = groundTrans.name;
                    groundMap[groundTrans.name].Add(point);
                    pointNum++;
                }
            }
        }

        GameObject ladders = GameObject.Find("Grid/Ladders");
        if(ladders.transform.childCount > 0)
        {
            foreach (Transform ladderTrans in ladders.transform)
            {
                ladderMap[ladderTrans.name] = new List<Point>();
                foreach (Transform pointTrans in ladderTrans)
                {
                    Point point = pointTrans.GetComponent<Point>();
                    point.ladder = ladderTrans.name;
                    ladderMap[ladderTrans.name].Add(point);
                    groundMap[point.ground].Add(point);
                    pointNum++;
                }
            }
        }
    }

    // Update is called once per frame
    public Point[] GetPoints(Point from, Point to)
    {
        if ((from.ground == null) || (to.ground == null && to.ladder == null)) return null;

        if (from.ground == to.ground) return new Point[] { to };

        from.list.Clear();
        foreach (var groundPoint in groundMap[from.ground])
        {
            if (groundPoint.pointType == PointType.JumpPoint || groundPoint.pointType == PointType.LadderPoint)
                from.list.Add(groundPoint);
        }

        string logstring = "";
        foreach (var p in from.list)
        {
            logstring = " " + p.gameObject.name;
        }
        if (from.list.Count == 0) return null;

        Point p2 = null;
        float distance = float.MaxValue;
        if (to.ladder != null)
        {
            foreach (var ladderPoint in ladderMap[to.ladder])
            {
                if (distance < Vector2.Distance(ladderPoint.transform.position, to.transform.position))
                    p2 = ladderPoint;
            }
        }
        else if((to.ground != null))
        {
            foreach (var groundPoint in groundMap[to.ground])
            {
                if (Mathf.Abs(groundPoint.transform.position.x - to.transform.position.x) < distance)
                    p2 = groundPoint;
            }
        }

        if (p2 == null) return null;
        
        Dictionary<Point, Point> predecessor = new Dictionary<Point, Point>(); // 用来还原最短路径
        Dictionary<Point, PointDistance> pointDistanceMap = new Dictionary<Point, PointDistance>();
        ComparePointDistance compare = new ComparePointDistance();
        SortedSet<PointDistance> sortedSet = new SortedSet<PointDistance>(compare); //模拟优先队列
        Dictionary<Point, bool> inqueue = new Dictionary<Point, bool>(); // 标记是否进入过队列
        foreach (var data in groundMap)
        {
            foreach(var point in data.Value)
            {
                pointDistanceMap[point] = new PointDistance(point, float.MaxValue);
            }
        }
        pointDistanceMap[from] = new PointDistance(from, 0);//把第一个节点距离设置为0
        sortedSet.Add(pointDistanceMap[from]);
        inqueue[from] = true;

        while (sortedSet.Count != 0)
        {
            PointDistance min = sortedSet.Min; //取堆顶元素并删除
            sortedSet.Remove(min);
            if (min.point == p2) break; //最短路径产生了
            if(min.point.list != null && min.point.list.Count > 0)
            {
                foreach (var nextPoint in min.point.list)
                {
                    float d = Vector2.Distance(nextPoint.transform.position, min.point.transform.position);
                    if(pointDistanceMap[min.point].distance + d < pointDistanceMap[nextPoint].distance)
                    {
                        pointDistanceMap[nextPoint].distance = pointDistanceMap[min.point].distance + d;
                        predecessor[nextPoint] = min.point;
                        if(!inqueue.ContainsKey(nextPoint))
                        {
                            sortedSet.Add(pointDistanceMap[nextPoint]);
                            inqueue[nextPoint] = true;
                        }
                    }
                }
            }
            
        }

        List<Point> list = new List<Point>();
        list.Add(to);
        list.Add(p2);
        Point tmp = p2;
        while(tmp != from)
        {
            list.Add(tmp);
            tmp = predecessor[tmp];
        }
        list.Reverse();


        return list.ToArray();
    }

    class PointDistance {
        public Point point;
        public float distance;
        public PointDistance(Point point, float distance)
        {
            this.point = point;
            this.distance = distance;
        }
    }

    class ComparePointDistance : IComparer<PointDistance>
    {
        int IComparer<PointDistance>.Compare(PointDistance x, PointDistance y)
        {
            if (x.distance > y.distance) return 1;
            else if (x.distance == y.distance) return 0;
            else return -1;
        }
    }
}
