using Fralle.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Fralle.PingTap.Benchmark
{
  [CreateAssetMenu(menuName = "Benchmark/AI Combat Testing")]
  public class AICombatTesting : BenchmarkEvent
  {
    public int wave1EnemyCount = 10;
    public int wave2EnemyCount = 25;
    public int wave3EnemyCount = 50;
    public int wave4EnemyCount = 100;
    public float timeBetweenWaves = 30;
    public GameObject enemyPrefab;

    BenchmarkController benchmarkController;

    public override void Run(BenchmarkController benchmarkController)
    {
      this.benchmarkController = benchmarkController;
      Debug.Log("--- Running AI Combat Testing ---");
      IEnumerator[] enumerators = { Wave1(), Wave2(), Wave3(), Wave4() };
      benchmarkController.RunEnumerators(enumerators);
    }

    IEnumerator Wave1()
    {
      Debug.Log("--- Wave 1 ---");
      for (int i = 0; i < wave1EnemyCount; i++)
      {
        yield return new WaitForEndOfFrame();
        SpawnEnemy(i);
      }
      yield return new WaitForSeconds(timeBetweenWaves);
      // Despawns enemies
      Debug.Log($"--- Wave completed with avg fps of {benchmarkController.CurrentFps} ---");
    }

    IEnumerator Wave2()
    {
      Debug.Log("--- Wave 2 ---");
      for (int i = 0; i < wave2EnemyCount; i++)
      {
        yield return new WaitForEndOfFrame();
        SpawnEnemy(i);
      }
      yield return new WaitForSeconds(timeBetweenWaves);
      // Despawns enemies
      Debug.Log($"--- Wave completed with avg fps of {benchmarkController.CurrentFps} ---");
    }

    IEnumerator Wave3()
    {
      Debug.Log("--- Wave 3 ---");
      for (int i = 0; i < wave3EnemyCount; i++)
      {
        yield return new WaitForEndOfFrame();
        SpawnEnemy(i);
      }
      yield return new WaitForSeconds(timeBetweenWaves);
      // Despawns enemies
      Debug.Log($"--- Wave completed with avg fps of {benchmarkController.CurrentFps} ---");
    }

    IEnumerator Wave4()
    {
      Debug.Log("--- Wave 4 ---");
      for (int i = 0; i < wave4EnemyCount; i++)
      {
        yield return new WaitForEndOfFrame();
        SpawnEnemy(i);
      }
      //SpawnEnemies(wave4EnemyCount);
      yield return new WaitForSeconds(timeBetweenWaves);
      // Despawns enemies
      Debug.Log($"--- Wave completed with avg fps of {benchmarkController.CurrentFps} ---");
      End();
    }

    static void End()
    {
      Debug.Log("--- Finished running AI Combat Testing ---");
    }

    void SpawnEnemy(int i)
    {
      RandomPoint(out Vector3 position);
      GameObject instance = Instantiate(enemyPrefab, position, Quaternion.identity);
      instance.name = i % 2 == 0 ? "Team 1 Soldier" : "Team 2 Soldier";
      TeamController teamController = instance.GetComponent<TeamController>();
      teamController.team = i % 2 == 0 ? Team.Team1 : Team.Team2;
      teamController.Setup();
    }

    static void RandomPoint(out Vector3 result)
    {
      for (int i = 0; i < 30; i++)
      {
        Vector3 randomPoint = Vector3.zero + Random.insideUnitSphere * 25f;
        if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
          continue;

        result = hit.position;
        return;
      }
      result = Vector3.zero;
    }
  }
}
