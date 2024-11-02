using BinaryTreeVisualization.Components.Services;
using System.Numerics;

public class BSTService : BinaryTreeService
{
    public new NodeService? Root { get; private set; }
    private const double RootX = 800; // Xác định vị trí X cố định cho node gốc
    private const double RootY = 50;  // Y cố định cho node gốc

    // Danh sách lưu các đường nối (lines) giữa các node để vẽ
    private List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> lines =
        new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();

    //public double GetRootX() => RootX;
    //public double GetRootY() => RootY;

    // Danh sách để lưu trữ giá trị của các node đã thêm vào cây
    private List<int> nodeValues = new List<int>();

    // Thay đổi: Không cần khôi phục lại node gốc nữa vì root sẽ không thay đổi khi duyệt cây.
    private string CurrentTraversalType = "in-order"; // Kiểu duyệt mặc định

    // Hàm thêm node vào cây nhị phân tìm kiếm
    public virtual Guid AddNode(int value)
    {
        NodeService newNode = new NodeService(value);
        if (Root == null)
        {
            Root = newNode;
            Root.IsRoot = true; // Đánh dấu đây là node gốc
            SetNodePosition(Root, RootX, RootY); // Cố định vị trí của node gốc
        }
        else
        {
            AddNodeRecursive(Root, newNode, RootX, 200);
        }
        nodeValues.Add(value); // Lưu lại giá trị của node đã thêm
        return newNode.NodeID;
    }
    // find parent node để thêm animation
    public NodeService? FindParentNode(int value)
    {
        NodeService? currentNode = Root;
        NodeService? parent = null;

        while (currentNode != null)
        {
            parent = currentNode;

            // Nếu giá trị cần thêm nhỏ hơn node hiện tại, đi về bên trái
            if (value < currentNode.Value)
            {
                currentNode = currentNode.LeftChild;
            }
            // Nếu lớn hơn hoặc bằng, đi về bên phải
            else
            {
                currentNode = currentNode.RightChild;
            }
        }

        return parent;
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
                AddNodeRecursive(current.LeftChild, newNode, x - offsetX, offsetX * 0.5);
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
                AddNodeRecursive(current.RightChild, newNode, x + offsetX, offsetX * 0.5);
            }
        }
    }

    // hàm tìm node cha của node hiện tại
    public override (double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)? GetParentLine(NodeService node)
    {
        return lines.FirstOrDefault(line =>
            line.x1 == node.Parent?.PositionX &&
            line.y1 == node.Parent?.PositionY &&
            line.x2 == node.PositionX &&
            line.y2 == node.PositionY);
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
    }

    // Phương thức này trả về danh sách vị trí của các node trong cây
    public override List<(NodeService node, double x, double y)> GetNodePositions(NodeService? node, string traversalType = "in-order")
    {
        var positions = new List<(NodeService node, double x, double y)>();

        if (node == null)
        {
            return positions; // Trả về danh sách rỗng nếu node là null
        }

        // Duyệt qua cây theo kiểu được chọn và lưu trữ các node
        var nodesInOrder = TraverseTree(node, traversalType);

        // Ghi lại vị trí của mỗi node trong danh sách
        foreach (var n in nodesInOrder)
        {
            positions.Add((n, n.PositionX, n.PositionY));
        }

        return positions;
    }

    // Tìm node nhỏ nhất trong cây con
    private NodeService FindMin(NodeService node)
    {
        while (node.LeftChild != null)
        {
            node = node.LeftChild;
        }
        return node;
    }

    public override void ArrangeNodePositions(NodeService node, double x, double y, double offsetX, int depth = 0)
    {
        double minOffset = Math.Max(60, offsetX / Math.Pow(2, depth)); // Khoảng cách tối thiểu giữa các node

        // Đặt vị trí cho node hiện tại
        SetNodePosition(node, x, y);

        // Điều chỉnh vị trí nút con trái nếu tồn tại
        if (node.LeftChild != null)
        {
            double leftX = x - minOffset;
            double leftY = y + 100;

            // Đệ quy để đặt vị trí cho con trái
            ArrangeNodePositions(node.LeftChild, leftX, leftY, offsetX, depth + 1);

            // Kiểm tra và đẩy nút con nếu nó chồng lên nút cha hoặc anh em
            if (IsOverlapping(node, node.LeftChild))
            {
                PushNodesApart(node.LeftChild, -minOffset);
            }
        }

        // Điều chỉnh vị trí nút con phải nếu tồn tại
        if (node.RightChild != null)
        {
            double rightX = x + minOffset;
            double rightY = y + 100;

            // Đệ quy để đặt vị trí cho con phải
            ArrangeNodePositions(node.RightChild, rightX, rightY, offsetX, depth + 1);

            // Kiểm tra và đẩy nút con nếu nó chồng lên nút cha hoặc anh em
            if (IsOverlapping(node, node.RightChild))
            {
                PushNodesApart(node.RightChild, minOffset);
            }
        }
    }

    // Kiểm tra xem hai nút có chồng lên nhau không
    private bool IsOverlapping(NodeService node1, NodeService node2)
    {
        double distance = Math.Sqrt(
            Math.Pow(node1.PositionX - node2.PositionX, 2) +
            Math.Pow(node1.PositionY - node2.PositionY, 2));

        return distance < 70; // Kiểm tra nếu khoảng cách giữa các node nhỏ hơn 40 đơn vị
    }

    // Đẩy các nút ra xa để tránh chồng lấn
    private void PushNodesApart(NodeService node, double pushAmount)
    {
        node.PositionX += pushAmount;

        // Nếu node có con, đẩy tất cả con theo hướng tương tự
        if (node.LeftChild != null)
        {
            PushNodesApart(node.LeftChild, pushAmount);
        }

        if (node.RightChild != null)
        {
            PushNodesApart(node.RightChild, pushAmount);
        }
    }

    // Hàm TraverseTree để duyệt cây theo kiểu được chọn (Pre-order, In-order, Post-order, v.v.)
    public override List<NodeService> TraverseTree(NodeService? node, string traversalType)
    {
        var result = new List<NodeService>();
        if (node == null) return result;

        // Tạo một hành động (Action) để thêm node vào danh sách result
        Action<NodeService> addToResult = node => result.Add(node);

        switch (traversalType)
        {
            case "pre-order":
                PreOrderTraversal(node, addToResult);
                break;
            case "in-order":
                InOrderTraversal(node, addToResult);
                break;
            case "post-order":
                PostOrderTraversal(node, addToResult);
                break;
            case "reverse-in-order":
                ReverseInOrderTraversal(node, addToResult);
                break;
            default:
                InOrderTraversal(node, addToResult); // Mặc định là In-order
                break;
        }
        return result;
    }
    public void SetTraversalType(string traversalType)
    {
        CurrentTraversalType = traversalType;
    }

    // Hàm thu thập đường nối giữa các node cha - con
    public override List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> GetLines()
    {
        var lines = new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();
        CollectLines(Root, lines);
        return lines;
    }

    private void InOrderTraversal(NodeService? node, Action<NodeService> action)
    {
        if (node == null) return;
        InOrderTraversal(node.LeftChild, action);  // Duyệt trái
        action(node);                              // Thao tác với node hiện tại
        InOrderTraversal(node.RightChild, action); // Duyệt phải
    }

    private void PreOrderTraversal(NodeService? node, Action<NodeService> action)
    {
        if (node == null) return;
        action(node);                              // Thao tác với node hiện tại
        PreOrderTraversal(node.LeftChild, action); // Duyệt trái
        PreOrderTraversal(node.RightChild, action);// Duyệt phải
    }

    private void PostOrderTraversal(NodeService? node, Action<NodeService> action)
    {
        if (node == null) return;
        PostOrderTraversal(node.LeftChild, action); // Duyệt trái
        PostOrderTraversal(node.RightChild, action);// Duyệt phải
        action(node);                               // Thao tác với node hiện tại
    }

    private void ReverseInOrderTraversal(NodeService? node, Action<NodeService> action)
    {
        if (node == null) return;
        ReverseInOrderTraversal(node.RightChild, action); // Duyệt phải trước
        action(node);                                     // Thao tác với node hiện tại
        ReverseInOrderTraversal(node.LeftChild, action);  // Duyệt trái sau
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
                case "BinarySearchTree":
                    AddNode(value);
                    break;
                
                //case "AVLTree":
                //    AddNodeToAVLTree(value);
                //    break;
            }
        }
    }

    //Hàm xóa cây
    public override void ResetTree()
    {
        Root = null; // Đặt lại root về null
    }

    // Hàm xóa node theo giá trị
    public bool DeleteNode(int value)
    {
        Root = DeleteNodeRecursive(Root, value);

        // Sau khi xóa node, cập nhật lại vị trí của tất cả các node
        if (Root != null)
        {
            ArrangeNodePositions(Root, RootX, RootY, 200);
        }

        return Root != null;
    }

    // Đệ quy xóa node khỏi cây nhị phân
    private NodeService? DeleteNodeRecursive(NodeService? node, int value)
    {
        if (node == null) return null;

        if (value < node.Value)
        {
            node.LeftChild = DeleteNodeRecursive(node.LeftChild, value);
        }
        else if (value > node.Value)
        {
            node.RightChild = DeleteNodeRecursive(node.RightChild, value);
        }
        else
        {
            // Node cần xóa được tìm thấy

            // Trường hợp 1: Node không có con hoặc chỉ có 1 con
            if (node.LeftChild == null) return node.RightChild;
            if (node.RightChild == null) return node.LeftChild;

            // Trường hợp 2: Node có 2 con
            // Tìm node nhỏ nhất trong nhánh phải
            var minLargerNode = FindMin(node.RightChild);
            node.Value = minLargerNode.Value; // Thay thế giá trị node hiện tại bằng giá trị node nhỏ nhất nhánh phải
            node.RightChild = DeleteNodeRecursive(node.RightChild, minLargerNode.Value); // Xóa node nhỏ nhất nhánh phải
        }

        return node;
    }


    public void UpdateRoot(NodeService newRoot)
    {
        Root = newRoot; // Cập nhật giá trị Root từ lớp con hoặc bên ngoài
    }

    public override async Task MoveControllerToPosition(
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

    public override List<(double x1, double y1, double x2, double y2)> CollectLinesToNode(NodeService targetNode, List<(double x1, double y1, double x2, double y2)> allLines)
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
