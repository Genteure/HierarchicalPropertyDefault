using HierarchicalPropertyDefault;

namespace UnitTest.Models
{
    public class Tree : HierarchicalObject<Tree, Tree>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
    public class Tree0 : HierarchicalObject<Tree, Tree0>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
    public class Tree1 : HierarchicalObject<Tree, Tree1>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
    public class Tree00 : HierarchicalObject<Tree0, Tree00>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
    public class Tree01 : HierarchicalObject<Tree0, Tree01>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
    public class Tree10 : HierarchicalObject<Tree1, Tree10>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
    public class Tree11 : HierarchicalObject<Tree1, Tree11>
    {
        public string? Property1 { get => this.GetPropertyValue<string?>(); set => this.SetPropertyValue(value); }
        public int Property2 { get => this.GetPropertyValue<int>(); set => this.SetPropertyValue(value); }
        public int? Property3 { get => this.GetPropertyValue<int?>(); set => this.SetPropertyValue(value); }
        public bool Property4 { get => this.GetPropertyValue<bool>(); set => this.SetPropertyValue(value); }
        public bool? Property5 { get => this.GetPropertyValue<bool?>(); set => this.SetPropertyValue(value); }
    }
}
