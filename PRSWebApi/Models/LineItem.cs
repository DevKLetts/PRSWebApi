using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PRSWebApi.Models;

[Table("LineItem")]
[Index("RequestID", "ProductID", Name = "req_pdt", IsUnique = true)]
public partial class LineItem
{
    [Key]
    [Column("ID")]
    public int ID { get; set; }

    [Column("RequestId")]
    public int RequestID { get; set; }

    [Column("ProductId")]
    public int ProductID { get; set; }

    public int Quantity { get; set; }

    //[ForeignKey("ProductId")]
    //[InverseProperty("LineItems")]
    [JsonIgnore]
    public virtual Product? Product { get; set; } = null!;

    //[ForeignKey("RequestId")]
    //[InverseProperty("LineItems")]
    [JsonIgnore]
    public virtual Request? Request { get; set; } = null!;
}
