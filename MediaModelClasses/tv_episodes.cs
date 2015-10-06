namespace MediaModelClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tv_episodes
    {
        public long show_id { get; set; }

        [Key]
        public long episode_ID { get; set; }

        [StringLength(2147483647)]
        public string episode_name { get; set; }

        public virtual tv_shows tv_shows { get; set; }
    }
}
