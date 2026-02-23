// Author: Juan Pablo Camporeale
// File: Player.cs
// Date: 12/12/2024

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    #region enums

    public enum PickupType
    {
        Health,
        Other
    }
    #endregion
    
    #region Inspector Vars

    [SerializeField] private PickupType m_pickupType;
    [SerializeField] private int m_restoreAmount;
    #endregion
  
    #region Public API

    public int OnPickedUp()
    {
        return m_restoreAmount;
    }
    #endregion
}
