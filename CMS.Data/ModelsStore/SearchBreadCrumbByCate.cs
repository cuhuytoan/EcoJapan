using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Data.ModelsStore
{
    public class SearchBreadCrumbByCate
    {
        public Nullable<long> Position { get; set; }
        public Nullable<int> Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string FullURL { get; set; }        
    }
}
