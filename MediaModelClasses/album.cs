namespace MediaModelClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("album")]
    public partial class album
    {
        public long artist_ID { get; set; }

        [Key]
        public long album_ID { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string album_title { get; set; }

        public virtual artist artist { get; set; }
    }
}
