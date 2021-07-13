using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApp.models
{
    public class ItemData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool done { get; set; }
    }
}
