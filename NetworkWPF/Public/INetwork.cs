using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkWPF
{
    public interface INetwork
    {
        public void ProcessData(Package package,User sender);
    }
}
