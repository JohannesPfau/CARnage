// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using System.Collections;
using UnityEngine;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a cloth bodies node.
  /// </summary>
  [AddComponentMenu("")]
  public class CNCloth : CNBody
  {
    [SerializeField]
    bool cloth_autoCollide_ = true;
    public bool Cloth_AutoCollide
    {
      get { return cloth_autoCollide_; }
      set { cloth_autoCollide_ = value; }
    }

    [SerializeField]
    bool disableCollisionNearJoints_ = true;
    public bool DisableCollisionNearJoints
    {
      get { return disableCollisionNearJoints_; }
      set { disableCollisionNearJoints_ = value; }
    }

    [SerializeField]
    bool safeModeIntegration_ = false;
    public bool SafeModeIntegration
    {
      get { return safeModeIntegration_; }
      set { safeModeIntegration_ = value; }
    }

    [SerializeField]  
    float cloth_collisionRadius_ = 0.05f; // 5 cm   
    public float Cloth_CollisionRadius
    {
      get { return cloth_collisionRadius_; }
      set { cloth_collisionRadius_ = value; }
    }

    [SerializeField]   
    float cloth_bend_ = 3.0f;       // 0.07 Newtons to bend 0.01 m a square fabric 1x1m
    public float Cloth_Bend
    {
      get { return cloth_bend_; }
      set { cloth_bend_ = value; }
    }
   
    [SerializeField]        
    float cloth_stretch_ = 1000.0f; // 1000 Newtons to enlarge 0.01 m a square fabric 1x1m
    public float Cloth_Stretch
    {
      get { return cloth_stretch_; }
      set { cloth_stretch_ = value; }
    }
    
    [SerializeField]     
    float cloth_dampingBend_ = 10f;
    public float Cloth_DampingBend
    {
      get { return cloth_dampingBend_; }
      set { cloth_dampingBend_ = value; }
    }
    
    [SerializeField]  
    float cloth_dampingStretch_ = 10f;
    public float Cloth_DampingStretch
    {
      get { return cloth_dampingStretch_; }
      set { cloth_dampingStretch_ = value; }
    }

    [SerializeField]
    bool useColliderUVs_ = false;
    public bool UseColliderUVs
    {
      get { return useColliderUVs_; }
      set { useColliderUVs_ = value; }
    }

    public override CNFieldContentType FieldContentType { get { return CNFieldContentType.ClothBodyNode; } }

    public override void CloneData(CommandNode originalNode)
    {
      base.CloneData(originalNode);

      CNCloth originalCloth = (CNCloth)originalNode;

      cloth_autoCollide_          = originalCloth.cloth_autoCollide_;
      disableCollisionNearJoints_ = originalCloth.disableCollisionNearJoints_;
      safeModeIntegration_        = originalCloth.safeModeIntegration_;

      cloth_bend_            = originalCloth.cloth_bend_;             
      cloth_stretch_         = originalCloth.cloth_stretch_;
      cloth_dampingBend_     = originalCloth.cloth_dampingBend_;
      cloth_dampingStretch_  = originalCloth.cloth_dampingStretch_;
      cloth_collisionRadius_ = originalCloth.cloth_collisionRadius_; 
      useColliderUVs_        = originalCloth.useColliderUVs_;
    }
  

  } //class CNCloth...

} //namespace CaronteFX...
