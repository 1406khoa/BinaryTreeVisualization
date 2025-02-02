﻿using BinaryTreeVisualization.Components.Services;
using System.Numerics;

public class BinaryTreeService
{
    public  NodeService? Root { get; private set; }
    private const double RootX = 800; // Xác định vị trí X cố định cho node gốc
    private const double RootY = 50;  // Y cố định cho node gốc

    public double GetRootX() => RootX;
    public double GetRootY() => RootY;

    private Random random = new Random(); // dùng để tạo giá trị ngẫu nhiên

    // Danh sách lưu các đường nối (lines) giữa các node để dễ vẽ và tô màu
    private List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> lines =
        new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();

    // danh sách tọa độ các node
    private List<(NodeService node, double x, double y)> nodePositions = new List<(NodeService node, double x, double y)>();


    public List<NodeService> GetAllNodes()
    {
        List<NodeService> nodes = new List<NodeService>();
        if (Root != null)
        {
            TraverseTree(Root, node => nodes.Add(node));
        }
        return nodes;
    }

    // Hàm trợ giúp TraverseTree dùng đệ quy để duyệt qua tất cả các node
    private void TraverseTree(NodeService? node, Action<NodeService> action)
    {
        if (node == null) return;

        action(node); // Thực hiện thao tác với node hiện tại
        TraverseTree(node.LeftChild, action); // Duyệt nhánh trái
        TraverseTree(node.RightChild, action); // Duyệt nhánh phải
    }

    // Hàm thêm node cây nhị phân tổng quát
    public Guid AddNodeToBinaryTree(int value, NodeService? parentNode, bool? selectedLeftChild = null)
    {
        NodeService newNode = new NodeService(value);              
        // Nếu không có nút cha, thêm nút gốc
        if (Root == null)
        {
            Root = newNode;
            Root.IsRoot = true;
            SetNodePosition(Root, RootX, RootY);
        }
        if (parentNode != null)
        {
            // ** Chế độ Dropdown Menu **
            // Nếu selectedLeftChild có giá trị (true hoặc false)
            // Nghĩa là user đã bấm chọn thêm trái hoặc phải ở menu
            if (selectedLeftChild.HasValue)
            {
                // Nếu selectedLeftChild = true nghĩa là user chọn thêm trái
                if (selectedLeftChild.Value) 
                {
                    if (parentNode.LeftChild == null) // Có chỗ trống thì mới thêm con vào
                    {
                        parentNode.LeftChild = newNode;
                        SetNodePosition(newNode, parentNode.PositionX - 100, parentNode.PositionY + 100); // Cố định vị trí cho con trái
                        newNode.Parent = parentNode;

                        // Lưu đường nối (LineID) giữa node cha và node con trái
                        var lineID = Guid.NewGuid();
                    }
                }
                else
                {
                    if (parentNode.RightChild == null)
                    {
                        parentNode.RightChild = newNode;
                        SetNodePosition(newNode, parentNode.PositionX + 100, parentNode.PositionY + 100); // Cố định vị trí cho con phải
                        newNode.Parent = parentNode;

                        // Lưu đường nối (LineID) giữa node cha và node con phải
                        var lineID = Guid.NewGuid();
                    }
                }
            }
            // ** Chế độ mặc định: thêm trái trước phải sau **
            // Nếu selectedLeftChild không có giá trị (null)
            // Nghĩa là user không mở menu chọn trái phải => vào chế độ mặc định
            else
            {
                if (parentNode.LeftChild == null)
                {
                    parentNode.LeftChild = newNode;
                    SetNodePosition(newNode, parentNode.PositionX - 100, parentNode.PositionY + 100);
                    newNode.Parent = parentNode;
                }
                else if (parentNode.RightChild == null)
                {
                    parentNode.RightChild = newNode;
                    SetNodePosition(newNode, parentNode.PositionX + 100, parentNode.PositionY + 100);
                    newNode.Parent = parentNode;
                }
                // Nếu đã có cả hai con, không làm gì cả
                else
                {
                    return Guid.Empty;
                }
            }
        }

        return newNode.NodeID;
    }

    // Hàm thiết lập vị trí cho các nút, giữ nguyên vị trí node gốc
    private void SetNodePosition(NodeService node, double x, double y)
    {
        if (node == Root)
        {
            node.PositionX = RootX; // Node gốc luôn nằm ở vị trí cố định (RootX, RootY)
            node.PositionY = RootY;
        }
        else
        {
            node.PositionX = x;
            node.PositionY = y;
        }
        // lưu tọa độ vào danh sách
        nodePositions.Add((node, node.PositionX, node.PositionY));
    }

    // Phương thức này trả về danh sách vị trí của các node trong cây
    public  List<(NodeService node, double x, double y)> GetNodePositions(NodeService? node)
    {
        var positions = new List<(NodeService node, double x, double y)>();

        if (node == null)
        {
            return positions; // Trả về danh sách rỗng nếu node là null
        }

        // Duyệt qua cây theo kiểu được chọn và lưu trữ các node
        var nodesInOrder = GetAllNodes();

        // Ghi lại vị trí của mỗi node trong danh sách
        foreach (var n in nodesInOrder)
        {
            positions.Add((n, n.PositionX, n.PositionY));
        }

        return positions;
    }   

    public void ArrangeNodePositions(NodeService node, double x, double y, double offsetX, int depth = 0)
    {
        double adjustedOffset = offsetX / Math.Max(1, Math.Pow(2, depth / 2.0));
        double minOffset = Math.Max(100, adjustedOffset);

        SetNodePosition(node, x, y);

        node.Depth = depth;

        // Kiểm tra va chạm với tất cả các node cùng cấp
        var nodesAtSameLevel = GetAllNodes().Where(n => n.Depth == depth && n != node).ToList();
        foreach (var existingNode in nodesAtSameLevel)
        {
            if (IsOverlappingWithBoundary(existingNode, node))
            {
                x += minOffset; // Đẩy node mới sang phải
                SetNodePosition(node, x, y);
            }
        }

        if (node.LeftChild != null)
        {
            double leftX = x - minOffset;
            double leftY = y + 100;
            ArrangeNodePositions(node.LeftChild, leftX, leftY, offsetX, depth + 1);
        }

        if (node.RightChild != null)
        {
            double rightX = x + minOffset;
            double rightY = y + 100;
            ArrangeNodePositions(node.RightChild, rightX, rightY, offsetX, depth + 1);
        }
    }

    private bool IsOverlappingWithBoundary(NodeService existingNode, NodeService newNode)
    {
        // Kiểm tra xem node mới có nắm trong vùng của bức tường trái hoặc phải của node hiện tại không, chỉ áp dụng với node cùng cấp
        bool overlapLeft = newNode.RightWallX > existingNode.LeftWallX && newNode.PositionX < existingNode.PositionX;
        bool overlapRight = newNode.LeftWallX < existingNode.RightWallX && newNode.PositionX > existingNode.PositionX;

        return overlapLeft || overlapRight;
    }

    // Hàm thu thập đường nối giữa các node cha - con. 
    public virtual List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> GetLines()
    {
        var lines = new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();
        CollectLines(Root, lines);
        return lines;
    }

    // Hàm đệ quy để thu thập các đường nối giữa các node
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

    // Random tree
    public void GenerateRandomBinaryTree(int numNodes, int minValue, int maxValue)
    {
        if (numNodes <= 0)
        {
            Root = null; // Nếu không có node, gán root là null
            return;
        }

        // Tạo node gốc
        Root = new NodeService(RandomValue(minValue, maxValue));
        numNodes--;

        // Tạo cây con ngẫu nhiên
        CreateSubtree(Root, numNodes, minValue, maxValue);
    }

    private void CreateSubtree(NodeService parent, int numNodes, int minValue, int maxValue)
    {
        if (numNodes <= 0) return;

        // Quyết định số lượng node cho con trái và con phải
        int leftNodes = random.Next(0, numNodes + 1);
        int rightNodes = numNodes - leftNodes;

        // Thêm node con trái
        if (leftNodes > 0)
        {
            var leftChild = new NodeService(RandomValue(minValue, maxValue));
            parent.LeftChild = leftChild;
            leftChild.Parent = parent; // Cập nhật parent
            SetNodePosition(leftChild, parent.PositionX - 100, parent.PositionY + 100); // Cố định vị trí
            lines.Add((parent.PositionX, parent.PositionY, leftChild.PositionX, leftChild.PositionY, IsHighlighted: false, Guid.NewGuid())); // Lưu đường nối

            CreateSubtree(leftChild, leftNodes - 1, minValue, maxValue);
        }

        // Thêm node con phải
        if (rightNodes > 0)
        {
            var rightChild = new NodeService(RandomValue(minValue, maxValue));
            parent.RightChild = rightChild;
            rightChild.Parent = parent; // Cập nhật parent
            SetNodePosition(rightChild, parent.PositionX + 100, parent.PositionY + 100); // Cố định vị trí
            lines.Add((parent.PositionX, parent.PositionY, rightChild.PositionX, rightChild.PositionY, IsHighlighted: false, Guid.NewGuid())); // Lưu đường nối

            CreateSubtree(rightChild, rightNodes - 1, minValue, maxValue);
        }
    }

    private int RandomValue(int minValue, int maxValue)
    {
        return random.Next(minValue, maxValue + 1); // Giá trị ngẫu nhiên trong khoảng
    }


    //Hàm xóa cây
    public virtual void ResetTree()
    {
        Root = null;
    }

    // Hàm xóa node
    public void DeleteNode(NodeService? nodeToDelete)
    {
        if (nodeToDelete == null) return;

        // Đệ quy xóa tất cả các node con trước
        DeleteNode(nodeToDelete.LeftChild);
        DeleteNode(nodeToDelete.RightChild);

        // Nếu xóa node gốc, cập nhật lại root = null
        if (nodeToDelete == Root)
        {
            Root = null;
        }
        else
        {
            // Kiểm tra xem cha của nút cần xóa là nút nào, coi nút cần xóa là con trái hay phải của cha đó để xóa
            if (nodeToDelete.Parent != null)
            {
                if (nodeToDelete.Parent.LeftChild == nodeToDelete)
                {
                    nodeToDelete.Parent.LeftChild = null;
                }
                else if (nodeToDelete.Parent.RightChild == nodeToDelete)
                {
                    nodeToDelete.Parent.RightChild = null;
                }
            }
        }
    }

    public virtual async Task MoveControllerToPosition(
    Vector2 currentPosition, Vector2 destination, int speed,
    Action<Vector2> updatePositionCallback)
    {
        float baseSpeed = 2f;
        float movementSpeed = baseSpeed * speed;

        int delayTime = Math.Max(1, 16 / speed);

        while ((destination - currentPosition).Length() > 0.1f)
        {
            Vector2 direction = Vector2.Normalize(destination - currentPosition);
            float step = Math.Min(movementSpeed, (destination - currentPosition).Length());

            currentPosition += direction * step;

            // Gọi callback để cập nhật vị trí trong Razor Component
            updatePositionCallback(currentPosition);

            await Task.Delay(delayTime);
        }

        updatePositionCallback(destination); // Đảm bảo vị trí cuối cùng chính xác
    }

    public virtual List<(double x1, double y1, double x2, double y2)> CollectLinesToNode(NodeService targetNode, List<(double x1, double y1, double x2, double y2)> allLines)
    {
        var path = new List<(double x1, double y1, double x2, double y2)>();
        var currentNode = targetNode;

        while (currentNode.Parent != null)
        {
            // Tìm đường nối giữa parent và node hiện tại từ danh sách đã có
            var line = allLines.FirstOrDefault(l =>
                l.x1 == currentNode.Parent.PositionX &&
                l.y1 == currentNode.Parent.PositionY &&
                l.x2 == currentNode.PositionX &&
                l.y2 == currentNode.PositionY);

            if (line != default)
            {
                path.Insert(0, line); // Thêm vào đầu danh sách để giữ đúng thứ tự từ gốc đến đích
            }

            currentNode = currentNode.Parent;
        }

        return path;
    }
}
