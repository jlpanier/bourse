using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Views.Main
{
    public class Response
    {
        public int NewShare { get; set;  }

        public int UpdatedShare { get; set; }

        public int NewSharePrice { get; set; }

        public int UpdatedSharePrice { get; set; }
    }
}
