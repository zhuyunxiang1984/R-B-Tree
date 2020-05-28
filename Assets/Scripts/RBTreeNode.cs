using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RBTreeNode : MonoBehaviour
{
    public RectTransform trans;
    public Image img;
    public TextMeshProUGUI label;
    public RBTreeNode parent;
    public RBTreeLink parentLink;
    public RBTreeNode lNode;
    public RBTreeNode rNode;
    public bool blackOrRed;
    public bool leftOrRight;
    public int value;
    public bool isNil;

    protected float m_height;
    protected float m_width;
    protected float m_widthl;
    protected float m_widthr;

    public float Width { get { return m_width; } }
    public float WidthL { get { return m_widthl; } }
    public float WidthR { get { return m_widthr; } }
    public float Height { get { return m_height; } }
    

    public void SetVal(int value)
    {
        this.value = value;
        isNil = false;
        label.text = value.ToString();
        gameObject.name = label.text;
    }
    public void SetNil()
    {
        isNil = true;
        label.text = "Nil";
        gameObject.name = label.text;
    }
    public bool IsLeft { get { return leftOrRight; } }
    public bool IsRight { get { return !leftOrRight; } }
    public void SetLeft()
    {
        leftOrRight = true;
    }
    public void SetRight()
    {
        leftOrRight = false;
    }

    public bool IsBlack { get { return blackOrRed; } }
    public bool IsRed { get { return !blackOrRed; } }
    public void SetBlack()
    {
        blackOrRed = true;
        img.color = Color.black;
        label.color = Color.white;
    }
    public void SetRed()
    {
        blackOrRed = false;
        img.color = Color.red;
        label.color = Color.white;
    }

    public const float paddingx = 10f;
    public const float paddingy = 50f;

    public void SetPosition(Vector2 position)
    {
        trans.anchoredPosition = position;
        if (parent != null)
        {
            parentLink.gameObject.SetActive(true);
            parentLink.LinkNode(this, parent);
        }
        else
        {
            parentLink.gameObject.SetActive(false);
        }
        if (isNil)
            return;
        //lNode.SetPosition(new Vector2(position.x - m_width * 0.25f, position.y - paddingy));
        //rNode.SetPosition(new Vector2(position.x + m_width * 0.25f, position.y - paddingy));

        lNode.SetPosition(new Vector2(position.x - (lNode.WidthR + paddingx * 0.5f), position.y - paddingy));
        rNode.SetPosition(new Vector2(position.x + (rNode.WidthL + paddingx * 0.5f), position.y - paddingy));
    }

    //计算树的宽度
    public float CalcTreeWidth()
    {
        if (isNil)
        {
            m_width = 50f;
            m_widthl = m_width * 0.5f;
            m_widthr = m_width * 0.5f;
        }
        else
        {
            var lw = lNode.CalcTreeWidth();
            var rw = rNode.CalcTreeWidth();
            //m_width = Mathf.Max(lw, rw) * 2 + paddingx;
            m_widthl = lw;
            m_widthr = rw;
            m_width = lw + rw + paddingx;
        }
        return m_width;
    }
    public float CalcTreeHeight()
    {
        if (isNil)
        {
            m_height = 50f;
        }
        else
        {
            var lw = lNode.CalcTreeHeight();
            var rw = rNode.CalcTreeHeight();
            m_height = Mathf.Max(lw, rw) + paddingy;
        }
        return m_height;
    }
    
}
