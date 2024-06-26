using ApplicationManagers;
using Projectiles;
using System.Collections;
using UnityEngine;

namespace Characters
{
    class FlareItem : SimpleUseable
    {
        Color _color;
        float Speed = 150f;
        Vector3 Gravity = Vector3.down * 15f;

        public FlareItem(BaseCharacter owner, string name, Color color, float cooldown): base(owner)
        {
            Name = name;
            _color = color;
            Cooldown = cooldown;
        }

        protected override void Activate()
        {
            int _type = 0;

            if (Name == "Flash")
                _type = 1;
            if (Name == "Acoustic")
                _type = 2;

            var human = (Human)_owner;
            Vector3 target = human.GetAimPoint();
            Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 2f;
            Vector3 direction = (target - start).normalized;

            /*if (_type == 2)
            {
                AcousticFlareController.FireAcousticFlare(start, Quaternion.identity);
                human.PlaySound(HumanSounds.FlareLaunch);
                return;
            }*/

            ProjectileSpawner.Spawn(ProjectilePrefabs.Flare, start, Quaternion.identity, direction * Speed, Gravity, 6.5f, _owner.Cache.PhotonView.ViewID,
                "", new object[] { _color }, _type); // flash added by Ata 26 May 2024 for flash flare //
            human.PlaySound(HumanSounds.FlareLaunch);
        }

        protected override void ActivateFromWheel() // added/modified by Ata 4 June 2024 for Flare Wheel //
        {
            int _type = 0;

            if (Name == "Flash")
                _type = 1;
            if (Name == "Acoustic")
                _type = 2;

            var human = (Human)_owner;
            
            Vector3 target = human.GetAimPoint(human.transform.position, SceneLoader.CurrentCamera.Camera.transform.forward * 30f + Vector3.up * 5f);

            Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 5f;
            Vector3 direction = (target - start).normalized;
            
            /*if (_type == 2)
            {
                AcousticFlareController.FireAcousticFlare(start, Quaternion.identity);
                human.PlaySound(HumanSounds.FlareLaunch);
                return;
            }*/
            
            ProjectileSpawner.Spawn(ProjectilePrefabs.Flare, start, Quaternion.identity, direction * Speed, Gravity, 6.5f, _owner.Cache.PhotonView.ViewID,
                "", new object[] { _color }, _type); // flash added by Ata 26 May 2024 for flash flare //
            human.PlaySound(HumanSounds.FlareLaunch);
        }
    }
}
