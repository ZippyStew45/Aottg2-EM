using ApplicationManagers;
using GameManagers;
using Photon.Pun;
using Settings;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utility;

namespace Projectiles
{
    class ProjectileSpawner: MonoBehaviour // this entire thing is made in to an if/else conditioning for Flare Wheel by Ata, 4 June 2024 //
    {
        public static BaseProjectile Spawn(string name, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 gravity, float liveTime,
            int charViewId, string team, object[] settings = null, int _type = 0)
        {
            if (_type == 0) {
                GameObject go = PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/" + name, position, rotation, 0);
                BaseProjectile projectile;
                projectile = go.GetComponent<BaseProjectile>();

                projectile.Setup(liveTime, velocity, gravity, charViewId, team, settings);
                return projectile;
            }
            else if (_type == 1) 
            {
                GameObject go = PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/FlashFlare", position, rotation, 0);
                BaseProjectile projectile;
                projectile = go.GetComponent<BaseProjectile>();
                projectile.EnableLensFlare();

                projectile.Setup(liveTime, velocity, gravity, charViewId, team, settings);
                return projectile;
            }
            else
            {
                GameObject go = PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/" + name, position, rotation, 0);
                BaseProjectile projectile;
                projectile = go.GetComponent<BaseProjectile>();

                projectile.Setup(liveTime, velocity, gravity, charViewId, team, settings);
                return projectile;
            }
        }
    }
}
