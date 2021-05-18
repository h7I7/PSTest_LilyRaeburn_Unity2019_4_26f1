////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: WorldGenerator.cs
// Description: Manage world generation
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
    [Header("World generation assets")]
    [SerializeField] private List<WorldTile> m_environmentTiles = null;
    [SerializeField] private List<WorldTile> m_interactableTiles = null;

    [Space(10f)]
    [Header("World generation variables")]
    [SerializeField] private float m_startingBlankTiles = 2;
    [SerializeField] private float m_totalWorldTiles = 5;

    [SerializeField] private float m_tileUpdateDistance = 25f; // Distance between the player and last tile before last tile is deleted and a new one is added
    [SerializeField] private float m_tileKerning = 1f; // Percentage distance between tiles (1f = 100%, 0f = 0%)

    [Space(10f)]
    [Header("Player handle")]
    [SerializeField] private Transform m_playerTransform = null;

    private Vector3 m_nextTilePosition = Vector3.zero;
    
    private Queue<GameObject> m_currentTiles = null;
    private WorldTile m_previousInteractable = null;
    #endregion Variables

    #region Functions
    private void Start()
    {
        InitialiseWorld();
    }

    private void InitialiseWorld()
    {
        m_currentTiles = new Queue<GameObject>();

        // Add m_startingTiles amount of blank environment tiles to the world
        for (int i = 0; i < m_startingBlankTiles; ++i)
        {
            GameObject temp = NewTile();
            UpdateNextTilePosition(temp.GetComponent<MeshFilter>().mesh);
            m_currentTiles.Enqueue(temp);
        }

        // Add m_totalTiles - m_startingTiles worth of tiles (with interactable elements) to the world
        for (int i = 0; i < m_totalWorldTiles - m_startingBlankTiles; ++i)
        {
            NextTile(false);
        }
    }

    private void Update()
    {
        CheckPlayerPosition();
    }

    // This code was written with the intention of adding turns to the level, some of the calculations may be redundant
    private void CheckPlayerPosition()
    {
        Vector3 currentTilePos = m_currentTiles.Peek().transform.position;

        // Checking player facing on the z axis
        float playerAngle = Vector3.Dot(Vector3.forward, m_playerTransform.forward);
        // Looking forward (0, 0, 1)
        if (playerAngle > 0.9f)
        {
            if (m_playerTransform.position.z - m_tileUpdateDistance > currentTilePos.z)
            {
                NextTile();
            }
        }
        // Looking backward (0, 0, -1)
        else if (playerAngle < -0.9f)
        {
            if (m_playerTransform.position.z + m_tileUpdateDistance < currentTilePos.z)
            {
                NextTile();
            }
        }

        // Checking player facing on the x axis
        playerAngle = Vector3.Dot(Vector3.forward, m_playerTransform.right);
        // Looking left (1, 0, 0)
        if (playerAngle > 0.9f)
        {
            if (m_playerTransform.position.x + m_tileUpdateDistance < currentTilePos.x)
            {
                NextTile();
            }
        }
        // Looking right (-1, 0, 0)
        else if (playerAngle < -0.9f)
        {
            if (m_playerTransform.position.x - m_tileUpdateDistance > currentTilePos.x)
            {
                NextTile();
            }
        }
    }

    // Adds a tile to the world
    private void NextTile(bool a_destroyNext = true)
    {
        if (a_destroyNext)
            Destroy(m_currentTiles.Dequeue());

        // Get new environment tile
        GameObject tile = NewTile();
        tile.transform.position = m_nextTilePosition;

        // Combine environment tile with interactable layer
        WorldTile interactable = NewInteractable();
        interactable.gameObject.transform.SetParent(tile.transform);

        // Update tile position and add tile to the tile queue
        UpdateNextTilePosition(tile.GetComponent<MeshFilter>().mesh);
        m_currentTiles.Enqueue(tile);
    }

    // Updates the next tile position based on the previous tile's size
    private void UpdateNextTilePosition(Mesh a_mesh)
    {
        Vector3 tileSize = a_mesh.bounds.size;
        Vector3 playerForward = m_playerTransform.TransformDirection(Vector3.forward);
        m_nextTilePosition += new Vector3(tileSize.x * playerForward.x, 0f, tileSize.z * playerForward.z) * m_tileKerning;
    }

    // Returns new random environment tile
    private GameObject NewTile()
    {
        int index = UnityEngine.Random.Range(0, m_environmentTiles.Count);
        GameObject temp = Instantiate(m_environmentTiles[index].gameObject, m_nextTilePosition, m_playerTransform.rotation, transform);
        return temp;
    }

    // Returns new random interactable layer
    private WorldTile NewInteractable()
    {
        if (m_previousInteractable == null)
        {
            m_previousInteractable = m_interactableTiles[UnityEngine.Random.Range(0, m_interactableTiles.Count)];
        }

        // Some interactable layers lead onto other interactable layers, this is specified in the editor
        // This gets a random layer based off the previous layer
        int arraySize = m_previousInteractable.nextTileIndex.Count;
        int index = m_previousInteractable.nextTileIndex[UnityEngine.Random.Range(0, arraySize)];

        WorldTile tile = new WorldTile();
        tile.gameObject = Instantiate(m_interactableTiles[index].gameObject, m_nextTilePosition, m_playerTransform.rotation);
        tile.nextTileIndex = m_interactableTiles[index].nextTileIndex;

        m_previousInteractable = tile;
        return tile;
    }
    #endregion Functions
}
