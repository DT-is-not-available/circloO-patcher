using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpatcher
{
    public class CircloOPatch
    {
        public Guid Id { get; init; }
        public string Name { get; init; } // aka internal name, init makes it so it cant be changes afterwards
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public Action Callback { get; set; }
        public bool Required { get; set; }

        public CircloOPatch(string Name, string DisplayName, string Description, Action Callback, bool Required = false)
        {
            this.Id = Guid.NewGuid();
            this.Name = Name;
            this.DisplayName = DisplayName;
            this.Description = Description;
            this.Callback = Callback;
            this.Required = Required;
        }

        public void Execute()
        {
            this.Callback(); // pass some additional arguments in like data and helper functions
        }
    }
}
