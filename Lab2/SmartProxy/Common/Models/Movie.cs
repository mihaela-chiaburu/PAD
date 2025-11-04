using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Movie : DBdocument
    {
        public String Name { get; set; }
        public List<String> Actors { get; set; }
        public decimal? Budget { get; set; }
        public String Description { get; set; }
    }
}
