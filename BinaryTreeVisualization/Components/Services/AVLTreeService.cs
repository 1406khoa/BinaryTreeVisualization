using BinaryTreeVisualization.Components.Services;

public class AVLTreeService : TreeService 
{
    // Hàm kiểm tra tính cân bằng của cây AVL dựa trên thuộc tính Height
    public bool IsBalanced(NodeService? node)
    {
        if (node == null) return true;

        // Tính chiều cao của nhánh trái và nhánh phải
        int leftHeight = node.LeftChild?.Height ?? 0;
        int rightHeight = node.RightChild?.Height ?? 0;

        // Kiểm tra sự chênh lệch chiều cao giữa nhánh trái và nhánh phải
        bool isBalanced = Math.Abs(leftHeight - rightHeight) <= 1;

        // Đệ quy kiểm tra cân bằng cho các node con
        return isBalanced && IsBalanced(node.LeftChild) && IsBalanced(node.RightChild);
    }

    // Hàm tính độ cao của cây
    private int GetHeight(NodeService? node)
    {
        if (node == null) return 0;
        return 1 + Math.Max(GetHeight(node.LeftChild), GetHeight(node.RightChild));
    }
}

