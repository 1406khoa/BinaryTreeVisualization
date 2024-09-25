
using BinaryTreeVisualization.Components.Services;
public class TreeService
{
    public ClassNode? Root { get; private set; }

    // Hàm thêm nút vào cây nhị phân
    public void AddNode(int value)
    {
        ClassNode newNode = new ClassNode(value);
        if (Root == null)
        {
            Root = newNode;
            SetNodePosition(Root, 500, 50); // Cố định tọa độ gốc
        }
        else
        {
            AddNodeRecursive(Root, newNode);
        }
    }

    private void AddNodeRecursive(ClassNode current, ClassNode newNode)
    {
        if (newNode.Value < current.Value)
        {
            if (current.LeftChild == null)
            {
                current.LeftChild = newNode;
                SetNodePosition(current.LeftChild, current.PositionX - 50, current.PositionY + 50);
            }
            else
            {
                AddNodeRecursive(current.LeftChild, newNode);
            }
        }
        else
        {
            if (current.RightChild == null)
            {
                current.RightChild = newNode;
                SetNodePosition(current.RightChild, current.PositionX + 50, current.PositionY + 50);
            }
            else
            {
                AddNodeRecursive(current.RightChild, newNode);
            }
        }
    }

    // Hàm thiết lập vị trí cho các nút
    private void SetNodePosition(ClassNode node, double x, double y)
    {
        node.PositionX = x;
        node.PositionY = y;
    }

    public List<(ClassNode node, double x, double y)> GetNodePositions(ClassNode? node, double x, double y, double offsetX)
    {
        var positions = new List<(ClassNode node, double x, double y)>();
        if (node != null)
        {
            positions.Add((node, x, y));
            positions.AddRange(GetNodePositions(node.LeftChild, x - offsetX, y + 100, offsetX / 2));
            positions.AddRange(GetNodePositions(node.RightChild, x + offsetX, y + 100, offsetX / 2));
        }
        return positions;
    }


    // Hàm Duyệt Cây (LNR - In-order Traversal)
    public void InOrderTraversal(ClassNode? node, Action<ClassNode> action)
    {
        if (node == null) return;
        InOrderTraversal(node.LeftChild, action);  // Duyệt trái
        action(node);                              // Thao tác trên nút hiện tại
        InOrderTraversal(node.RightChild, action); // Duyệt phải
    }

    // Hàm thu thập các đường nối giữa node cha và con (lines)
    public List<(double x1, double y1, double x2, double y2)> GetLines()
    {
        var lines = new List<(double x1, double y1, double x2, double y2)>();
        CollectLines(Root, lines);
        return lines;
    }

    private void CollectLines(ClassNode? node, List<(double x1, double y1, double x2, double y2)> lines)
    {
        if (node == null) return;

        // Nếu có con trái, vẽ đường từ node hiện tại đến con trái
        if (node.LeftChild != null)
        {
            lines.Add((node.PositionX, node.PositionY, node.LeftChild.PositionX, node.LeftChild.PositionY));
            CollectLines(node.LeftChild, lines);
        }

        // Nếu có con phải, vẽ đường từ node hiện tại đến con phải
        if (node.RightChild != null)
        {
            lines.Add((node.PositionX, node.PositionY, node.RightChild.PositionX, node.RightChild.PositionY));
            CollectLines(node.RightChild, lines);
        }
    }
}
