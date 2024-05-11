using Effects;
using System.Collections;
using UnityEngine;

namespace Characters // added by Ata 2 May 2024 for Air Dodge Human Special //
{
    class AirDodgeSpecial : ExtendedUseable
    {
        protected override float ActiveTime => 0.4f;

        public AirDodgeSpecial(BaseCharacter owner) : base(owner)
        {
            UsesLeft = MaxUses = -1;
        }

        public override bool CanUse()
        {
            return base.CanUse() && ((Human)_owner).State == HumanState.Idle && !((Human)_owner).CanJump();
        }

        protected override void Activate()
        {
            Debug.Log("Air Special Called!");
            //((Human)_owner).DisableHitBox();
            ((Human)_owner).CrossFade(HumanAnimations.AirRelease, 0.1f);
            //((Human)_owner).DisableHumanHitbox();
        }

        protected override void Deactivate()
        {
            ((Human)_owner).PlaySound(HumanSounds.Dodge);
            ((Human)_owner).SpecialActionState(.3f);
            //((Human)_owner).EnableHumanHitbox();
        }
    }
}
