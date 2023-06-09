using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Player;
using Random = UnityEngine.Random;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private int numWayPointsToPatrol = 0;
    [SerializeField] private NavMeshAgent agent = null;
    private GameObject wayPointParent = null;
    [SerializeField] private float idleTimer = 0;
    private List<Vector3> wayPoints = new List<Vector3>();
    private EnemyManager enemyManager = null;
    private GameObject player = null;
    private float distanceToPlayer = 100;
    private bool shouldMove = true;

    [SerializeField] private GameObject deathAudioObject;
    
    private void OnDestroy()
    {
        GameObject audioInstance = Instantiate(deathAudioObject);
        audioInstance.GetComponent<AudioSource>().Play();
        Destroy(audioInstance, 5f);
    }

    private void Awake()
    {
        GetInitialStuff();
        SetWayPoints();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetInitialStuff()
    {
        enemyManager = FindAnyObjectByType<EnemyManager>();
        wayPointParent = GameObject.Find("WayPointList");
        player = enemyManager.GetPlayer();
    }

    public void StartShouldMoveTimer()
    {
        shouldMove = false;
        StartCoroutine(MoveTimer());
    }

    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(idleTimer);
        SetShouldMove(true);
    }
    #region Getter Functions
    public bool GetShouldMove()
    {
        return shouldMove;
    }

    //Returns the distacne from the player
    public float GetPlayerDistance()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
        return distanceToPlayer;
    }

    //Returns the way points the enemy will be patroling
    public List<Vector3> GetWayPoints()
    {
        return wayPoints;
    }

    //Returns the Nav Mesh Agent
    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public Vector3 GetRandomWayPoint()
    {
/*       if(wayPoints.Count == 0)
        {
            return new Vector3(0, 0, 0);
        }*/
        int temp = Random.Range(0, wayPoints.Count - 1);
        return wayPoints[temp];
    }

    public GameObject GetPlayer()
    {
        return player;
    }
    #endregion

    #region Mutator Functions
    public void SetShouldMove(bool answer)
    {
        shouldMove = answer;
    }

    private void SetWayPoints()
    {
        for(int i = 0; i < numWayPointsToPatrol; i ++)
        {
            wayPoints.Add(wayPointParent.transform.GetChild(Random.Range(0, wayPointParent.transform.childCount - 1)).position);
        }
    }

    public void SetEnemyDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void ClearEnemyDestination()
    {
        agent.ResetPath();
    }

    public void DestroySelf()
    {
        enemyManager.DestroyEnemy(this.gameObject);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ouhcy Ouch");
            PlayerManager.Instance.Damage(1);
        }
    }
}
