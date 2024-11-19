using BinaryTreeVisualization.Components.Services;

public class BSTService
{
    public  NodeService? Root { get; private set; }
    private const double RootX = 800; // Xác định vị trí X cố định cho node gốc
    private const double RootY = 50;  // Y cố định cho node gốc

    // Danh sách lưu các đường nối (lines) giữa các node để vẽ
    private List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> lines =
        new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();

    //public double GetRootX() => RootX;
    //public double GetRootY() => RootY;

    // Danh sách để lưu trữ giá trị của các node đã thêm vào cây
    public List<int> nodeValues = new List<int>();

    private string CurrentTraversalType = "in-order"; // Kiểu duyệt mặc định

    public List<NodeService> GetAllNodes()
    {
        List<NodeService> nodes = new List<NodeService>();
        PreOrderTraversal(Root, node => nodes.Add(node));
        return nodes;
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
            SetNodePosition(Root, RootX, RootY); // Cố định vị trí của node gốc
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
    public  List<(NodeService node, double x, double y)> GetNodePositions(NodeService? node, string traversalType = "in-order")
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
    // Phương thức tìm node dựa trên vị trí
    public NodeService? GetNodeByPosition(double x, double y, string traversalType = "in-order")
    {
        // Lấy danh sách tất cả node và vị trí của chúng
        var positions = GetNodePositions(Root, traversalType);

        // Tìm node gần nhất với tọa độ (x, y)
        return positions
            .Where(pos => Math.Abs(pos.x - x) < 1 && Math.Abs(pos.y - y) < 1)
            .Select(pos => pos.node)
            .FirstOrDefault();
    }



    // Hàm TraverseTree để duyệt cây theo kiểu được chọn (Pre-order, In-order, Post-order, v.v.)
    public virtual List<NodeService> TraverseTree(NodeService? node, string traversalType)
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

    // Tìm node nhỏ nhất trong cây con
    private NodeService FindMin(NodeService node)
    {
        while (node.LeftChild != null)
        {
            node = node.LeftChild;
        }
        return node;
    }

    public  void ArrangeNodePositions(NodeService node, double x, double y, double offsetX, int depth = 0)
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
    private bool IsOverlapping(NodeService node1, NodeService node2)
    {
        double distance = Math.Sqrt(
            Math.Pow(node1.PositionX - node2.PositionX, 2) +
            Math.Pow(node1.PositionY - node2.PositionY, 2));

        return distance < 100; // Kiểm tra nếu khoảng cách giữa các node nhỏ hơn 40 đơn vị
    }

    // Đẩy các nút ra xa để tránh chồng lấn
    private void PushNodesApart(NodeService node, double pushAmount)
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

    public void SetTraversalType(string traversalType)
    {
        CurrentTraversalType = traversalType;
    }

    // Hàm thu thập đường nối giữa các node cha - con
    public List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> GetLines()
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

    // Hàm xóa node theo giá trị
    public bool DeleteNode(int value)
    {
        // Gọi hàm đệ quy để xóa node
        Root = DeleteNodeRecursive(Root, value);

        // Sau khi xóa node thành công, loại bỏ giá trị khỏi danh sách nodeValues
        if (!nodeValues.Remove(value))
        {
            Console.WriteLine($"Warning: Value {value} not found in nodeValues list.");
        }

        // Cập nhật lại tham chiếu Parent cho tất cả các node còn lại trong cây
        UpdateParentReferences(Root, null);

        // Cập nhật lại vị trí của tất cả các node nếu cây không rỗng
        if (Root != null)
        {
            ArrangeNodePositions(Root, RootX, RootY, 200);
        }

        return Root != null;
    }

    // Hàm đệ quy để cập nhật tham chiếu Parent của các node
    private void UpdateParentReferences(NodeService? currentNode, NodeService? parent)
    {
        if (currentNode == null) return;

        // Cập nhật Parent cho node hiện tại
        currentNode.Parent = parent;

        // Đệ quy cập nhật cho các node con
        UpdateParentReferences(currentNode.LeftChild, currentNode);
        UpdateParentReferences(currentNode.RightChild, currentNode);
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

    // Hàm tìm kiếm nút
    private NodeService? FindNodeRecursive(NodeService? currentNode, int value)
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
            return FindNodeRecursive(currentNode.LeftChild, value);
        }
        else
        {
            return FindNodeRecursive(currentNode.RightChild, value);
        }
    }
    public NodeService? FindNodeFromRoot(int value)
    {
        return FindNodeRecursive(Root, value);
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
