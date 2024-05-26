using ApplicationManagers;
using GameManagers;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Projectiles
{
    class ProjectileSpawner: MonoBehaviour
    {
        public static BaseProjectile Spawn(string name, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 gravity, float liveTime,
            int charViewId, string team, object[] settings = null, bool flash = false)
        {
            GameObject go = PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/" + name, position, rotation, 0);
            BaseProjectile projectile;
            projectile = go.GetComponent<BaseProjectile>();
            
            Light light = go.GetComponentInChildren<Light>(); // flash added by Ata 26 May 2024 for flash flare //
            if (flash) 
            {
                light.enabled = true;
            }
            else 
            {
                light.enabled = false;
            }
            

            projectile.Setup(liveTime, velocity, gravity, charViewId, team, settings);
            return projectile;
        }
    }
}
