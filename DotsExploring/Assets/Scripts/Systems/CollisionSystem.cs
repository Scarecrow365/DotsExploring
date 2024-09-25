using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

namespace Systems
{
  public class CollisionSystem : SystemBase
  {
    private StepPhysicsWorld stepPhysicsWorld;
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate()
    {
      stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
      buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
      Dependency = new CollisionJob()
        {
          PersonGroup = GetComponentDataFromEntity<PersonTag>(true),
          ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
          Seed = DateTimeOffset.Now.Millisecond
        }
        .Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);
    }

    private struct CollisionJob : ITriggerEventsJob
    {
      [ReadOnly] public ComponentDataFromEntity<PersonTag> PersonGroup;
      
      public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup; 
      public float Seed;
      
      public void Execute(TriggerEvent triggerEvent)
      {
        bool isEntityAPerson = PersonGroup.HasComponent(triggerEvent.EntityA);
        bool isEntityBPerson = PersonGroup.HasComponent(triggerEvent.EntityB);

        if (!isEntityAPerson || !isEntityBPerson) return;

        var random = new Random((uint)((1 + Seed) + (triggerEvent.BodyIndexA * triggerEvent.BodyIndexB)));
        random = ChangeMaterialColor(random, triggerEvent.EntityA);
        ChangeMaterialColor(random, triggerEvent.EntityB);
      }

      private Random ChangeMaterialColor(Random random, Entity entity)
      {
        var colorComponent = ColorGroup[entity];
        colorComponent.Value.x = random.NextFloat(0, 1);
        colorComponent.Value.y = random.NextFloat(0, 1);
        colorComponent.Value.z = random.NextFloat(0, 1);
        ColorGroup[entity] = colorComponent;

        return random;
      }
    }
  }
}