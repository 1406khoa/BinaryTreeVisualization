using BinaryTreeVisualization.Components.Services;
using System.Xml.Linq;

public class BinaryTreeService
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

    private Random random = new Random();

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
                        lines.Add((parentNode.PositionX, parentNode.PositionY, newNode.PositionX, newNode.PositionY, IsHighlighted: false, lineID));
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
                        lines.Add((parentNode.PositionX, parentNode.PositionY, newNode.PositionX, newNode.PositionY, IsHighlighted: false, lineID));
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

        nodeValues.Add(value); // Lưu lại giá trị của node đã thêm
        return newNode.NodeID;
    }

    // Danh sách lưu các đường nối (lines) giữa các node để dễ vẽ và tô màu
    private List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)> lines =
        new List<(double x1, double y1, double x2, double y2, bool IsHighlighted, Guid LineID)>();

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

    private List<(NodeService node, double x, double y)> nodePositions = new List<(NodeService node, double x, double y)>();// danh sách tọa độ các node

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

    // Hàm này dùng để gán vị trí cho các node dựa trên cấu trúc cây
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

    // Random tree
    public void GenerateRandomBinaryTree(int numNodes, int minValue, int maxValue)
    {
        nodeValues.Clear(); // Xóa danh sách trước khi tạo cây mới
        if (numNodes <= 0)
        {
            Root = null; // Nếu không có node, gán root là null
            return;
        }

        // Tạo node gốc
        Root = new NodeService(RandomValue(minValue, maxValue));
        nodeValues.Add(Root.Value); // Lưu giá trị của node gốc
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

    private bool RandomBool()
    {
        return random.NextDouble() >= 0.5; // Trả về true hoặc false ngẫu nhiên
    }

    //Hàm xóa cây
    public void ResetTree()
    {
        Root = null; // Đặt lại root về null
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
            // Cập nhật parent để xóa node
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
}
