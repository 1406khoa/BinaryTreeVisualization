using BinaryTreeVisualization.Components.Services;

public class BSTService
{
    public NodeService? Root { get; private set; }
    private const double RootX = 800; // Xác định vị trí X cố định cho node gốc
    private const double RootY = 50;  // Y cố định cho node gốc

    public double GetRootX() => RootX;
    public double GetRootY() => RootY;

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
    public List<(NodeService node, double x, double y)> GetNodePositions(NodeService? node, string traversalType = "in-order")
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

    public void AssignPositionsBasedOnTreeStructure(NodeService node, double x, double y, double offsetX)
    {
        double minOffset = 50;

        // Thiết lập vị trí cho node hiện tại
        SetNodePosition(node, x, y);

        if (offsetX > minOffset)
        {
            offsetX *= 0.75; // Giảm dần offsetX cho các node tiếp theo
        }

        // Đặt vị trí cho node con trái (LeftChild)
        if (node.LeftChild != null)
        {
            double leftX = x - offsetX;
            double leftY = y + 100;
            AssignPositionsBasedOnTreeStructure(node.LeftChild, leftX, leftY, offsetX);
        }

        // Đặt vị trí cho node con phải (RightChild)
        if (node.RightChild != null)
        {
            double rightX = x + offsetX;
            double rightY = y + 100;
            AssignPositionsBasedOnTreeStructure(node.RightChild, rightX, rightY, offsetX);
        }
    }

    // Hàm TraverseTree để duyệt cây theo kiểu được chọn (Pre-order, In-order, Post-order, v.v.)
    public List<NodeService> TraverseTree(NodeService? node, string traversalType)
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
    public void ResetTree()
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
            AssignPositionsBasedOnTreeStructure(Root, RootX, RootY, 200);
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

    // Hàm tìm kiếm nút
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

    public void UpdateRoot(NodeService newRoot)
    {
        Root = newRoot; // Cập nhật giá trị Root từ lớp con hoặc bên ngoài
    }
}
