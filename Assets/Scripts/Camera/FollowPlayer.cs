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
        Vector3 up = m_targetTransform.up;
        Vector3 posTarget = new Vector3(m_targetTransform.position.x * forward.x, 0f, m_targetTransform.position.z * forward.z) + (forward * m_positionOffset.z) + (up * m_positionOffset.y);

        transform.position = posTarget;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_targetTransform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
    #endregion // Functions
}
