using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{

    /// <summary>
    /// A simple data transfer object (DTO) that contains raw data about a person.
    /// </summary>
    public class TVItem
    {
        readonly List<TVItem> _children = new List<TVItem>();
        public IList<TVItem> Children
        {
            get { return _children; }
        }

        public string Name { get; set; }
    }
}
