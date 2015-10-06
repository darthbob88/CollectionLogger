namespace MediaModelClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("porn")]
    public partial class porn
    {
        [StringLength(2147483647)]
        public string video_name { get; set; }

        [Key]
        public long video_id { get; set; }
    }
}
