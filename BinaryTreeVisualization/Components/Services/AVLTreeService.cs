using BinaryTreeVisualization.Components.Services;
using Microsoft.AspNetCore.Routing;

public class AVLTreeService : BSTService
{
    public bool DidRotate { get; private set; }
    // Phương thức thêm nút vào cây AVL
    public override Guid AddNode(int value)
    {
        // Cờ để xác định xem có xoay cây hay không
        DidRotate = false;

        // Thêm nút vào cây nhị phân thông qua phương thức AddNode của lớp cơ sở
        var newNodeID = base.AddNode(value);

        // Sau khi thêm, lấy nút vừa thêm và cân bằng nếu cần
        var newNode = FindNodeFromRoot(value);

        if (newNode != null)
        {
            DidRotate = BalanceTreeAfterInsert(newNode);

            // Cập nhật lại root nếu cần
            if (newNode.Parent == null)
            {
                UpdateRoot(newNode);
            }
        }

        return newNodeID;
    }

    private bool BalanceTreeAfterInsert(NodeService node)
    {
        bool rotated = false;
        NodeService? currentNode = node;

        while (currentNode != null)
        {
            currentNode.UpdateHeight();

            if (!IsBalanced(currentNode))
            {
                PerformRotation(currentNode);
                rotated = true;

                // Sau khi xoay, thoát khỏi vòng lặp vì chỉ xoay một lần
                break;
            }

            // Tiếp tục dò ngược lên cao hơn
            currentNode = currentNode.Parent; 
        }

        return rotated;
    }

    public override bool DeleteNode(int value)
    {
        DidRotate = false;

        // Gọi hàm DeleteNode của BSTService
        bool isDeleted = base.DeleteNode(value);
        if (!isDeleted) return false;

        var nodeToDelete = FindNodeFromRoot(value);

        if (Root != null)
        {
            DidRotate = BalanceTreeAfterDelete(Root);
        }

        return true;
    }



    private bool BalanceTreeAfterDelete(NodeService? node)
    {
        bool rotated = false;
        var allNodes = new List<NodeService>();
        CollectAllNodes(node, allNodes);
     

        bool changed = true;
        // Quay vòng cho đến khi ko rotate nữa
        while (changed)
        {
            changed = false;
            foreach (var n in allNodes)
            {
                n.UpdateHeight();
                if (!IsBalanced(n))
                {
                    PerformRotation(n);
                    rotated = true;
                    changed = true;
                    // Sau xoay => break, rebuild allNodes => 
                    //   (để logic chặt chẽ, ta reload node parent references)
                    UpdateParentReferences(Root, null);
                    ArrangeNodePositions(Root, RootX, RootY, 200);
                    // Gom lại node => break foreach => while => lặp
                    allNodes.Clear();
                    CollectAllNodes(Root, allNodes);
                    break;
                }
            }
        }
        return rotated;
    }

    // Hàm gom toàn bộ node, BFS hoặc DFS
    private void CollectAllNodes(NodeService? node, List<NodeService> list)
    {
        if (node == null) return;
        list.Add(node);
        CollectAllNodes(node.LeftChild, list);
        CollectAllNodes(node.RightChild, list);
    }

    // Kiểm tra cân bằng tại thời điểm thêm nút
    private bool IsBalanced(NodeService node)
    {
        if (node == null) return true;
        return Math.Abs(GetBalanceFactor(node)) <= 1;
    }

    // Tính toán hệ số cân bằng
    private int GetBalanceFactor(NodeService node)
    {
        if (node == null) return 0;
        return (node.LeftChild?.Height ?? 0) - (node.RightChild?.Height ?? 0);
    }

    private NodeService PerformRotation(NodeService node)
    {
        int balanceFactor = GetBalanceFactor(node);

        // Lệch trái -> xoay phải
        if (balanceFactor > 1)
        {
            if (node.LeftChild != null && GetBalanceFactor(node.LeftChild) < 0)
            {
                node.LeftChild = RotateLeft(node.LeftChild);
            }
            return RotateRight(node);
        }
        // Lệch phải -> xoay trái
        else if (balanceFactor < -1)
        {
            if (node.RightChild != null && GetBalanceFactor(node.RightChild) > 0)
            {
                node.RightChild = RotateRight(node.RightChild);
            }
            return RotateLeft(node);
        }

        return node; // Không xoay nếu cây cân bằng
    }

    private NodeService RotateRight(NodeService node)
    {
        NodeService? newRoot = node.LeftChild;
        if (newRoot == null) return node; // Tránh trường hợp null exception

        // Lưu vị trí cũ cho animation
        node.OldPositionX = node.PositionX;
        node.OldPositionY = node.PositionY;
        newRoot.OldPositionX = newRoot.PositionX;
        newRoot.OldPositionY = newRoot.PositionY;

        // Thực hiện xoay
        node.LeftChild = newRoot.RightChild;
        if (newRoot.RightChild != null)
        {
            newRoot.RightChild.Parent = node;
        }
        newRoot.RightChild = node;

        // Cập nhật chiều cao
        node.UpdateHeight();
        newRoot.UpdateHeight();

        // Cập nhật parent
        newRoot.Parent = node.Parent;
        node.Parent = newRoot;

        // Cập nhật liên kết với parent của node gốc
        if (newRoot.Parent == null)
        {
            UpdateRoot(newRoot); // Cập nhật gốc sau khi xoay
        }
        else
        {
            if (newRoot.Parent.LeftChild == node)
            {
                newRoot.Parent.LeftChild = newRoot;
            }
            else
            {
                newRoot.Parent.RightChild = newRoot;
            }
        }
        return newRoot;
    }

    private NodeService RotateLeft(NodeService node)
    {
        NodeService? newRoot = node.RightChild;
        if (newRoot == null) return node; // Tránh trường hợp null exception

        // Lưu vị trí cũ cho animation
        node.OldPositionX = node.PositionX;
        node.OldPositionY = node.PositionY;
        newRoot.OldPositionX = newRoot.PositionX;
        newRoot.OldPositionY = newRoot.PositionY;

        // Thực hiện xoay
        node.RightChild = newRoot.LeftChild;
        if (newRoot.LeftChild != null)
        {
            newRoot.LeftChild.Parent = node;
        }
        newRoot.LeftChild = node;

        // Cập nhật chiều cao
        node.UpdateHeight();
        newRoot.UpdateHeight();

        // Cập nhật parent
        newRoot.Parent = node.Parent;
        node.Parent = newRoot;

        // Cập nhật liên kết với parent của node gốc
        if (newRoot.Parent == null)
        {
            UpdateRoot(newRoot); // Cập nhật gốc sau khi xoay
        }
        else
        {
            if (newRoot.Parent.LeftChild == node)
            {
                newRoot.Parent.LeftChild = newRoot;
            }
            else
            {
                newRoot.Parent.RightChild = newRoot;
            }
        }

        return newRoot;
    }

    public void StoreOldPositions(NodeService? node)
    {
        if (node == null) return;

        node.OldPositionX = node.AnimatedX;
        node.OldPositionY = node.AnimatedY;

        StoreOldPositions(node.LeftChild);
        StoreOldPositions(node.RightChild);
    }



    public override List<(NodeService node, double x, double y)> GetNodePositions(NodeService? root)
    {
        var positions = new List<(NodeService node, double x, double y)>();
        if (root == null) return positions;

        positions.Add((root, root.AnimatedX, root.AnimatedY));
        positions.AddRange(GetNodePositions(root.LeftChild));
        positions.AddRange(GetNodePositions(root.RightChild));

        return positions;
    }

    public void UpdateNodePositions(NodeService? node, double x, double y, double offset)
    {
        if (node == null) return;

        node.PositionX = x;
        node.PositionY = y;

        if (node.LeftChild != null)
        {
            UpdateNodePositions(node.LeftChild, x - offset, y + 80, offset / 2);
        }
        if (node.RightChild != null)
        {
            UpdateNodePositions(node.RightChild, x + offset, y + 80, offset / 2);
        }
    }
    public void SetAnimatedPositionToCurrent()
    {
        SetAnimatedPositionToCurrent(Root);
    }

    private void SetAnimatedPositionToCurrent(NodeService? node)
    {
        if (node == null) return;

        node.AnimatedX = node.PositionX;
        node.AnimatedY = node.PositionY;

        SetAnimatedPositionToCurrent(node.LeftChild);
        SetAnimatedPositionToCurrent(node.RightChild);
    }
}
