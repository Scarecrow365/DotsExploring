using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Systems
{
  [UpdateInGroup(typeof(InitializationSystemGroup))]
  public class RandomSystem : SystemBase
  {
    public NativeArray<Random> RandomArray { get; private set; }
    
    protected override void OnCreate()
    {
      var randomArray = new Random[JobsUtility.MaxJobThreadCount];
      var seed = new System.Random(); 
        
      for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
      {
        randomArray[i] = new Random((uint)seed.Next());
      }

      RandomArray = new NativeArray<Random>(randomArray, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnDestroy()
    {
      RandomArray.Dispose();
    }
  }
}