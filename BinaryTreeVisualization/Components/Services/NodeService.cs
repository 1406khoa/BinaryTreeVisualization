namespace BinaryTreeVisualization.Components.Services
{
    public class NodeService
    {
        public int Value { get; set; }
        public NodeService? LeftChild { get; set; }  // Nullable
        public NodeService? RightChild { get; set; } // Nullable

        // Thuộc tính hỗ trợ trực quan hóa
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public bool IsHighlighted { get; set; }

        // Liên kết đến node cha
        public NodeService? Parent { get; set; }

        // Độ sâu của node trong cây
        public int Depth { get; set; }

        // Định danh duy nhất cho node
        public Guid NodeID { get; set; }

        // Constructor cho node
        public NodeService(int value)
        {
            Value = value;
            LeftChild = null;
            RightChild = null;
            Parent = null;
            PositionX = 0;
            PositionY = 0;
            IsHighlighted = false;
            Depth = 0;
            NodeID = Guid.NewGuid(); // Tạo ID duy nhất cho node
        }
    }
}
