namespace PJH.EquipmentSkillSystem
{
    public class BowPassiveSkill : EquipmentSkill
    {
        public int maxPenetrationsCount = 1;
        public int reduceDamagePercent = 50;

        protected override void UpgradeSkill()
        {
            reduceDamagePercent -= 10;
        }

        public override void UseSkill(bool isHolding)
        {
        }
    }
}