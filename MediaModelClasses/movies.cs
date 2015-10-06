namespace MediaModelClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class movies
    {
        [Required]
        [StringLength(2147483647)]
        public string movie_name { get; set; }

        [Key]
        public long movie_ID { get; set; }
    }
}
