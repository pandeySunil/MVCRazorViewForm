namespace RegistrationForm.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Image = new HashSet<Image>();
            ProfessionalSkillMapping = new HashSet<ProfessionalSkillMapping>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string FullName { get; set; }
       
        public DateTime? DOB { get; set; }
        [Required]
        [StringLength(50)]
        public string EmailAddress { get; set; }
        [Required]
        [StringLength(25)]
        public string MobileNo { get; set; }

        public int? StateId { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Image> Image { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfessionalSkillMapping> ProfessionalSkillMapping { get; set; }

        public virtual State State { get; set; }
    }
}
