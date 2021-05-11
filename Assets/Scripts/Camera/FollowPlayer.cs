////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: FollowPlayer.cs
// Description: 
// Date Created: 11/05/2021
// Last Edit: 11/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform m_targetTransform;

    [SerializeField] private Vector3 m_positionOffset;
    #endregion // Variables

    #region Functions
    private void Update()
    {
        if (m_targetTransform == null)
        {
            enabled = false;
            return;
        }

        Follow();
    }

    private void Follow()
    {
        Vector3 forward = m_targetTransform.TransformDirection(Vector3.forward);
        Vector3 position = new Vector3(
            m_targetTransform.position.x * forward.x,
            m_targetTransform.position.y * forward.y,
            m_targetTransform.position.z * forward.z
            );
        Vector3 posTarget = position + m_positionOffset;

        transform.position = posTarget;
    }
    #endregion // Functions
}
