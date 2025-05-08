using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PRSWebApi.Models;

[Table("Request")]
public partial class Request
{
    [Key]
    [Column("ID")]
    public int ID { get; set; }

    [Column("UserID")]
    public int UserID { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string RequestNumber { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Description { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Justification { get; set; } = null!;

    public DateTime DateNeeded { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string DeliveryMode { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SubmittedDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ReasonForRejection { get; set; }

    //[InverseProperty("Request")]
    [JsonIgnore]
    public virtual ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();

    //[ForeignKey("UserId")]
    //[InverseProperty("Requests")]
    [JsonIgnore]
    public virtual User? User { get; set; } = null!;
}
