////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: WorldGenerator.cs
// Description: 
// Date Created: 12/05/2021
// Last Edit: 13/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldTile
{
    public GameObject gameObject;
    public List<int> nextTileIndex;
}


public class WorldGenerator : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<WorldTile> m_worldEnvironmentTiles = null;
    [SerializeField] private List<WorldTile> m_worldInteractableTiles = null;

    [SerializeField] private Transform m_playerTransform = null;

    [SerializeField] private float m_startingTiles = 2;
    [SerializeField] private float m_totalTiles = 5;

    [SerializeField] private float m_tileSpacing = 25f;
    [SerializeField] private float m_tileDistance = 1f;
    [SerializeField] private Vector3 m_nextTilePosition = Vector3.zero;    
    private Queue<GameObject> m_currentTiles = null;

    private WorldTile m_previousInteractable = null;
    #endregion // Variables

    #region Functions
    private void Start()
    {
        InitialiseWorld();
    }

    private void InitialiseWorld()
    {
        m_currentTiles = new Queue<GameObject>();

        for (int i = 0; i < m_startingTiles; ++i)
        {
            GameObject temp = NewTile();
            UpdateNextTilePosition(temp.GetComponent<MeshFilter>().mesh);
            m_currentTiles.Enqueue(temp);
        }

        for (int i = 0; i < m_totalTiles - m_startingTiles; ++i)
        {
            NextTile(false);
        }
    }

    private void Update()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        Vector3 currentTilePos = m_currentTiles.Peek().transform.position;

        // Checking facing on the z axis
        float playerAngle = Vector3.Dot(Vector3.forward, m_playerTransform.forward);
        // Looking forward (0, 0, 1)
        if (playerAngle > 0.9f)
        {
            if (m_playerTransform.position.z - m_tileSpacing > currentTilePos.z)
            {
                NextTile();
            }
        }
        // Looking backward (0, 0, -1)
        else if (playerAngle < -0.9f)
        {
            Debug.Log("backward");
            if (m_playerTransform.position.z + m_tileSpacing < currentTilePos.z)
            {
                NextTile();
            }
        }

        // Checking facing on the x axis
        playerAngle = Vector3.Dot(Vector3.forward, m_playerTransform.right);
        // Looking left (1, 0, 0)
        if (playerAngle > 0.9f)
        {
            Debug.Log("left");
            if (m_playerTransform.position.x + m_tileSpacing < currentTilePos.x)
            {
                NextTile();
            }
        }
        // Looking right (-1, 0, 0)
        else if (playerAngle < -0.9f)
        {
            Debug.Log("right");
            if (m_playerTransform.position.x - m_tileSpacing > currentTilePos.x)
            {
                NextTile();
            }
        }
    }

    private void NextTile(bool a_destroyNext = true)
    {
        if (a_destroyNext)
            Destroy(m_currentTiles.Dequeue());

        GameObject tile = NewTile();
        tile.transform.position = m_nextTilePosition;

        WorldTile interactable = NewInteractable();
        interactable.gameObject.transform.SetParent(tile.transform);

        UpdateNextTilePosition(tile.GetComponent<MeshFilter>().mesh);
        m_currentTiles.Enqueue(tile);
    }

    private void UpdateNextTilePosition(Mesh a_mesh)
    {
        Vector3 tileSize = a_mesh.bounds.size;
        Vector3 spacing = m_playerTransform.TransformDirection(Vector3.forward);
        m_nextTilePosition += new Vector3(tileSize.x * spacing.x, 0f, tileSize.z * spacing.z) * m_tileDistance;
    }

    private GameObject NewTile()
    {
        int index = UnityEngine.Random.Range(0, m_worldEnvironmentTiles.Count);
        GameObject temp = Instantiate(m_worldEnvironmentTiles[index].gameObject, m_nextTilePosition, m_playerTransform.rotation, transform);
        return temp;
    }

    private WorldTile NewInteractable()
    {
        if (m_previousInteractable == null)
        {
            m_previousInteractable = m_worldInteractableTiles[UnityEngine.Random.Range(0, m_worldInteractableTiles.Count)];
        }

        int arraySize = m_previousInteractable.nextTileIndex.Count;
        int index = m_previousInteractable.nextTileIndex[UnityEngine.Random.Range(0, arraySize)];

        WorldTile tile = new WorldTile();
        tile.gameObject = Instantiate(m_worldInteractableTiles[index].gameObject, m_nextTilePosition, m_playerTransform.rotation);
        tile.nextTileIndex = m_worldInteractableTiles[index].nextTileIndex;

        m_previousInteractable = tile;
        return tile;
    }
    #endregion // Functions
}
