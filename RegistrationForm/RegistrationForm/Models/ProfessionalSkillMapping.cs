namespace RegistrationForm.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProfessionalSkillMapping")]
    public partial class ProfessionalSkillMapping
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? SkillId { get; set; }

        public virtual Skill Skill { get; set; }

        public virtual User User { get; set; }
    }
}
