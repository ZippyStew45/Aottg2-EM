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
            int charViewId, string team, object[] settings = null, int _type = 0)
        {
            GameObject go = PhotonNetwork.Instantiate(ResourcePaths.Projectiles + "/" + name, position, rotation, 0);
            BaseProjectile projectile;
            projectile = go.GetComponent<BaseProjectile>();
            
            Light light = go.GetComponentInChildren<Light>(); // flash added by Ata 26 May 2024 for flash flare //
            if (_type == 1) 
            {
                light.enabled = true;
            }
            if (_type == 2)
            {
                GameObject marker = PhotonNetwork.Instantiate(ResourcePaths.UI + "/Prefabs/InGame/AcousticFlareMarker", position, rotation, 0);
                AcousticFlare _settings = marker.GetComponent<AcousticFlare>();
                _settings.Setup(marker.transform, PhotonNetwork.LocalPlayer);
            }    

            projectile.Setup(liveTime, velocity, gravity, charViewId, team, settings);
            return projectile;
        }
    }
}
