using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBTree : MonoBehaviour
{
    public RBTreeNode pfbTreeNode;
    public RBTreeLink pfbTreeLink;
    public Transform nodeRoot;
    public Transform linkRoot;
    public RectTransform content;
    public bool hideRedNode;

    public string initValues;
    public string values;
    public bool DoInsert;
    public bool DoDelete;
    public bool DoResetx;

    protected RBTreeNode m_root;

    void Start()
    {
        if (!string.IsNullOrEmpty(initValues))
        {
            var temps = initValues.Split(' ');
            foreach (var value in temps)
            {
                AddValue(int.Parse(value));
            }
        }
        
    }
    void Update()
    {
        if (DoInsert)
        {
            if (!string.IsNullOrEmpty(values))
            {
                var temps = values.Split(' ');
                foreach (var value in temps)
                {
                    AddValue(int.Parse(value));
                }
            }
            else
            {
                AddValue(Random.Range(1,101));
            }
            DoInsert = false;
        }
        if (DoDelete)
        {
            if (!string.IsNullOrEmpty(values))
            {
                var temps = values.Split(' ');
                foreach (var value in temps)
                {
                    DelValue(int.Parse(value));
                }
            }
            else
            {
                DelValue(Random.Range(1, 101));
            }
            DoDelete = false;
        }
        if (DoResetx)
        {
            _RemoveNode(m_root, true);
            m_root = null;
            DoResetx = false;
        }
    }

    public void AddValue(int value)
    {
        if (m_root == null)
        {
            m_root = _CreateRoot(value);
            _RepaintTree();
            return;
        }
        RBTreeNode insertNode = _SearchInsertNode(m_root, value);
        if (insertNode == null)
        {
            Debug.Log($"已经存在 {value}");
            return;
        }
        Debug.Log($"添加{value}");
        if (insertNode.IsLeft)
        {
            _CreateLNode(insertNode.parent, value);
        }
        else
        {
            _CreateRNode(insertNode.parent, value);
        }
        _FixAddNode(insertNode);
        _RepaintTree();
    }
    public void DelValue(int value)
    {
        if (m_root == null)
            return;
        RBTreeNode deletedNode = _SearchNode(m_root, value);
        if (deletedNode == null)
        {
            return;
        }
        Debug.Log($"删除{value}");
        RBTreeNode replaceNode = null;
        //同时拥有左右子节点
        if (!deletedNode.lNode.isNil && !deletedNode.rNode.isNil)
        {
            //查找后驱点
            var tempNode = _FindLNodeAtRTree(deletedNode.rNode);
            var tempVal = tempNode.value;
            tempNode.SetVal(deletedNode.value);
            deletedNode.SetVal(tempVal);

            deletedNode = tempNode;
            replaceNode = _RemoveNode(tempNode);
        }
        else
        {
            replaceNode = _RemoveNode(deletedNode);
        }
        _FixDelNode(deletedNode, replaceNode);
        _RepaintTree();
    }

    #region 红黑树修正

    #region 新增修复

    //添加节点修正
    protected void _FixAddNode(RBTreeNode node)
    {
        if (node.parent == m_root)
        {
            node.SetBlack();
            return;
        }
        if (node.parent.IsBlack)
            return;
        _FixAddNodeCase(node);
        m_root.SetBlack();
    }

    protected bool _FixAddNodeCase(RBTreeNode node)
    {
        if (_FixAddNodeCase5(node))
            return true;
        if (_FixAddNodeCase1(node))
            return true;
        if (_FixAddNodeCase2(node))
            return true;
        if (_FixAddNodeCase3(node))
            return true;
        if (_FixAddNodeCase4(node))
            return true;
        return false;
    }

    //父红，父(左)，叔黑，新(左)
    protected bool _FixAddNodeCase1(RBTreeNode node)
    {
        if (!node.parent.IsLeft)
            return false;
        var uncle = _GetUncleNode(node);
        if (uncle == null)
            return true;
        if (!uncle.IsBlack)
            return false;
        if (!node.IsLeft)
            return false;
        node.parent.SetBlack();
        node.parent.parent.SetRed();
        _RotRight(node.parent.parent);
        return true;
    }
    //父红，父(左)，叔黑，新(右)
    protected bool _FixAddNodeCase2(RBTreeNode node)
    {
        if (!node.parent.IsLeft)
            return false;
        var uncle = _GetUncleNode(node);
        if (uncle == null)
            return true;
        if (!uncle.IsBlack)
            return false;
        if (!node.IsRight)
            return false;
        var parent = node.parent;
        _RotLeft(parent);
        return _FixAddNodeCase1(parent);
    }
    //父红，父(右)，叔黑，新(左)
    protected bool _FixAddNodeCase3(RBTreeNode node)
    {
        if (!node.parent.IsRight)
            return false;
        var uncle = _GetUncleNode(node);
        if (uncle == null)
            return true;
        if (!uncle.IsBlack)
            return false;
        if (!node.IsLeft)
            return false;
        var parent = node.parent;
        _RotRight(parent);
        return _FixAddNodeCase4(parent);
    }
    //父红，父(右)，叔黑，新(右)
    protected bool _FixAddNodeCase4(RBTreeNode node)
    {
        if (!node.parent.IsRight)
            return false;
        var uncle = _GetUncleNode(node);
        if (uncle == null)
            return true;
        if (!uncle.IsBlack)
            return false;
        if (!node.IsRight)
            return false;
        node.parent.SetBlack();
        node.parent.parent.SetRed();
        _RotLeft(node.parent.parent);
        return true;
    }

    //父红，叔红
    protected bool _FixAddNodeCase5(RBTreeNode node)
    {
        var uncle = _GetUncleNode(node);
        if (uncle == null)
            return true;
        if (!uncle.IsRed)
            return false;
        node.parent.SetBlack();
        uncle.SetBlack();

        var grand = node.parent.parent;
        grand.SetRed();
        if (grand.parent != null && grand.parent.IsRed)
        {
            return _FixAddNodeCase(node.parent.parent);
        }
        return true;
    }

    #endregion

    #region 删除修复

    //删除节点修正
    protected void _FixDelNode(RBTreeNode deletedNode, RBTreeNode replaceNode)
    {
        if (deletedNode.IsRed)
            return;
        if (deletedNode.parent == null)
            return;
        //删黑，有右子（必红）， 涂黑即可
        if (replaceNode.IsRed)
        {
            replaceNode.SetBlack();
            return;
        }
        if (_FixDelNodeCase1(replaceNode))
            return;
    }

    //删父为红，兄为黑，兄两子皆黑，则父染黑，兄染红
    protected bool _FixDelNodeCase1(RBTreeNode node)
    {
        if (node.parent.IsBlack)
            return false;
        var bro = _GetBrotherNode(node);
        if (bro.IsRed)
            return false;
        node.parent.SetBlack();
        bro.SetRed();
        return true;
    }

    //父为黑(左子)，兄为黑
    protected bool _FixDelNodeCase2(RBTreeNode node)
    {
        if (node.parent.IsRed)
            return false;
        if (node.parent.IsRight)
            return false;
        var bro = _GetBrotherNode(node);
        if (bro.IsRed)
            return false;
        if (_FixDelNodeCase2_1(node))
            return true;
        if (_FixDelNodeCase2_2(node))
            return true;
        return false;
    }
    //兄有红右子，父兄换色，父染黑，兄右子染黑，父左旋
    protected bool _FixDelNodeCase2_1(RBTreeNode node)
    {

        return false;
    }
    //兄有红左子，兄与左子换色，兄右旋，然后_FixDelNodeCase2_1
    protected bool _FixDelNodeCase2_2(RBTreeNode node)
    {
        return false;
    }

    #endregion

    //左旋
    protected void _RotLeft(RBTreeNode node)
    {
        var parent = node.parent;
        var n = node;
        var nr = node.rNode;
        var nrl = nr.lNode;

        _ChangeParent(parent, n, nr);
        _ChangeParent(nr, nrl, n);
        _ChangeParent(n, nr, nrl);
    }
    //右旋
    protected void _RotRight(RBTreeNode node)
    {
        var parent = node.parent;
        var n = node;
        var nl = node.lNode;
        var nlr = nl.rNode;

        _ChangeParent(parent, n, nl);
        _ChangeParent(nl, nlr, n);
        _ChangeParent(n, nl, nlr);
    }

    //替换节点 node1的父节点设置为node2的父节点
    protected void _ChangeParent(RBTreeNode parent, RBTreeNode node1, RBTreeNode node2)
    {
        if (parent == null)//说明是跟节点
        {
            m_root = node2;
            m_root.parent = null;
            return;
        }
        if (parent.lNode == node1)
        {
            node2.SetLeft();
            node2.parent = parent;
            node2.parent.lNode = node2;
            return;
        }
        if (parent.rNode == node1)
        {
            node2.SetRight();
            node2.parent = parent;
            node2.parent.rNode = node2;
            return;
        }
    }

    protected RBTreeNode _GetBrotherNode(RBTreeNode node)
    {
        if (node.IsLeft)
            return node.parent.rNode;
        return node.parent.lNode;
    }
    protected RBTreeNode _GetUncleNode(RBTreeNode node)
    {
        var parent = node.parent;
        if (parent.parent == null)
            return null;
        if (parent.IsLeft)
            return parent.parent.rNode;
        return parent.parent.lNode;
    }

    //查找后驱点
    protected RBTreeNode _FindLNodeAtRTree(RBTreeNode node)
    {
        if (node.lNode.isNil)
            return node;
        return _FindLNodeAtRTree(node.lNode);
    }


    #endregion

    protected void _RepaintTree()
    {
        m_root.CalcTreeWidth();
        m_root.CalcTreeHeight();

        var totalw = m_root.Width + 100f;
        content.sizeDelta = new Vector2(totalw, m_root.Height + 100f);
        m_root.SetPosition(new Vector2((m_root.WidthL - m_root.WidthR) * 0.5f, m_root.Height * 0.5f));
    }

    //查找插入的节点
    protected RBTreeNode _SearchInsertNode(RBTreeNode node, int value)
    {
        if (node.value == value)
            return null;
        if (node.isNil)
            return node;
        if (value > node.value)
            return _SearchInsertNode(node.rNode, value);
        return _SearchInsertNode(node.lNode, value);
    }
    //查找节点
    protected RBTreeNode _SearchNode(RBTreeNode node, int value)
    {
        if (node.isNil)
            return null;
        if (node.value == value)
            return node;
        if (value > node.value)
            return _SearchNode(node.rNode, value);
        return _SearchNode(node.lNode, value);
    }

    //test
    protected RBTreeNode _CreateRoot(int value)
    {
        var root = _CreateNode();
        root.SetBlack();
        root.SetVal(value);
        _CreateLNil(root);
        _CreateRNil(root);
        return root;
    }
    protected void _CreateLNode(RBTreeNode parent, int value)
    {
        parent.lNode.SetVal(value);
        parent.lNode.SetRed();
        parent.lNode.SetLeft();
        _CreateLNil(parent.lNode);
        _CreateRNil(parent.lNode);
    }
    protected void _CreateRNode(RBTreeNode parent, int value)
    {
        parent.rNode.SetVal(value);
        parent.rNode.SetRed();
        parent.rNode.SetRight();
        _CreateLNil(parent.rNode);
        _CreateRNil(parent.rNode);
    }

    protected void _CreateLNil(RBTreeNode node)
    {
        node.lNode = _CreateNode(node);
        node.lNode.SetNil();
        node.lNode.SetBlack();
        node.lNode.SetLeft();
    }
    protected void _CreateRNil(RBTreeNode node)
    {
        node.rNode = _CreateNode(node);
        node.rNode.SetNil();
        node.rNode.SetBlack();
        node.rNode.SetRight();
    }

    protected RBTreeNode _CreateNode(RBTreeNode parent = null)
    {
        var go = Instantiate(pfbTreeNode.gameObject, nodeRoot);
        var mono = go.GetComponent<RBTreeNode>();
        mono.parent = parent;
        mono.parentLink = _CreateLink();
        return mono;
    }
    protected RBTreeLink _CreateLink()
    {
        var go = Instantiate(pfbTreeLink.gameObject, linkRoot);
        var mono = go.GetComponent<RBTreeLink>();
        return mono;
    }
    protected RBTreeNode _RemoveNode(RBTreeNode node, bool includeChild = false)
    {
        if (node == null)
            return null;
        if (node.parentLink != null)
        {
            Destroy(node.parentLink.gameObject);
            node.parentLink = null;
        }
        Destroy(node.gameObject);

        if (includeChild)
        {
            if (node.isNil)
                return null;
            _RemoveNode(node.lNode, true);
            _RemoveNode(node.rNode, true);
            return null;
        }
        RBTreeNode childNode = null;
        if (node.lNode.isNil)
        {
            childNode = node.rNode;
            _RemoveNode(node.lNode, true);
        }
        else
        {
            childNode = node.lNode;
            _RemoveNode(node.rNode, true);
        }
        _ChangeParent(node.parent, node, childNode);

        return childNode;
    }
}

