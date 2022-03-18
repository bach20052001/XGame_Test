using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjectPrefabs = new List<GameObject>();

    private List<GameObject> listGameObject = new List<GameObject>();

    public int initialSize;

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            InitialObject();
        }
    }

    public GameObject GetUnactiveObject()
    {
        for (int i = 0; i < listGameObject.Count; i++)
        {
            if (!listGameObject[i].activeSelf)
            {
                listGameObject[i].SetActive(true);
                return listGameObject[i];
            }
        }

        //If all object actived
        GameObject newObject = InitialObject();
        newObject.SetActive(true);
        return newObject;
    }

    private GameObject InitialObject()
    {
        GameObject tmp = Instantiate(gameObjectPrefabs[Random.Range(0, gameObjectPrefabs.Count)], Vector3.zero, Quaternion.identity);
        tmp.SetActive(false);
        tmp.transform.SetParent(this.gameObject.transform);
        listGameObject.Add(tmp);

        return tmp;
    }
}
