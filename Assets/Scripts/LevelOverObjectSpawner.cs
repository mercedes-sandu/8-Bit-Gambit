using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOverObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnLocations;

    [SerializeField] private GameObject[] playerWinPrefabs;
    [SerializeField] private GameObject[] playerLossPrefabs;
    [SerializeField] private GameObject[] drawPrefabs;
    
    [SerializeField] private GameObject playerDamageParticles;
    [SerializeField] private GameObject opponentDamageParticles;

    [SerializeField] private float pauseBeforeSpawn = 1f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float timeBetweenParticleSpawns = 2f;

    private LevelManager.EndState _endState;
    
    private List<GameObject> _spawnedObjects = new ();

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        GameEvent.OnLevelOver += StartSpawnObjects;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endState"></param>
    private void StartSpawnObjects(LevelManager.EndState endState)
    {
        _endState = endState;
        Invoke(nameof(SpawnObjects), pauseBeforeSpawn);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void SpawnObjects()
    {
        List<GameObject> particlesToSpawn;
        
        switch (_endState)
        {
            case LevelManager.EndState.PlayerWon:
                SpawnPlayerWinObjects();
                particlesToSpawn = new List<GameObject>
                {
                    playerDamageParticles, playerDamageParticles, playerDamageParticles, playerDamageParticles
                };
                break;
            case LevelManager.EndState.PlayerLost:
                SpawnPlayerLossObjects();
                particlesToSpawn = new List<GameObject>
                {
                    opponentDamageParticles, opponentDamageParticles, opponentDamageParticles, opponentDamageParticles
                };
                break;
            case LevelManager.EndState.Draw:
                SpawnDrawObjects();
                particlesToSpawn = new List<GameObject>
                {
                    playerDamageParticles, opponentDamageParticles, playerDamageParticles, opponentDamageParticles
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        StartCoroutine(RotateSpawnedObjects());
        StartCoroutine(SpawnParticles(particlesToSpawn));
    }

    /// <summary>
    /// 
    /// </summary>
    private void SpawnPlayerWinObjects()
    {
        for (var i = 0; i < spawnLocations.Length; i++)
        {
            var spawnLocation = spawnLocations[i];
            var prefab = playerWinPrefabs[i];
            var pos = spawnLocation.position;
            var spawnedPiece = Instantiate(prefab, pos, Quaternion.identity);
            spawnedPiece.transform.SetParent(transform, true);
            spawnedPiece.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            _spawnedObjects.Add(spawnedPiece);
            Instantiate(playerDamageParticles, pos, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void SpawnPlayerLossObjects()
    {
        for (var i = 0; i < spawnLocations.Length; i++)
        {
            var spawnLocation = spawnLocations[i];
            var prefab = playerLossPrefabs[i];
            var pos = spawnLocation.position;
            var spawnedPiece = Instantiate(prefab, pos, Quaternion.identity);
            spawnedPiece.transform.SetParent(transform, true);
            spawnedPiece.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            _spawnedObjects.Add(spawnedPiece);
            Instantiate(opponentDamageParticles, pos, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void SpawnDrawObjects()
    {
        for (var i = 0; i < spawnLocations.Length; i++)
        {
            var spawnLocation = spawnLocations[i];
            var prefab = drawPrefabs[i];
            var pos = spawnLocation.position;
            var spawnedPiece = Instantiate(prefab, pos, Quaternion.identity);
            spawnedPiece.transform.SetParent(transform, true);
            spawnedPiece.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            _spawnedObjects.Add(spawnedPiece);
            Instantiate(i % 2 == 0 ? playerDamageParticles : opponentDamageParticles, pos, Quaternion.identity);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotateSpawnedObjects()
    {
        while (true)
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                spawnedObject.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="particlesToSpawn"></param>
    /// <returns></returns>
    private IEnumerator SpawnParticles(List<GameObject> particlesToSpawn)
    {
        while (true)
        {
            for (var i = 0; i < spawnLocations.Length; i++)
            {
                Instantiate(particlesToSpawn[i], spawnLocations[i].position, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(timeBetweenParticleSpawns);
        }
    
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnLevelOver -= StartSpawnObjects;
    }
}