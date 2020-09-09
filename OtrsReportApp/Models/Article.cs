using System;
using System.Collections.Generic;

namespace OtrsReportApp.Models
{
    public partial class Article
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public short ArticleTypeId { get; set; }
        public short ArticleSenderTypeId { get; set; }
        public string AFrom { get; set; }
        public string AReplyTo { get; set; }
        public string ATo { get; set; }
        public string ACc { get; set; }
        public string ASubject { get; set; }
        public string AMessageId { get; set; }
        public string AMessageIdMd5 { get; set; }
        public string AInReplyTo { get; set; }
        public string AReferences { get; set; }
        public string AContentType { get; set; }
        public string ABody { get; set; }
        public int IncomingTime { get; set; }
        public string ContentPath { get; set; }
        public short ValidId { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime ChangeTime { get; set; }
        public int ChangeBy { get; set; }

        public virtual Users ChangeByNavigation { get; set; }
        public virtual Users CreateByNavigation { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
