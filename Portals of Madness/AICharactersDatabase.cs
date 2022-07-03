namespace Portals_of_Madness
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AICharactersDatabase")]
    public partial class AICharactersDatabase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string imageSet { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public int? level { get; set; }

        [StringLength(50)]
        public string type { get; set; }

        public double? baseHealth { get; set; }

        public double? healthMult { get; set; }

        [StringLength(10)]
        public string resourceName { get; set; }

        public int? maxResource { get; set; }

        public double? basePhysAttack { get; set; }

        public double? physAttackMult { get; set; }

        public double? baseMagicAttack { get; set; }

        public double? magicAttackMult { get; set; }

        public double? basePhysArmor { get; set; }

        public double? physArmorMult { get; set; }

        public double? baseMagicArmor { get; set; }

        public double? magicArmorMult { get; set; }

        [StringLength(50)]
        public string weaknesses { get; set; }

        [StringLength(50)]
        public string ability1Name { get; set; }

        [StringLength(50)]
        public string ability2Name { get; set; }

        [StringLength(50)]
        public string ability3Name { get; set; }

        public int? baseSpeed { get; set; }

        [StringLength(50)]
        public string aiName { get; set; }

        public AICharacter convToAICharacter()
        {
            return new AICharacter(imageSet, (int)level, name, (double)baseHealth, (double)healthMult, resourceName,
                (int)maxResource, (double)basePhysAttack, (double)physAttackMult, (double)baseMagicAttack,
                (double)magicAttackMult, (double)basePhysAttack, (double)physAttackMult, (double)baseMagicArmor,
                (double)magicArmorMult, weaknesses, ability1Name, ability2Name, ability3Name, (int)baseSpeed, aiName);
        }
    }
}
