//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OICAR19_API.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SCHEDULE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SCHEDULE()
        {
            this.SCHEDULE_IMAGE = new HashSet<SCHEDULE_IMAGE>();
        }
        [JsonProperty(Order = -7)]
        public int IDSCHEDULE { get; set; }
        [JsonProperty(Order = -6)]
        [Column(TypeName = "Date")]
        public System.DateTime SCHEDULE_DATE { get; set; }
        public Nullable<int> PROFILEID { get; set; }
    
        [JsonIgnore]
        public virtual PROFILE PROFILE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCHEDULE_IMAGE> SCHEDULE_IMAGE { get; set; }
    }
}
