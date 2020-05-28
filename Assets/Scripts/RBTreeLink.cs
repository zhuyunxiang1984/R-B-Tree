using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBTreeLink : MonoBehaviour
{
    public RectTransform trans;
    public void LinkNode(RBTreeNode node1 , RBTreeNode node2)
    {
        if (node1 == null || node2 == null)
            return;
        var pos1 = node1.trans.anchoredPosition;
        var pos2 = node2.trans.anchoredPosition;
        if (pos1 == pos2)
            return;

        trans.anchoredPosition = (pos1 + pos2) * 0.5f;
        trans.rotation = Quaternion.Euler(0, 0, _CalcAngle(pos1, pos2));
        trans.sizeDelta = new Vector3(Vector3.Distance(pos1, pos2), 5);
    }

    protected float _CalcAngle(Vector2 pos1, Vector2 pos2)
    {
        var offset = pos2 - pos1;
        var tan = offset.y / offset.x;
        return Mathf.Atan(tan) * Mathf.Rad2Deg;
    }
}
