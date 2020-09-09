using System;
using System.Collections.Generic;

namespace OtrsReportApp.Models
{
    public partial class DynamicFieldValue
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public long ObjectId { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public long? ValueInt { get; set; }

        public virtual DynamicField Field { get; set; }
    }
}
