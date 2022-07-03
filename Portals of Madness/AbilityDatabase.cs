namespace Portals_of_Madness
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AbilityDatabase")]
    public partial class AbilityDatabase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public int? cost { get; set; }

        public int? cooldown { get; set; }

        public double? physAttackDamage { get; set; }

        public double? magicAttackDamage { get; set; }

        [StringLength(50)]
        public string damageType { get; set; }

        [StringLength(50)]
        public string target { get; set; }

        public int? duration { get; set; }

        public int? targetCount { get; set; }

        [StringLength(50)]
        public string abilityType { get; set; }

        [StringLength(50)]
        public string modifier { get; set; }

        public double? modifiedAmount { get; set; }

        [StringLength(50)]
        public string imageIcon { get; set; }

        [StringLength(50)]
        public string sprite { get; set; }

        public Ability convToAbility()
        {
            return new Ability(name, (int)cost, (int)cooldown, (double)physAttackDamage, (double)magicAttackDamage,
                (int)duration, damageType, target, (int)targetCount, abilityType, modifier, (double)modifiedAmount,
                imageIcon, sprite);
        }
    }
}
