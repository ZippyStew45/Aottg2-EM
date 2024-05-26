﻿using Projectiles;
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
            var human = (Human)_owner;
            Vector3 target = human.GetAimPoint();
            Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 2f;
            Vector3 direction = (target - start).normalized;
            ProjectileSpawner.Spawn(ProjectilePrefabs.Flare, start, Quaternion.identity, direction * Speed, Gravity, 6.5f, _owner.Cache.PhotonView.ViewID,
                "", new object[] { _color }, Name == "Flash"); // flash added by Ata 26 May 2024 for flash flare //
            human.PlaySound(HumanSounds.FlareLaunch);
        }
    }
}
