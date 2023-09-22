namespace SmartGrid.BSP
{

    public interface IBSPNode
    {
        IBSPNode LeftChild { get; set; }
        IBSPNode RightChild { get; set; }
        IBSPNode Parent { get; set; }
        int Value { get; }
        bool IsLeaf { get; }
    }

}