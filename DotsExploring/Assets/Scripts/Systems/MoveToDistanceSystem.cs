using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
  public class MoveToDistanceSystem : SystemBase
  {
    protected override void OnUpdate()
    {
      var deltaTime = Time.DeltaTime;

      Entities
        .ForEach((ref Translation translation, ref Rotation rotation, in Destination destination, in MovementSpeed speed) =>
        {
          if (math.all(destination.Value == translation.Value))
            return;

          var toDestination = destination.Value - translation.Value;
          rotation.Value = quaternion.LookRotation(toDestination, new float3(0, 1, 0));

          var movement = math.normalize(toDestination) * speed.Value * deltaTime;

          if (math.length(movement) >= math.length(toDestination))
          {
            translation.Value = destination.Value;
          }
          else
          {
            translation.Value += movement;
          }
        })
        .Schedule();
    }
  }
}