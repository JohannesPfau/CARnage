// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaronteFX
{
  /// <summary>
  /// Contains the data of a gravity node.
  /// </summary>
  [AddComponentMenu("")]
  public class CNGravity : CNEntity
  {
    [SerializeField]
    private Vector3 gravity_ = new Vector3(0.0f, -9.81f, 0.0f);
    public Vector3 Gravity
    {
      get { return gravity_; }
      set { gravity_ = value; }
    }

    public override CNFieldContentType FieldContentType { get { return CNFieldContentType.GravityNode; } }

    //-----------------------------------------------------------------------------------
    public override void CloneData(CommandNode original)
    {
      base.CloneData(original);

      CNGravity originalGr = (CNGravity)original;
      gravity_ = originalGr.gravity_;
    }
    //-----------------------------------------------------------------------------------
  }
}