using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PRSWebApi.Models;

[Table("Product")]
[Index("VendorID", "PartNumber", Name = "vendor_part", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ID")]
    public int ID { get; set; }

    [Column("VendorID")]
    public int VendorID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PartNumber { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Unit { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? PhotoPath { get; set; }

    //[InverseProperty("Product")]
    [JsonIgnore]
    public virtual ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();

    //[ForeignKey("VendorId")]
    //[InverseProperty("Products")]
    //[JsonIgnore]
    //public virtual Vendor Vendor { get; set; } = null!;
}
