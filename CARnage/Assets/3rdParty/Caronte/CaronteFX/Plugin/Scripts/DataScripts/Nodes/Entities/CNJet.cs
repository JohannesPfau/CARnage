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
  /// 
  /// </summary>
  [AddComponentMenu("")]
  public class CNJet : CNEntity
  {
    [SerializeField]
    CNField locators_;
    public CNField Locators
    {
      get
      {
        if ( locators_ == null )
        {
          locators_ = new CNField( false, CNFieldContentType.Locator | CNFieldContentType.Geometry,
                                   CNField.ScopeFlag.Inherited, false );
        }
        return locators_;
      }
    }

    [SerializeField]
    private Vector3 force_ = new Vector3(10f, 0f, 0f);
    public Vector3 Force
    {
      get { return force_; }
      set { force_ = value; }
    }

    [SerializeField]
    private float speedLimit_ = 20f;
    public float SpeedLimit
    {
      get { return speedLimit_; }
      set { speedLimit_ = Mathf.Clamp(value, 0f, float.MaxValue); }
    }

    [SerializeField]
    private float forceDeltaMax_ = 5f;
    public float ForceDeltaMax
    {
      get { return forceDeltaMax_; }
      set { forceDeltaMax_ = Mathf.Clamp(value, 0f, float.MaxValue); }
    }

    [SerializeField]
    private float angleDeltaMax_ = 45f;
    public float AngleDeltaMax
    {
      get { return angleDeltaMax_; }
      set { angleDeltaMax_ = Mathf.Clamp(value, 0f, 360f); }
    }

    [SerializeField]
    private float periodTime_ = 5.0f;
    public float PeriodTime
    {
      get { return periodTime_; }
      set { periodTime_ = Mathf.Clamp(value, 0f, float.MaxValue); }
    }

    [SerializeField]
    private float periodSpace_ = 5.0f;
    public float PeriodSpace
    {
      get { return periodSpace_; }
      set { periodSpace_ = Mathf.Clamp(value, 0f, float.MaxValue) ; }
    }

    [SerializeField]
    private float highFrequency_am_ = 0.1f;
    public float HighFrequency_am
    {
      get { return highFrequency_am_; }
      set { highFrequency_am_ = Mathf.Clamp(value, 0f, float.MaxValue); }
    }

    [SerializeField]
    private float highFrequency_sp_ = 10f;
    public float HighFrequency_sp
    {
      get { return highFrequency_sp_;  }
      set { highFrequency_sp_ = Mathf.Clamp(value, 0f, float.MaxValue); }
    }

    [SerializeField]
    private bool noiseFoldout_ = false;
    public bool NoiseFoldout
    {
      get { return noiseFoldout_; }
      set { noiseFoldout_ = value; }
    }

    [SerializeField]
    private bool hfFoldout_ = false;
    public bool HfFoldout
    {
      get { return hfFoldout_; }
      set { hfFoldout_ = value; }
    }

    public override CNFieldContentType FieldContentType { get { return CNFieldContentType.JetNode; } }

    public override void CloneData(CommandNode original)
    {
      base.CloneData(original);

      CNJet originalJet = (CNJet)original;

      locators_ = originalJet.Locators.DeepClone();
       
      Name          = originalJet.Name;
      Timer         = originalJet.Timer;
      
      force_        = originalJet.Force;
      speedLimit_   = originalJet.SpeedLimit;
      
      forceDeltaMax_ = originalJet.forceDeltaMax_;
      angleDeltaMax_ = originalJet.angleDeltaMax_;
      
      periodTime_       = originalJet.periodTime_;
      periodSpace_      = originalJet.periodSpace_;
      
      highFrequency_am_ = originalJet.highFrequency_am_;
      highFrequency_sp_ = originalJet.highFrequency_sp_;
    }
  }
}
