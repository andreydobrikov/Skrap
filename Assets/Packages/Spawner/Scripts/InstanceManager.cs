#define POOLMANAGER_INSTALLED
#define USINGPHOTON
///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: InstanceManager.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 09 Feb 2012
//
// Copyright (c) 2012 Garth de Wet
// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace CorruptedSmileStudio.Spawner
{
    using UnityEngine;
    /// <summary>
    /// Contains methods which interface with Pool Manager by Path-o-Logical
    /// This will ensure that there is less Instantiating and Destroying being called which will
    /// increase performance.
    /// </summary>
    public static class InstanceManager
    {
        /// <summary>
        /// The pool name under which to spawn the various units.
        /// </summary>
        public static string poolName = "SpawnedEnemy";
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="prefab">The prefab to spawn an instance from</param>
        /// <param name="pos">The position to spawn the instance</param>
        /// <param name="rot">The rotation of the new instance</param>
        /// <returns></returns>
        public static Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
        {
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            // If the pool doesn't exist, it will be created before use
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            return PoolManager.Pools[poolName].Spawn(prefab, pos, rot);
#elif USINGPHOTON
			GameObject GO = (GameObject)PhotonNetwork.Instantiate(prefab.name, pos, rot, 0);
			return GO.transform;
#else
            return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
		}
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="prefab">The prefab to spawn an instance from</param>
        /// <param name="pos">The position to spawn the instance</param>
        /// <param name="rot">The rotation of the new instance</param>
        /// <param name="spawnerID">The ID of the spawner</param>
        /// <returns></returns>
        public static Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, int spawnerID)
		{
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            string name = string.Format("{0}-{1}", poolName, spawnerID.ToString());
            // If the pool doesn't exist, it will be created before use
            if (!PoolManager.Pools.ContainsKey(name))
                (new GameObject(name)).AddComponent<SpawnPool>();

            return PoolManager.Pools[name].Spawn(prefab, pos, rot);
#elif USINGPHOTON
			GameObject GO = (GameObject)PhotonNetwork.Instantiate(prefab.name, pos, rot, 0);
			return GO.transform;
#else
			return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
        }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="prefab">The prefab to spawn an instance from</param>
        /// <param name="pos">The position to spawn the instance</param>
        /// <param name="rot">The rotation of the new instance</param>
        /// <param name="poolName">The name of the pool.</param>
        /// <returns></returns>
        public static Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, string poolName)
		{
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            // If the pool doesn't exist, it will be created before use
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            return PoolManager.Pools[poolName].Spawn(prefab, pos, rot);
#elif USINGPHOTON
			GameObject GO = (GameObject)PhotonNetwork.Instantiate(prefab.name, pos, rot, 0);
			return GO.transform;
#else
			return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
		}

        /// <summary>
        /// Despawns an instance.
        /// </summary>
        public static void Despawn(Transform instance)
		{
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            PoolManager.Pools[poolName].Despawn(instance);
#elif USINGPHOTON
			PhotonNetwork.Destroy(instance.gameObject);
#else
			Object.Destroy(instance.gameObject);
#endif
		}
        /// <summary>
        /// Despawns an instance.
        /// </summary>
        /// <param name="instance">The instance you want despawned</param>
        /// <param name="spawnerID">The SpawnerID</param>
        public static void Despawn(Transform instance, int spawnerID)
		{
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            PoolManager.Pools[string.Format("{0}-{1}", poolName, spawnerID.ToString())].Despawn(instance);
#elif USINGPHOTON
			PhotonNetwork.Destroy(instance.gameObject);
#else
			Object.Destroy(instance.gameObject);
#endif
		}
        /// <summary>
        /// Despawns an instance.
        /// </summary>
        /// <param name="instance">The instance you want despawned</param>
        /// <param name="poolName">The name of the Pool</param>
        public static void Despawn(Transform instance, string poolName)
		{
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            PoolManager.Pools[poolName].Despawn(instance);
#elif USINGPHOTON
			PhotonNetwork.Destroy(instance.gameObject);
#else
			Object.Destroy(instance.gameObject);
#endif
		}
        /// <summary>
        /// This is used by Spawner.cs, You should never need to access this.
        /// This is only used when Pool Manager is present.
        /// </summary>
        /// <param name="prefab">The Prefab to start pooling.</param>
        /// <param name="amount">The number of prefabs to pool.</param>
        public static void ReadyPreSpawn(Transform prefab, int amount)
		{
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            if (!PoolManager.Pools[poolName].prefabs.ContainsKey(prefab.name))
            {
                PrefabPool item = new PrefabPool(prefab);
                item.preloadAmount = amount;

                PoolManager.Pools[poolName].CreatePrefabPool(item);
            }
#elif USINGPHOTON

#endif
		}
        /// <summary>
        /// This is used by Spawner.cs, You should never need to access this.
        /// This is only used when Pool Manager is present.
        /// </summary>
        /// <param name="prefab">The Prefab to start pooling.</param>
        /// <param name="amount">The number of prefabs to pool.</param>
        /// <param name="spawnerID">The ID of the spawner</param>
        public static void ReadyPreSpawn(Transform prefab, int amount, int spawnerID)
        {
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            string name = string.Format("{0}-{1}", poolName, spawnerID.ToString());
            if (!PoolManager.Pools.ContainsKey(name))
                (new GameObject(name)).AddComponent<SpawnPool>();

            if (!PoolManager.Pools[name].prefabs.ContainsKey(prefab.name))
            {
                PrefabPool item = new PrefabPool(prefab);
                item.preloadAmount = amount;

                PoolManager.Pools[name].CreatePrefabPool(item);
            }
#elif USINGPHOTON

#endif
		}
        /// <summary>
        /// This is used by Spawner.cs, You should never need to access this.
        /// This is only used when Pool Manager is present.
        /// </summary>
        /// <param name="prefab">The Prefab to start pooling.</param>
        /// <param name="amount">The number of prefabs to pool.</param>
        /// <param name="poolName">The poolname to be spawned into.</param>
        public static void ReadyPreSpawn(Transform prefab, int amount, string poolName)
        {
#if POOLMANAGER_INSTALLED && !USINGPHOTON
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            if (!PoolManager.Pools[poolName].prefabs.ContainsKey(prefab.name))
            {
                PrefabPool item = new PrefabPool(prefab);
                item.preloadAmount = amount;

                PoolManager.Pools[poolName].CreatePrefabPool(item);
            }
#elif USINGPHOTON

#endif
		}
    }
}