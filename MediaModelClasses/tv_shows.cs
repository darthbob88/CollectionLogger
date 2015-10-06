namespace MediaModelClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tv_shows
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tv_shows()
        {
            tv_episodes = new HashSet<tv_episodes>();
        }

        [Key]
        public long show_ID { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string show_name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tv_episodes> tv_episodes { get; set; }
    }
}
