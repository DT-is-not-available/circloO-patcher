public class CircloOPatch {

    public string Name { get; init; } // aka internal name, aka id
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public Action Callback { get; set; }
    public bool Recommended { get; set; }
    public bool Required { get; set; }

    public CircloOPatch(string Name, string DisplayName, string Description, Action Callback, bool Recommended, bool Required) {
        this.Name = Name;
        this.DisplayName = DisplayName;
        this.Description = Description;
        this.Callback = Callback;
        this.Recommended = Recommended;
        this.Required = Required;
    }

    public void Execute() {
        this.Callback();
    }

}