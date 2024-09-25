using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
  public class DestinationSystem : SystemBase
  {
    private RandomSystem randomSystem;
    
    protected override void OnCreate()
    {
      randomSystem = World.GetExistingSystem<RandomSystem>();
    }

    protected override void OnUpdate()
    {
      var randomArray = randomSystem.RandomArray;

      Entities
        .WithNativeDisableParallelForRestriction(randomArray)
        .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) =>
        {
          var distance = math.abs(math.length(destination.Value - translation.Value));

          if (distance > 0.1f) return;
          
          var random = randomArray[nativeThreadIndex];

          destination.Value.x = random.NextFloat(-50,50);
          destination.Value.z = random.NextFloat(-50,50);

          randomArray[nativeThreadIndex] = random;
        })
        .ScheduleParallel();
    }
  }
}