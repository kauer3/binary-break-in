using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class Room : MonoBehaviour
{
    public bool onPath, isEntrance, isExit;

    public int gridX, gridY;

    public List<Room> neighbours;
    public List<string> openings = new List<string>();
    public List<string> limits = new List<string>();

    public GameObject[] bases;
    public GameObject[] entranceBases;
    public GameObject[] exitBases;
    public GameObject computerEnd;
    public GameObject computerSet;
    public GameObject serverSet;
    public GameObject singleServer;

    public GameObject PickBase(GameObject[] roomBases)
    {
        GameObject roomBase;
        List<GameObject> possibleBases = new List<GameObject>();
        for (int i = 0; i < roomBases.Length; i++)
        {
            if (onPath)
            {
                RoomBaseInfo rbInfo = roomBases[i].GetComponent<RoomBaseInfo>();
                bool containsAllOpenings = false;
                foreach (string j in openings)
                {
                    if (rbInfo.roomType.Contains(j) == false)
                    {
                        containsAllOpenings = false;
                        break;
                    }
                    else
                    {
                        containsAllOpenings = true;
                    }
                }
                if (containsAllOpenings)
                {
                    possibleBases.Add(roomBases[i]);
                }
            }
            else
            {
                possibleBases.Add(roomBases[i]);
            }
        }

        if (limits.Count > 0)
        {
            for (int i = 0; i < possibleBases.Count; i++)
            {
                RoomBaseInfo rbInfo = possibleBases[i].GetComponent<RoomBaseInfo>();
                if (rbInfo.roomType.Contains("X"))
                {
                    possibleBases.Remove(possibleBases[i]);
                    i--;
                }
                else
                {
                    foreach (string j in limits)
                    {
                        if (rbInfo.roomType.Contains(j))
                        {
                            possibleBases.Remove(possibleBases[i]);
                            i--;
                            break;
                        }
                    }
                }
            }
        }

        int roomBaseIndex = Random.Range(0, possibleBases.Count - 1);
        roomBase = possibleBases[roomBaseIndex];
        return roomBase;
    }

    public void PlaceRoom(bool computer)
    {
        GameObject roomBase = PickBase(isEntrance ? entranceBases : isExit ? exitBases : bases);

        roomBase = (GameObject)Instantiate(roomBase);
        roomBase.transform.parent = transform;
        roomBase.transform.localPosition = Vector3.zero;

        if (isEntrance)
        {
            gameObject.tag = "FirstRoom";
            //gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        else if (isExit)
        {
            Instantiate(computerEnd, transform);
        }
        else
        {
            if (computer)
            {
                SpawnObject(computerSet);
            }

            int servers = Random.Range(0, 7);

            for (int i = 0; i < servers; i++)
            {
                SpawnObject(serverSet);
            }

            int singleServers = Random.Range(0, 5);

            for (int i = 0; i < servers; i++)
            {
                SpawnObject(singleServer);
            }
        }
    }

    void SpawnObject(GameObject obj)
    {
        GameObject instance;
        instance = Instantiate(obj, transform);
        Collider[] colliders;
        List<Collider> validColliders;

        do
        {
            instance.transform.SetLocalPositionAndRotation(
                new Vector3(Random.Range(-8.5f, 8.5f), 0, Random.Range(-8.5f, 8.5f)),
                Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
            colliders = Physics.OverlapSphere(instance.transform.position, 1.5f, 1 << 11);

            validColliders = new List<Collider>();
            foreach (Collider collider in colliders)
            {
                if (!(collider.transform == instance.transform || collider.transform.parent.transform == instance.transform))
                {
                    validColliders.Add(collider);
                }
            }
        }
        while (validColliders.Count > 0);
    }
}
