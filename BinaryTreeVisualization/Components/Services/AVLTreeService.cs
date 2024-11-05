using BinaryTreeVisualization.Components.Services;

public class AVLTreeService : BSTService
{
    // Phương thức thêm nút vào cây AVL
    public override Guid AddNode(int value)
    {
        // Thêm nút vào cây nhị phân thông qua phương thức AddNode của TreeService
        var newNodeID = base.AddNode(value);

        // Sau khi thêm, lấy nút vừa thêm và cân bằng nếu cần
        var newNode = FindNodeFromRoot(value);

        if (newNode != null)
        {
            BalanceTreeAfterInsert(newNode);

            // Cập nhật lại root nếu cần
            if (newNode.Parent == null)
            {
                UpdateRoot(newNode); // Gọi phương thức cập nhật root
            }
        }

        return newNodeID;
    }

    // Kiểm tra và cân bằng cây sau khi thêm nút
    private void BalanceTreeAfterInsert(NodeService node)
    {
        NodeService? parent = node.Parent;

        while (parent != null)
        {
            // Cập nhật chiều cao của cha
            parent.UpdateHeight();

            // Kiểm tra cân bằng
            if (!IsBalanced(parent))
            {
                // Nếu không cân bằng, thực hiện xoay
                parent = PerformRotation(parent);
            }

            parent = parent.Parent; // Tiếp tục dò ngược lên cao hơn
        }
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

    // Xoay cây
    private NodeService PerformRotation(NodeService node)
    {
        int balanceFactor = GetBalanceFactor(node);

        // Lệch trái -> xoay phải
        if (balanceFactor > 1)
        {
            if (node.LeftChild != null && GetBalanceFactor(node.LeftChild) < 0)
            {
                // Xoay trái trước
                node.LeftChild = RotateLeft(node.LeftChild);
            }
            // Xoay phải
            node = RotateRight(node);
        }
        // Lệch phải -> xoay trái
        else if (balanceFactor < -1)
        {
            if (node.RightChild != null && GetBalanceFactor(node.RightChild) > 0)
            {
                // Xoay phải trước
                node.RightChild = RotateRight(node.RightChild);
            }
            // Xoay trái
            node = RotateLeft(node);
        }

        // Cập nhật parent cho nút mới
        if (node.Parent != null)
        {
            if (node.Parent.LeftChild == node)
            {
                node.Parent.LeftChild = node;
            }
            else
            {
                node.Parent.RightChild = node;
            }
        }
        else
        {
            // Nếu node là gốc, cập nhật lại gốc của cây
            UpdateRoot(node); // Cập nhật gốc sau khi xoay
        }

        // Cập nhật cha cho các nút con
        if (node.LeftChild != null)
        {
            node.LeftChild.Parent = node; // Cập nhật parent cho LeftChild
        }
        if (node.RightChild != null)
        {
            node.RightChild.Parent = node; // Cập nhật parent cho RightChild
        }

        return node; // Trả về nút gốc mới
    }

    private NodeService RotateRight(NodeService node)
    {
        NodeService? newRoot = node.LeftChild;
        if (newRoot == null) return node; // Tránh trường hợp null exception

        node.LeftChild = newRoot.RightChild; // Cập nhật LeftChild của node hiện tại
        if (newRoot.RightChild != null) // Nếu newRoot.RightChild không null, cập nhật parent của nó
        {
            newRoot.RightChild.Parent = node; // Cập nhật parent cho child
        }
        newRoot.RightChild = node; // Đặt node hiện tại làm RightChild của newRoot

        // Cập nhật chiều cao
        node.UpdateHeight();
        newRoot.UpdateHeight();

        // Cập nhật cha cho newRoot
        newRoot.Parent = node.Parent; // Cập nhật parent của newRoot
        node.Parent = newRoot; // Cập nhật parent của node hiện tại

        // Nếu node là gốc, cập nhật lại gốc của cây
        if (newRoot.Parent == null)
        {
            UpdateRoot(node); // Cập nhật gốc sau khi xoay
        }
        else
        {
            // Cập nhật con cho parent
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

        node.RightChild = newRoot.LeftChild; // Cập nhật RightChild của node hiện tại
        if (newRoot.LeftChild != null) // Nếu newRoot.LeftChild không null, cập nhật parent của nó
        {
            newRoot.LeftChild.Parent = node; // Cập nhật parent cho child
        }
        newRoot.LeftChild = node; // Đặt node hiện tại làm LeftChild của newRoot

        // Cập nhật chiều cao
        node.UpdateHeight();
        newRoot.UpdateHeight();

        // Cập nhật cha cho newRoot
        newRoot.Parent = node.Parent; // Cập nhật parent của newRoot
        node.Parent = newRoot; // Cập nhật parent của node hiện tại

        // Nếu node là gốc, cập nhật lại gốc của cây
        if (newRoot.Parent == null)
        {
            UpdateRoot(node); // Cập nhật gốc sau khi xoay
        }
        else
        {
            // Cập nhật con cho parent
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
}