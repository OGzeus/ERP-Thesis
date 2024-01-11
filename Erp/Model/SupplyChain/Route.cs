using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain
{
    public class Route
    {
        public Vehicle Vehicle { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
    }
}
