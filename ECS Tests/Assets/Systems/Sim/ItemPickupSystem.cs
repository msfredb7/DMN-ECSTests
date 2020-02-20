using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(CollisionSystem))]
public class ItemPickupSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        new CollisionReactionJob()
        {
            em = EntityManager
        }
            .Execute(World.GetExistingSystem<CollisionSystem>(), default);
    }

    struct CollisionReactionJob : ICollisionEnterReactionJob
    {
        public EntityManager em;

        public void Execute(in CollisionEnterData collision)
        {
            Entity collectibleEntity = Entity.Null;
            Entity pawnEntity = Entity.Null;

            if (collision.EntityA.Has<PawnTag>(em) && collision.EntityB.Has<CollectibleTag>(em))
            {
                pawnEntity = collision.EntityA;
                collectibleEntity = collision.EntityB;
            }
            else if (collision.EntityB.Has<PawnTag>(em) && collision.EntityA.Has<CollectibleTag>(em))
            {
                pawnEntity = collision.EntityB;
                collectibleEntity = collision.EntityA;
            }

            if (pawnEntity != Entity.Null && collectibleEntity != Entity.Null)
            {
                if (collectibleEntity.Has<ItemReference>(em))
                {
                    ItemReference itemRef = collectibleEntity.Get<ItemReference>(em);

                    if (itemRef.ItemEntity != Entity.Null)
                        ItemSystem.EquipItem(em, itemRef.ItemEntity, pawnEntity);
                }

                em.DestroyEntity(collectibleEntity);
            }
        }
    }
}

public class EffectMoveDistanceBoostOperations
{
    [RegisterItemEquipOperation(typeof(EffectMoveDistanceBoost))]
    public static void OnItemEquipped(EntityManager em, Entity pawn, Entity item)
    {
        pawn.SetValue<MoveDistance>(em, pawn.GetValue<MoveDistance>(em) + item.GetValue<EffectMoveDistanceBoost>(em));
    }

    [RegisterItemUnequipOperation(typeof(EffectMoveDistanceBoost))]
    public static void OnItemUnequipped(EntityManager em, Entity pawn, Entity item)
    {
        pawn.SetValue<MoveDistance>(em, pawn.GetValue<MoveDistance>(em) - item.GetValue<EffectMoveDistanceBoost>(em));
    }
}

public class EffectMoveSpeedBoostOperations
{
    [RegisterItemEquipOperation(typeof(EffectMoveSpeedBoost))]
    public static void OnItemEquipped(EntityManager em, Entity pawn, Entity item)
    {
        pawn.SetValue<MoveSpeed>(em, pawn.GetValue<MoveSpeed>(em) + item.GetValue<EffectMoveSpeedBoost>(em));
    }

    [RegisterItemUnequipOperation(typeof(EffectMoveSpeedBoost))]
    public static void OnItemUnequipped(EntityManager em, Entity pawn, Entity item)
    {
        pawn.SetValue<MoveSpeed>(em, pawn.GetValue<MoveSpeed>(em) - item.GetValue<EffectMoveSpeedBoost>(em));
    }
}

public class RegisterItemEquipOperationAttribute : Attribute
{
    public Type ComponentType;
    public RegisterItemEquipOperationAttribute(Type componentType)
    {
        ComponentType = componentType;
    }
}

public class RegisterItemUnequipOperationAttribute : Attribute
{
    public Type ComponentType;
    public RegisterItemUnequipOperationAttribute(Type componentType)
    {
        ComponentType = componentType;
    }
}

public class ItemSystem
{
    public delegate void ItemOperationDelegate(EntityManager em, Entity pawn, Entity item);

    static readonly Dictionary<Type, ItemOperationDelegate> OnEquipOperations = new Dictionary<Type, ItemOperationDelegate>();
    static readonly Dictionary<Type, ItemOperationDelegate> OnUnequipOperations = new Dictionary<Type, ItemOperationDelegate>();

    static ItemSystem()
    {
        foreach (MethodInfo item in UnityEditor.TypeCache.GetMethodsWithAttribute<RegisterItemEquipOperationAttribute>())
        {
            var registerData = item.GetCustomAttribute<RegisterItemEquipOperationAttribute>();
            OnEquipOperations.Add(registerData.ComponentType, (ItemOperationDelegate)item.CreateDelegate(typeof(ItemOperationDelegate)));
        }
        foreach (MethodInfo item in UnityEditor.TypeCache.GetMethodsWithAttribute<RegisterItemUnequipOperationAttribute>())
        {
            var registerData = item.GetCustomAttribute<RegisterItemUnequipOperationAttribute>();
            OnUnequipOperations.Add(registerData.ComponentType, (ItemOperationDelegate)item.CreateDelegate(typeof(ItemOperationDelegate)));
        }
    }

    public static void EquipItem(EntityManager entityManager, Entity item, Entity pawn)
    {
        if (entityManager.HasComponent<ItemReferenceElement>(pawn))
        {
            // Add item in pawn's inventory
            entityManager.GetBuffer<ItemReferenceElement>(pawn).Add(new ItemReferenceElement() { ItemEntity = item });

            // apply equip effects
            using (NativeArray<ComponentType> itemComponentTypes = entityManager.GetComponentTypes(item, Allocator.Temp))
            {
                foreach (ComponentType componentType in itemComponentTypes)
                {
                    if (OnEquipOperations.TryGetValue(componentType.GetManagedType(), out ItemOperationDelegate operation))
                    {
                        operation.Invoke(entityManager, pawn, item);
                    }
                }
            }
        }
    }
}