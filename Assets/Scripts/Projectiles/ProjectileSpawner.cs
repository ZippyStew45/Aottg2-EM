using ApplicationManagers;
using GameManagers;
using Photon.Pun;
using System.Collections.Generic;
using UI;
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
            
            // block added by ata for flare wheel //
            if (_type == 1) 
            {
                PhotonView photonView = go.GetComponent<PhotonView>();
                if (photonView.IsMine)
                    photonView.RPC("EnableLight", RpcTarget.AllBuffered, new object[] { go });
            }
            if (_type == 2)
            {
                GameObject marker = PhotonNetwork.Instantiate(ResourcePaths.UI + "/Prefabs/AtasFolder/AcousticFlareMarker", position, rotation, 0);
                AcousticFlare _settings = marker.GetComponent<AcousticFlare>();
                _settings.Setup(marker.transform, PhotonNetwork.LocalPlayer);
            }    
            // block added by ata for flare wheel //

            projectile.Setup(liveTime, velocity, gravity, charViewId, team, settings);
            return projectile;
        }

        [PunRPC]
        private void EnableLight(GameObject go)
        {
            Light light = go.GetComponentInChildren<Light>();
            light.enabled = true;
        }
    }
}
