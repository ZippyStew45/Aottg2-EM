using Settings;
using UnityEngine;

namespace Characters
{
    /// <summary>
    /// A simple useable that is only triggered on button down and performs all logic on activation.
    /// </summary>
    abstract class SimpleUseable: BaseUseable
    {
        public SimpleUseable(BaseCharacter owner): base(owner)
        {
        }

        public override void ReadInput(KeybindSetting keybind)
        {
            SetInput(keybind.GetKeyDown());
        }

        public override void SetInput(bool key, bool fromWheel = false)// added/modified by Ata 4 June 2024 for Flare Wheel //
        {
            if (key && CanUse())
            {
                Debug.Log("Cooldown: " + Cooldown);
                if (!fromWheel) // added/modified by Ata 4 June 2024 for Flare Wheel //
                    Activate();
                else
                    ActivateFromWheel();
                OnUse();
            }
        }

        protected override void ActivateFromWheel() // added/modified by Ata 4 June 2024 for Flare Wheel //
        {
        }
    }
}
