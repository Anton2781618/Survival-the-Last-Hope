using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRaycastHandler
{
    public void UpdateMe();

    public Collider GetHitCollider();
}
