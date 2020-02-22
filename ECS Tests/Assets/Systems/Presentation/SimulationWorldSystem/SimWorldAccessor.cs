using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class SimWorldAccessor
{
    World _simWorld;
    BeginViewSystem _beginViewSystem;
    ComponentSystem _someSimSystem;

    public SimWorldAccessor(World simWorld, BeginViewSystem beginViewSystem, ComponentSystem someSimSystem)
    {
        _simWorld = simWorld;
        _beginViewSystem = beginViewSystem;
        _someSimSystem = someSimSystem;
    }

    public SimWorldAccessorJob JobAccessor => new SimWorldAccessorJob(_beginViewSystem.ExclusiveSimWorld);

    /// <summary>
    /// Gets an array-like container containing all components of type T, indexed by Entity.
    /// </summary>
    /// <param name="isReadOnly">Whether the data is only read, not written. Access data as
    /// read-only whenever possible.</param>
    /// <typeparam name="T">A struct that implements <see cref="IComponentData"/>.</typeparam>
    /// <returns>All component data of type T.</returns>
    public ComponentDataFromEntity<T> GetComponentDataFromEntity<T>() where T : struct, IComponentData
        => _someSimSystem.GetComponentDataFromEntity<T>(true);

    /// <summary>
    /// Gets a BufferFromEntity&lt;T&gt; object that can access a <seealso cref="DynamicBuffer{T}"/>.
    /// </summary>
    /// <remarks>Assign the returned object to a field of your Job struct so that you can access the
    /// contents of the buffer in a Job.</remarks>
    /// <param name="isReadOnly">Whether the buffer data is only read or is also written. Access data in
    /// a read-only fashion whenever possible.</param>
    /// <typeparam name="T">The type of <see cref="IBufferElementData"/> stored in the buffer.</typeparam>
    /// <returns>An array-like object that provides access to buffers, indexed by <see cref="Entity"/>.</returns>
    /// <seealso cref="ComponentDataFromEntity{T}"/>
    public BufferFromEntity<T> GetBufferFromEntity<T>() where T : struct, IBufferElementData
        => _someSimSystem.GetBufferFromEntity<T>(true);

    /// <summary>
    /// Creates a EntityQuery from an array of component types.
    /// </summary>
    /// <param name="requiredComponents">An array containing the component types.</param>
    /// <returns>The EntityQuery derived from the specified array of component types.</returns>
    /// <seealso cref="EntityQueryDesc"/>
    public EntityQuery CreateEntityQuery(params ComponentType[] requiredComponents)
        => _simWorld.EntityManager.CreateEntityQuery(requiredComponents);

    /// <summary>
    /// Gets the number of shared components managed by this EntityManager.
    /// </summary>
    /// <returns>The shared component count</returns>
    public int GetSharedComponentCount()
        => _simWorld.EntityManager.GetSharedComponentCount();

    /// <summary>
    /// Gets the value of a component for an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of component to retrieve.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
    public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        => _simWorld.EntityManager.GetComponentData<T>(entity);

    /// <summary>
    /// Gets the value of a chunk component.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    /// </remarks>
    /// <param name="chunk">The chunk.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
    public T GetChunkComponentData<T>(ArchetypeChunk chunk) where T : struct, IComponentData
        => _simWorld.EntityManager.GetChunkComponentData<T>(chunk);

    /// <summary>
    /// Gets the value of chunk component for the chunk containing the specified entity.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    public T GetChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        => _simWorld.EntityManager.GetChunkComponentData<T>(entity);

    /// <summary>
    /// Gets the managed [UnityEngine.Component](https://docs.unity3d.com/ScriptReference/Component.html) object
    /// from an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the managed object.</typeparam>
    /// <returns>The managed object, cast to type T.</returns>
    public T GetComponentObject<T>(Entity entity)
        => _simWorld.EntityManager.GetComponentObject<T>(entity);

    public T GetComponentObject<T>(Entity entity, ComponentType componentType)
        => _simWorld.EntityManager.GetComponentObject<T>(entity, componentType);

    /// <summary>
    /// Gets a shared component from an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of shared component.</typeparam>
    /// <returns>A copy of the shared component.</returns>
    public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        => _simWorld.EntityManager.GetSharedComponentData<T>(entity);

    public int GetSharedComponentDataIndex<T>(Entity entity) where T : struct, ISharedComponentData
        => _simWorld.EntityManager.GetSharedComponentDataIndex<T>(entity);

    /// <summary>
    /// Gets a shared component by index.
    /// </summary>
    /// <remarks>
    /// The ECS framework maintains an internal list of unique shared components. You can get the components in this
    /// list, along with their indices using
    /// <see cref="GetAllUniqueSharedComponentData{T}(List{T},List{int})"/>. An
    /// index in the list is valid and points to the same shared component index as long as the shared component
    /// order version from <see cref="GetSharedComponentOrderVersion{T}(T)"/> remains the same.
    /// </remarks>
    /// <param name="sharedComponentIndex">The index of the shared component in the internal shared component
    /// list.</param>
    /// <typeparam name="T">The data type of the shared component.</typeparam>
    /// <returns>A copy of the shared component.</returns>
    public T GetSharedComponentData<T>(int sharedComponentIndex) where T : struct, ISharedComponentData
        => _simWorld.EntityManager.GetSharedComponentData<T>(sharedComponentIndex);

    /// <summary>
    /// Gets a list of all the unique instances of a shared component type.
    /// </summary>
    /// <remarks>
    /// All entities with the same archetype and the same values for a shared component are stored in the same set
    /// of chunks. This function finds the unique shared components existing across chunks and archetype and
    /// fills a list with copies of those components.
    /// </remarks>
    /// <param name="sharedComponentValues">A List<T> object to receive the unique instances of the
    /// shared component of type T.</param>
    /// <typeparam name="T">The type of shared component.</typeparam>
    public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues) where T : struct, ISharedComponentData
        => _simWorld.EntityManager.GetAllUniqueSharedComponentData<T>(sharedComponentValues);

    /// <summary>
    /// Gets a list of all unique shared components of the same type and a corresponding list of indices into the
    /// internal shared component list.
    /// </summary>
    /// <remarks>
    /// All entities with the same archetype and the same values for a shared component are stored in the same set
    /// of chunks. This function finds the unique shared components existing across chunks and archetype and
    /// fills a list with copies of those components and fills in a separate list with the indices of those components
    /// in the internal shared component list. You can use the indices to ask the same shared components directly
    /// by calling <see cref="GetSharedComponentData{T}(int)"/>, passing in the index. An index remains valid until
    /// the shared component order version changes. Check this version using
    /// <see cref="GetSharedComponentOrderVersion{T}(T)"/>.
    /// </remarks>
    /// <param name="sharedComponentValues"></param>
    /// <param name="sharedComponentIndices"></param>
    /// <typeparam name="T"></typeparam>
    public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues, List<int> sharedComponentIndices) where T : struct, ISharedComponentData
        => _simWorld.EntityManager.GetAllUniqueSharedComponentData<T>(sharedComponentValues, sharedComponentIndices);

    /// <summary>
    /// Gets the dynamic buffer of an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the buffer's elements.</typeparam>
    /// <returns>The DynamicBuffer object for accessing the buffer contents.</returns>
    /// <exception cref="ArgumentException">Thrown if T is an unsupported type.</exception>
    public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        => _simWorld.EntityManager.GetBuffer<T>(entity);

    /// <summary>
    /// Gets the chunk in which the specified entity is stored.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The chunk containing the entity.</returns>
    public ArchetypeChunk GetChunk(Entity entity)
        => _simWorld.EntityManager.GetChunk(entity);

    /// <summary>
    /// Gets the number of component types associated with an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The number of components.</returns>
    public int GetComponentCount(Entity entity)
        => _simWorld.EntityManager.GetComponentCount(entity);

    /// <summary>
    /// Returns false if the component has the 'Disabled' component. Disabled entities are excluded from entity queries by default
    /// </summary>
    public bool GetEnabled(Entity entity)
        => _simWorld.EntityManager.GetEnabled(entity);
}

public struct SimWorldAccessorJob
{
    [ReadOnly] ExclusiveEntityTransaction _exclusiveTransaction;

    public SimWorldAccessorJob(ExclusiveEntityTransaction exclusiveEntityTransaction)
    {
        _exclusiveTransaction = exclusiveEntityTransaction;
    }

    public bool Exists(Entity entity)
        => _exclusiveTransaction.Exists(entity);

    public bool HasComponent(Entity entity, ComponentType type)
        => _exclusiveTransaction.HasComponent(entity, type);

    public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        => _exclusiveTransaction.GetComponentData<T>(entity);

    public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        => _exclusiveTransaction.GetSharedComponentData<T>(entity);

    public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        => _exclusiveTransaction.GetBuffer<T>(entity);
}