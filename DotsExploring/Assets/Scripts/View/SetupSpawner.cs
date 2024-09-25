using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View
{
  public class SetupSpawner : MonoBehaviour
  {
    [SerializeField] private GameObject personPrefab;
    [SerializeField] private int gridSize;
    [SerializeField] private int spread;
    [SerializeField] private Vector2 speedRange;

    private BlobAssetStore blobAssetStore;

    private void Start()
    {
      blobAssetStore = new BlobAssetStore();

      var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
      var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(personPrefab, settings);
      var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

      Init(entityManager, entity);
    }

    private void Init(EntityManager entityManager, Entity entity)
    {
      for (int x = 0; x < gridSize; x++)
      {
        for (int z = 0; z < gridSize; z++)
        {
          var instance = entityManager.Instantiate(entity);
          SetDestination(x, z, entityManager, instance);
          SetTranslation(x, z, entityManager, instance);
          SetSpeed(entityManager, instance);
        }
      }
    }

    private void SetDestination(int x, int z, EntityManager entityManager, Entity instance)
    {
      var position = new float3(x * spread, 0, z * spread);
      entityManager.SetComponentData(instance, new Destination { Value = position });
    }
    
    private void SetTranslation(int x, int z, EntityManager entityManager, Entity instance)
    {
      var position = new float3(x * spread, 0, z * spread);
      entityManager.SetComponentData(instance, new Translation { Value = position });
    }

    private void SetSpeed(EntityManager entityManager, Entity instance)
    {
      var speed = Random.Range(speedRange.x, speedRange.y);
      entityManager.SetComponentData(instance, new MovementSpeed { Value = speed });
    }

    private void OnDestroy()
    {
      blobAssetStore.Dispose();
    }
  }
}