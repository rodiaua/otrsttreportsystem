using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class TicketComment
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    [StringLength(500, ErrorMessage = "The Comment value cannot exceed 500 characters. ")]
    public string Comment { get; set; }
    [Required]
    public string CommentedBy { get; set; }
    [JsonConverter(typeof(EpochConverter))]
    public DateTime? UpdateTime { get; set; } = DateTime.Now;
  }
}
