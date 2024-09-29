using BinaryTreeVisualization.Components.Services;

public class TreeService
{
    public NodeService? Root { get; private set; }

    // Hàm thêm nút vào cây nhị phân
    public Guid AddNode(int value)
    {
        NodeService newNode = new NodeService(value);
        if (Root == null)
        {
            Root = newNode;
            SetNodePosition(Root, 800, 50); // Đặt vị trí root node
        }
        else
        {
            AddNodeRecursive(Root, newNode, 800, 200);
        }

        return newNode.NodeID; // Trả về NodeID của node vừa được thêm
    }

    private void AddNodeRecursive(NodeService current, NodeService newNode, double x, double offsetX)
    {
        if (newNode.Value < current.Value)
        {
            if (current.LeftChild == null)
            {
                current.LeftChild = newNode;
                SetNodePosition(current.LeftChild, x - offsetX, current.PositionY + 100);
                current.LeftChild.Parent = current;  // Liên kết cha
            }
            else
            {
                AddNodeRecursive(current.LeftChild, newNode, x - offsetX, offsetX * 0.8);
            }
        }
        else
        {
            if (current.RightChild == null)
            {
                current.RightChild = newNode;
                SetNodePosition(current.RightChild, x + offsetX, current.PositionY + 100);
                current.RightChild.Parent = current;  // Liên kết cha
            }
            else
            {
                AddNodeRecursive(current.RightChild, newNode, x + offsetX, offsetX * 0.8);
            }
        }
    }

    // Hàm duyệt ngược để xác định đường line kết nối với node hiện tại
    public (double x1, double y1, double x2, double y2, Guid LineID)? GetParentLine(NodeService node)
    {
        if (node.Parent == null)
        {
            return null; // Nếu không có cha, tức là node gốc
        }

        var parent = node.Parent;
        // Trả về tọa độ đường line kết nối cha - con cùng với LineID của nó
        return (parent.PositionX, parent.PositionY, node.PositionX, node.PositionY, Guid.NewGuid());
    }

    // Hàm kiểm tra xem cây có cân bằng không (dành cho cây AVL)
    public bool IsBalanced(NodeService? node)
    {
        if (node == null) return true;

        int leftHeight = GetHeight(node.LeftChild);
        int rightHeight = GetHeight(node.RightChild);

        // Kiểm tra chênh lệch độ cao của nhánh trái và nhánh phải
        return Math.Abs(leftHeight - rightHeight) <= 1
            && IsBalanced(node.LeftChild)
            && IsBalanced(node.RightChild);
    }

    // Hàm tính độ cao của cây
    private int GetHeight(NodeService? node)
    {
        if (node == null) return 0;
        return 1 + Math.Max(GetHeight(node.LeftChild), GetHeight(node.RightChild));
    }


    // Hàm thiết lập vị trí cho các nút
    private void SetNodePosition(NodeService node, double x, double y)
    {
        node.PositionX = x;
        node.PositionY = y;
    }

    // Hàm thu thập vị trí của các nút
    public List<(NodeService node, double x, double y)> GetNodePositions(NodeService? node)
    {
        var positions = new List<(NodeService node, double x, double y)>();
        if (node != null)
        {
            // Sử dụng trực tiếp PositionX và PositionY của node thay vì tính lại
            positions.Add((node, node.PositionX, node.PositionY));
            positions.AddRange(GetNodePositions(node.LeftChild));
            positions.AddRange(GetNodePositions(node.RightChild));
        }
        return positions;
    }

    // Hàm thu thập các đường nối giữa node cha và con
    public List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> GetLines()
    {
        var lines = new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();
        CollectLines(Root, lines);
        return lines;
    }

    private void CollectLines(NodeService? node, List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> lines)
    {
        if (node == null) return;

        // Nếu có con trái, thêm đường nối từ cha đến con trái
        if (node.LeftChild != null)
        {
            var lineID = Guid.NewGuid();  // Tạo một LineID duy nhất cho đường nối
            lines.Add((node.PositionX, node.PositionY, node.LeftChild.PositionX, node.LeftChild.PositionY, IsHighlighted: false, lineID));
            CollectLines(node.LeftChild, lines);
        }

        // Nếu có con phải, thêm đường nối từ cha đến con phải
        if (node.RightChild != null)
        {
            var lineID = Guid.NewGuid();  // Tạo một LineID duy nhất cho đường nối
            lines.Add((node.PositionX, node.PositionY, node.RightChild.PositionX, node.RightChild.PositionY, IsHighlighted: false, lineID));
            CollectLines(node.RightChild, lines);
        }
    }

    // Hàm tạo giá trị ngẫu nhiên để sử dụng cho hàm tạo cây ngẫu nhiên
    private List<int> GenerateRandomValues(int count, int minValue, int maxValue)
    {
        Random random = new Random();
        List<int> values = new List<int>();

        for (int i = 0; i < count; i++)
        {
            values.Add(random.Next(minValue, maxValue + 1));
        }

        return values;
    }


    // Hàm tạo cây ngẫu nhiên
    public void BuildRandomTree(int nodeCount, int minValue, int maxValue, string treeType)
    {
        List<int> randomValues = GenerateRandomValues(nodeCount, minValue, maxValue);

        foreach (var value in randomValues)
        {
            switch (treeType)
            {
                case "BinaryTree":
                    AddNode(value);  // Thêm node vào cây Binary Tree
                    break;

                //case "BinarySearchTree":
                //    AddNodeToBST(value);  // Cần viết hàm riêng cho BST
                //    break;

                //case "AVLTree":
                //    AddNodeToAVLTree(value);  // Cần viết hàm riêng cho AVL Tree
                //    break;
            }
        }
    }

    //Hàm xóa cây
    public void ResetTree()
    {
        Root = null; // Đặt lại root về null
    }
}
