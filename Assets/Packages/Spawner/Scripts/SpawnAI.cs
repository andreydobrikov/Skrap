///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnAI.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 22 Nov 2012
//
// Copyright (c) 2012 Garth de Wet
// 
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using CorruptedSmileStudio.Spawner;
using UnityEngine;

/// <summary>
/// SpawnAI component. Provides the base methods required to interact with the spawner.
/// </summary>
/// <description>
/// This component can be used out of the box on existing GameObjects that get spawned.<br />
/// This class interacts with the Spawner and handles all interaction between the two. It also makes
/// use of InstanceManager in order to work with Pool Manager by Path-o-Logical (if it is installed).<br />
/// It is highly suggested to add this component to GameObjects you wish to spawn, simply interact with this class
/// in your killing method and call Remove() on this component last in the method,
/// you will not need to call Destroy(gameObject) in order to destroy the GameObject as this class handles all of that.
/// </description>
public class SpawnAI : MonoBehaviour
{
    /// <summary>
    /// If the unit is currently alive or not.
    /// </summary>
    private bool alive = false;
    /// <summary>
    /// The spawner that owns this unit.
    /// </summary>
    private Spawner owner;
    /// <summary>
    /// Call this method to remove the unit.
    /// You will need to use tags to send a message to all spawner objects with the ID of the spawner
    /// </summary>
    public void Remove()
    {
        if (alive)
        {
            owner.KillUnit();
            InstanceManager.Despawn(transform);
            alive = false;
        }
    }
    /// <summary>
    /// Sets the Spawner that owns this unit.
    /// </summary>
    /// <param name="owner">The spawner that spawned this unit.</param>
    public void SetOwner(Spawner owner)
    {
        this.owner = owner;
        alive = true;
    }
}