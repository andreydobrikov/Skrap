///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: Spawner.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 22 Nov 2012
//
// Copyright (c) 2012 Garth de Wet
// 
///////////////////////////////////////////////////////////////////////////////////////////////////////////////
using CorruptedSmileStudio.Spawner;
using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns prefabs, either in waves, at once or continually till all enemies are spawned.
/// </summary>
/// <description>
/// Controls the spawning of selected perfabs, useful for making enemy spawn points.<br />
/// It supports a variety of spawn modes, which allows you to bend the system to fit your needs.<br />
/// This class is required for the system to work, you will need to place this class on a GameObject with
/// a tag of Spawner (However, this is changeable within the SpawnAI class).
/// </description>
[AddComponentMenu("Spawner")]
public class Spawner : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The unit level to spawn.
    /// </summary>
    public UnitLevels unitLevel = UnitLevels.Easy;
    /// <summary>
    /// The list of units to spawn.
    /// </summary>
    public SpawnableClass[] unitList = new SpawnableClass[4];
    /// <summary>
    /// The total number of enemies to spawn on each spawn session.
    /// </summary>
    public int totalUnitsToSpawn = 10;
    /// <summary>
    /// The current number of spawned enemies.
    /// </summary>
    private int numberOfSpawnedUnits = 0;
    /// <summary>
    /// The total number of spawned enemies. Not just alive.
    /// </summary>
    private int totalSpawnedUnits = 0;
    /// <summary>
    /// The ID of the spawner.
    /// </summary>
    public int spawnID = 0;
    /// <summary>
    /// Used to determine if there is actual spawning to occur.
    /// </summary>
    private bool waveSpawn = true;
    /// <summary>
    /// Determines whether the spawn should spawn.
    /// </summary>
    public bool spawn = true;
    /// <summary>
    /// The type of spawning.
    /// </summary>
    public SpawnModes spawnType = SpawnModes.Normal;
    /// <summary>
    /// The time between each wave when spawn type is set to wave.
    /// </summary>
    public float waveTimer = 30.0f;
    /// <summary>
    /// The time the last wave was spawned
    /// </summary>
    private float lastWaveSpawnTime = 0.0f;
    /// <summary>
    /// The total number of waves to spawn.
    /// </summary>
    public int totalWavesToSpawn = 5;
    /// <summary>
    /// The number of waves that has spawned.
    /// </summary>
    private int numberOfWavesSpawned = 0;
    /// <summary>
    /// Used within the TimeSplitWave.
    /// </summary>
    private bool checkEnemyLevel = false;
    /// <summary>
    /// The time between each spawn of a unit
    /// </summary>
    public float timeBetweenSpawns = 0.5f;
    /// <summary>
    /// The location of where to spawn units.
    /// </summary>
    public Transform[] spawnLocations;
    #endregion

    void Awake()
    {
        if (spawnLocations.Length == 0)
        {
            spawnLocations = new Transform[1];
            spawnLocations[0] = (transform);
        }
    }

    void Start()
    {
        foreach (GameObject go in unitList[(int)unitLevel].units)
        {
            InstanceManager.ReadyPreSpawn(go.transform, (totalUnitsToSpawn / unitList[(int)unitLevel].units.Length));
        }
        StartCoroutine("DoSpawn");
    }
    /// <summary>
    /// Spawns a unit based on the level set.
    /// </summary>
    private void SpawnUnit()
    {
        int unitToSpawn = Random.Range(0, unitList[(int)unitLevel].units.Length);
        if (unitList[(int)unitLevel].units[unitToSpawn] != null)
        {
            int locationToSpawn = Random.Range(0, spawnLocations.Length);
            Transform unit = InstanceManager.Spawn(unitList[(int)unitLevel].units[unitToSpawn].transform, spawnLocations[locationToSpawn].position, Quaternion.identity);
            unit.SendMessage("SetOwner", this);
            // Increase the total number of enemies spawned and the number of spawned enemies
            numberOfSpawnedUnits++;
            totalSpawnedUnits++;
        }
        else
        {
            Debug.LogError("Error trying to spawn unit of level " + unitLevel.ToString() + " on spawner " + spawnID + " - No unit set");
            if (unitList[(int)unitLevel].units.Length == 1)
                spawn = false;
        }
    }
    /// <summary>
    /// Enable the spawner by ID
    /// </summary>
    /// <param name="sID">The spawner's ID</param>
    public void EnableSpawner(int sID)
    {
        if (spawnID == sID)
        {
            spawn = true;
            foreach (GameObject go in unitList[(int)unitLevel].units)
            {
                InstanceManager.ReadyPreSpawn(go.transform, (totalUnitsToSpawn / unitList[(int)unitLevel].units.Length));
            }
            StartCoroutine("DoSpawn");
        }
    }
    /// <summary>
    /// Disable the spawner by ID
    /// </summary>
    /// <param name="sID">The spawner's ID</param>
    public void DisableSpawner(int sID)
    {
        if (spawnID == sID)
        {
            spawn = false;
            StopCoroutine("DoSpawn");
        }
    }
    /// <summary>
    /// The time till the next wave
    /// </summary>
    public float TimeTillWave
    {
        get
        {
            if (numberOfWavesSpawned >= totalWavesToSpawn)
            {
                return 0;
            }
            if (spawnType == SpawnModes.TimeSplitWave && waveSpawn || numberOfSpawnedUnits > 0)
            {
                return 0;
            }

            float time = (lastWaveSpawnTime + waveTimer) - Time.time;
            if (time >= 0)
                return time;
            else
                return 0;
        }
    }
    /// <summary>
    /// Enables the spawner. Useful for triggers.
    /// </summary>
    public void EnableTrigger()
    {
        spawn = true;
        StartCoroutine("DoSpawn");
    }
    /// <summary>
    /// Disables the spawner. Useful for triggers.
    /// </summary>
    public void DisableTrigger()
    {
        spawn = false;
    }
    /// <summary>
    /// Resets all the private variables to their original state.
    /// </summary>
    public void Reset()
    {
        waveSpawn = false;
        checkEnemyLevel = true;
        numberOfWavesSpawned = 0;
        totalSpawnedUnits = 0;
        lastWaveSpawnTime = Time.time;
    }
    /// <summary>
    /// Returns the number of waves left
    /// </summary>
    public int WavesLeft
    {
        get
        {
            return totalWavesToSpawn - numberOfWavesSpawned;
        }
    }
    /// <summary>
    /// Controls the spawning of units and so forth.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoSpawn()
    {
        while (spawn)
        {
            switch (spawnType)
            {
                case SpawnModes.Normal:
                    if (numberOfSpawnedUnits < totalUnitsToSpawn)
                    {
                        yield return new WaitForSeconds(timeBetweenSpawns);
                        SpawnUnit();
                    }
                    break;
                case SpawnModes.Once:
                    if (totalSpawnedUnits >= totalUnitsToSpawn)
                    {
                        spawn = false;
                    }
                    else
                    {
                        yield return new WaitForSeconds(timeBetweenSpawns);
                        SpawnUnit();
                    }
                    break;
                case SpawnModes.Wave:
                    if (numberOfWavesSpawned <= totalWavesToSpawn)
                    {
                        if (waveSpawn)
                        {
                            yield return new WaitForSeconds(timeBetweenSpawns);
                            SpawnUnit();
                            checkEnemyLevel = true;

                            if ((totalSpawnedUnits / (numberOfWavesSpawned + 1)) == totalUnitsToSpawn)
                            {
                                waveSpawn = false;
                            }
                        }
                        if (checkEnemyLevel)
                        {
                            if (numberOfSpawnedUnits <= 0)
                            {
                                checkEnemyLevel = false;
                                waveSpawn = true;
                                numberOfSpawnedUnits = 0;
                                numberOfWavesSpawned++;
                            }
                        }
                    }
                    else
                    {
                        spawn = false;
                    }
                    break;
                case SpawnModes.TimedWave:
                    if (numberOfWavesSpawned <= totalWavesToSpawn)
                    {
                        if (waveSpawn)
                        {
                            yield return new WaitForSeconds(timeBetweenSpawns);
                            SpawnUnit();

                            if ((totalSpawnedUnits / (numberOfWavesSpawned + 1)) == totalUnitsToSpawn)
                            {
                                waveSpawn = false;
                            }
                        }
                        else
                        {
                            waveSpawn = true;
                            numberOfWavesSpawned++;
                            // A hack to spawn even if there are unit left alive.
                            numberOfSpawnedUnits = 0;
                            lastWaveSpawnTime = Time.time;
                            yield return new WaitForSeconds(waveTimer);
                        }
                    }
                    else
                    {
                        spawn = false;
                    }
                    break;
                case SpawnModes.TimeSplitWave:
                    if (numberOfWavesSpawned <= totalWavesToSpawn)
                    {
                        if (waveSpawn)
                        {
                            yield return new WaitForSeconds(timeBetweenSpawns);
                            SpawnUnit();
                            if ((totalSpawnedUnits / (numberOfWavesSpawned + 1)) == totalUnitsToSpawn)
                            {
                                waveSpawn = false;
                                checkEnemyLevel = true;
                            }
                        }
                        else
                        {
                            if (checkEnemyLevel)
                            {
                                if (numberOfSpawnedUnits <= 0)
                                {
                                    numberOfWavesSpawned++;
									totalUnitsToSpawn += 5;
                                    checkEnemyLevel = false;
                                    numberOfSpawnedUnits = 0;
                                    lastWaveSpawnTime = Time.time;
                                    yield return new WaitForSeconds(waveTimer);
                                    waveSpawn = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        spawn = false;
                    }
                    break;
                default:
                    spawn = false;
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    /// <summary>
    /// Starts spawning, if you want to interact with a spawner from code.
    /// </summary>
    public void StartSpawn()
    {
        EnableTrigger();
    }
    /// <summary>
    /// This removes an "unit" in order to allow waves and such that depend on the number of enemies decreasing
    /// to properly start a new spawn.
    /// </summary>
    public void KillUnit()
    {
        numberOfSpawnedUnits--;
    }
}