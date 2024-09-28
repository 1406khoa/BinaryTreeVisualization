using BinaryTreeVisualization.Components.Services;

public class TreeService
{
    public NodeService? Root { get; private set; }

    // Hàm thêm nút vào cây nhị phân
    public void AddNode(int value)
    {
        NodeService newNode = new NodeService(value);
        if (Root == null)
        {
            Root = newNode;
            SetNodePosition(Root, 800, 50); // Tọa độ gốc (giữa màn hình)
        }
        else
        {
            AddNodeRecursive(Root, newNode, 800, 200); // Thêm node với tọa độ ban đầu và khoảng cách x
        }
    }

    private void AddNodeRecursive(NodeService current, NodeService newNode, double x, double offsetX)
    {
        if (newNode.Value < current.Value)
        {
            if (current.LeftChild == null)
            {
                current.LeftChild = newNode;
                SetNodePosition(current.LeftChild, x - offsetX, current.PositionY + 100); // Di chuyển sang trái
            }
            else
            {
                AddNodeRecursive(current.LeftChild, newNode, x - offsetX, offsetX * 0.8); // Giảm offsetX từ từ hơn
            }
        }
        else
        {
            if (current.RightChild == null)
            {
                current.RightChild = newNode;
                SetNodePosition(current.RightChild, x + offsetX, current.PositionY + 100); // Di chuyển sang phải
            }
            else
            {
                AddNodeRecursive(current.RightChild, newNode, x + offsetX, offsetX * 0.8); // Giảm offsetX từ từ hơn
            }
        }
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
    public List<(double x1, double y1, double x2, double y2)> GetLines()
    {
        var lines = new List<(double x1, double y1, double x2, double y2)>();
        CollectLines(Root, lines);
        return lines;
    }

    private void CollectLines(NodeService? node, List<(double x1, double y1, double x2, double y2)> lines)
    {
        const double circleRadius = 20;  // Bán kính của hình tròn
        const double adjustment = 5;
        const double separation = 10;
        if (node == null) return;

        // Nếu có con trái, vẽ đường từ giữa cạnh dưới của nút cha đến giữa cạnh trên của nút con trái
        if (node.LeftChild != null)
        {
            lines.Add((
                node.PositionX - separation,                                   // Vị trí x của nút cha
                node.PositionY + circleRadius - adjustment,         // Dưới hình tròn cha
                node.LeftChild.PositionX,                           // Vị trí x của nút con trái
                node.LeftChild.PositionY - circleRadius             // Phía trên hình tròn con trái
            ));
            CollectLines(node.LeftChild, lines);
        }

        // Nếu có con phải, vẽ đường từ giữa cạnh dưới của nút cha đến giữa cạnh trên của nút con phải
        if (node.RightChild != null)
        {
            lines.Add((
                node.PositionX + separation,                // Dịch sang phải
                node.PositionY + circleRadius - adjustment, // Điều chỉnh y để đường line đi lên một chút
                node.RightChild.PositionX,                  // Tọa độ x của nút con phải
                node.RightChild.PositionY - circleRadius    // Tọa độ y của nút con phải (kết thúc ở trên hình tròn con)
            ));
            CollectLines(node.RightChild, lines);
        }
    }
}
