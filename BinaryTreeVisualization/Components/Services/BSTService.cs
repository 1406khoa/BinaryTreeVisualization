using BinaryTreeVisualization.Components.Services;

public class BSTService
{
    public  NodeService? Root { get; protected set; }

    public const double RootX = 800; // Xác định vị trí X cố định cho node gốc
    public const double RootY = 50;  // Y cố định cho node gốc

    // Danh sách lưu các đường nối (lines) giữa các node để vẽ
    private List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> lines =
        new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();

    // Danh sách để lưu trữ giá trị của các node đã thêm vào cây
    public List<int> nodeValues = new List<int>();


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

    // Hàm thêm node vào cây nhị phân tìm kiếm
    public virtual Guid AddNode(int value)
    {
        NodeService newNode = new NodeService(value);
        if (nodeValues.Contains(value))
        {
            return Guid.Empty;
        }

        if (Root == null)
        {
            Root = newNode;
            Root.IsRoot = true; // Đánh dấu đây là node gốc
            SetNodePosition(Root, RootX, RootY);
        }
        else
        {
            AddNodeRecursive(Root, newNode, RootX, 200);
        }
        nodeValues.Add(value); // Lưu lại giá trị của node đã thêm
        return newNode.NodeID;
    }

    //Find parent node để thêm animation
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

    // Hàm thiết lập vị trí cho các nút, giữ nguyên vị trí node gốc
    public virtual void SetNodePosition(NodeService? node, double x, double y)
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
    public virtual  List<(NodeService node, double x, double y)> GetNodePositions(NodeService? node)
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
    // Phương thức tìm node dựa trên vị trí
    public NodeService? GetNodeByPosition(double x, double y, string traversalType = "in-order")
    {
        // Lấy danh sách tất cả node và vị trí của chúng
        var positions = GetNodePositions(Root);

        // Tìm node gần nhất với tọa độ (x, y)
        return positions
            .Where(pos => Math.Abs(pos.x - x) < 1 && Math.Abs(pos.y - y) < 1)
            .Select(pos => pos.node)
            .FirstOrDefault();
    }






    public  void ArrangeNodePositions(NodeService? node, double x, double y, double offsetX, int depth = 0)
    {
        double minOffset = Math.Max(60, offsetX / Math.Pow(2, depth)); // Khoảng cách tối thiểu giữa các node

        // Đặt vị trí cho node hiện tại
        SetNodePosition(node, x, y);

        // Điều chỉnh vị trí nút con trái nếu tồn tại
        if (node.LeftChild != null)
        {
            double leftX = x - minOffset;
            double leftY = y + 115;

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
            double rightY = y + 115;

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
    public virtual bool IsOverlapping(NodeService node1, NodeService node2)
    {
        double distance = Math.Sqrt(
            Math.Pow(node1.PositionX - node2.PositionX, 2) +
            Math.Pow(node1.PositionY - node2.PositionY, 2));

        return distance < 100; // Kiểm tra nếu khoảng cách giữa các node nhỏ hơn 40 đơn vị
    }

    // Đẩy các nút ra xa để tránh chồng lấn
    public virtual void PushNodesApart(NodeService node, double pushAmount)
    {
        node.PositionX *= pushAmount;

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



   


    // Hàm thu thập đường nối giữa các node cha - con
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

    // Hàm tạo giá trị ngẫu nhiên
    private List<int> GenerateRandomValues(int count, int minValue, int maxValue)
    {
        Random random = new Random();
        HashSet<int> uniqueValues = new HashSet<int>();

        // Tiếp tục tạo số cho đến khi có đủ count số khác nhau
        while (uniqueValues.Count < count)
        {
            int newValue = random.Next(minValue, maxValue + 1);
            uniqueValues.Add(newValue); // HashSet sẽ tự động loại bỏ nếu trùng
        }

        return uniqueValues.ToList(); // Chuyển đổi lại thành List<int>
    }

    //Hàm tạo cây ngẫu nhiên
    public void BuildRandomTree(int nodeCount, int minValue, int maxValue)
    {
        List<int> randomValues = GenerateRandomValues(nodeCount, minValue, maxValue);

        foreach (var value in randomValues)
        {
            AddNode(value);
        }
    }

    //Hàm xóa cây
    public void ResetTree()
    {
        Root = null;
        nodeValues.Clear();
    }

    public virtual bool DeleteNode(int value)
    {
        // Thực hiện xóa đệ quy
        Root = DeleteNodeRecursive(Root, value);

        // Nếu xóa không thành công, Root sẽ không đổi hoặc nodeValues không mất giá trị
        bool removed = nodeValues.Remove(value);
        if (!removed)
        {
            Console.WriteLine($"[BSTService] Warning: Value {value} not found in nodeValues.");
        }

        // Cập nhật Parent cho toàn bộ
        UpdateParentReferences(Root, null);

        // Arrange lại vị trí nếu còn node
        if (Root != null)
        {
            ArrangeNodePositions(Root, RootX, RootY, 200);
        }

        // Trả về true nếu Root != null nghĩa là còn cây hoặc xóa xong vẫn hợp lệ
        // Tuỳ bạn muốn logic trả true/false như thế nào.
        return removed;
    }

    // Hàm đệ quy để cập nhật tham chiếu Parent của các node
    public void UpdateParentReferences(NodeService? currentNode, NodeService? parent)
    {
        if (currentNode == null) return;

        // Cập nhật Parent cho node hiện tại
        currentNode.Parent = parent;

        // Đệ quy cập nhật cho các node con
        UpdateParentReferences(currentNode.LeftChild, currentNode);
        UpdateParentReferences(currentNode.RightChild, currentNode);
    }


    private NodeService? DeleteNodeRecursive(NodeService? current, int value)
    {
        if (current == null) return null;

        if (value < current.Value)
        {
            current.LeftChild = DeleteNodeRecursive(current.LeftChild, value);
        }
        else if (value > current.Value)
        {
            current.RightChild = DeleteNodeRecursive(current.RightChild, value);
        }
        else
        {
            // Tìm thấy node cần xóa
            // TH1: Node lá
            if (current.LeftChild == null && current.RightChild == null)
            {
                return null; // Bỏ node này (xóa)
            }
            // TH2: Node có 1 con
            else if (current.LeftChild == null)
            {
                return current.RightChild;
            }
            else if (current.RightChild == null)
            {
                return current.LeftChild;
            }
            // TH3: Node có 2 con => copy predecessor
            else
            {
                // Lấy node có giá trị lớn nhất bên trái
                var predecessor = FindMax(current.LeftChild);
                // Copy giá trị
                current.Value = predecessor.Value;
                // Xóa predecessor
                current.LeftChild = DeleteNodeRecursive(current.LeftChild, predecessor.Value);
            }
        }
        return current;
    }

    protected NodeService FindMax(NodeService node)
    {
        while (node.RightChild != null)
        {
            node = node.RightChild;
        }
        return node;
    }

    public void UpdateRoot(NodeService newRoot)
    {
        Root = newRoot; // Cập nhật giá trị Root từ lớp con hoặc bên ngoài
    }

    public NodeService? FindNodeFromRoot(int value)
    {
        return SearchNode(Root, value);
    }

    public NodeService? SearchNode(NodeService? currentNode, int value)
    {
        if (currentNode == null)
        {
            return null;
        }
        if (currentNode.Value == value)
        {
            return currentNode;
        }
        else if (value < currentNode.Value)
        {
            return SearchNode(currentNode.LeftChild, value);
        }
        else
        {
            return SearchNode(currentNode.RightChild, value);
        }
    }
}
