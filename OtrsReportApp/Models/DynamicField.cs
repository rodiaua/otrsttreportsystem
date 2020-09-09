using System;
using System.Collections.Generic;

namespace OtrsReportApp.Models
{
    public partial class DynamicField
    {
        public DynamicField()
        {
            DynamicFieldValue = new HashSet<DynamicFieldValue>();
        }

        public int Id { get; set; }
        public short InternalField { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int FieldOrder { get; set; }
        public string FieldType { get; set; }
        public string ObjectType { get; set; }
        public byte[] Config { get; set; }
        public short ValidId { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime ChangeTime { get; set; }
        public int ChangeBy { get; set; }

        public virtual Users ChangeByNavigation { get; set; }
        public virtual Users CreateByNavigation { get; set; }
        public virtual ICollection<DynamicFieldValue> DynamicFieldValue { get; set; }
    }
}
