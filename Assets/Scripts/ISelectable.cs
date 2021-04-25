public interface ISelectable {
    string Name { get; }
    
    string GetInfo();
    
    void OnRightClick();
}
