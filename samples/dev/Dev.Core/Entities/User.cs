using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev.Core.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Table("uc_users", Schema = "uc")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Column("real_name")]
        public string? RealName { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("creator_id")]
        public long CreatorId { get; set; }

        [Column("creator_name")]
        public string? CreatorName { get; set; }

        [Column("last_modified_at")]
        public DateTime? LastModifiedAt { get; set; }

        [Column("last_modifier_id")]
        public long? LastModifierId { get; set; }

        [Column("last_modifier_name")]
        public string? LastModifierName { get; set; }
    }
}
