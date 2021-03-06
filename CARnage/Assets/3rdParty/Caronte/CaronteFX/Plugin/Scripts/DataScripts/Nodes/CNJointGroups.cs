// ***********************************************************
//	Copyright 2016 Next Limit Technologies, http://www.nextlimit.com
//	All rights reserved.
//
//	This source code is free for all non-commercial uses.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CaronteFX
{
  /// <summary>
  /// Holds the data of a multi joint node.
  /// </summary>
  [AddComponentMenu("")]
  public class CNJointGroups : CommandNode
  {
    public enum ForceMaxModeEnum
    {
      Unlimited,
      ConstantLimit,
    }

    public enum CreationModeEnum
    {
      ByContact,
      ByStem,
      ByMatchingVertices,
      AtLocatorsPositions,
      AtLocatorsBBoxCenters,
      AtLocatorsVertexes
    }

    #region createParams
    [SerializeField]
    CNField objectsA_;
    public CNField ObjectsA
    {
      get
      {
        if ( objectsA_ == null )
        {
          objectsA_ = new CNField( false, CNFieldContentType.Geometry | CNFieldContentType.BodyNode, 
                                   CNField.ScopeFlag.Inherited, false );
        }
        return objectsA_;
      }
    }

    [SerializeField]
    CNField objectsB_;
    public CNField ObjectsB
    {
      get
      {
        if ( objectsB_ == null )
        {
          objectsB_ = new CNField( false, CNFieldContentType.Geometry | CNFieldContentType.BodyNode, 
                                   CNField.ScopeFlag.Inherited, false );
        }
        return objectsB_;
      }
    }

    [SerializeField]
    CNField locatorsC_;
    public CNField LocatorsC
    {
      get
      {
        if ( locatorsC_ == null )
        {
          locatorsC_ = new CNField( false, CNFieldContentType.Locator | CNFieldContentType.Geometry,
                                    CNField.ScopeFlag.Inherited, false );
        }
        return locatorsC_;
      }
    }

    [SerializeField]
    CreationModeEnum creationMode_ = CreationModeEnum.ByContact;
    public CreationModeEnum CreationMode
    {
      get { return creationMode_; }
      set { creationMode_ = value; }
    }

    [SerializeField]
    bool isRigidGlue_ = false;
    public bool IsRigidGlue
    {
      get { return isRigidGlue_; }
      set { isRigidGlue_ = value; }
    }


    #region By Contact Parameters
    [SerializeField]
    float contactDistanceSearch_ = 0.01f;
    public float ContactDistanceSearch
    {
      get { return contactDistanceSearch_; }
      set { contactDistanceSearch_ = value; }
    }

    [SerializeField]
    float contactAreaMin_ = 0.0001f;
    public float ContactAreaMin
    {
      get { return contactAreaMin_; }
      set { contactAreaMin_ = value; }
    }
    
    [SerializeField]
    float contactAngleMaxInDegrees_ = 10.0f;
    public float ContactAngleMaxInDegrees
    {
      get { return contactAngleMaxInDegrees_; }
      set { contactAngleMaxInDegrees_ = value; }
    }

    [SerializeField]
    int contactNumberMax_ = 4;
    public int ContactNumberMax
    {
      get { return contactNumberMax_; }
      set { contactNumberMax_ = Mathf.Clamp( value, 0, int.MaxValue ); }
    }

    #endregion

    [SerializeField]
    float matchingDistanceSearch_ = 0.002f;
    public float MatchingDistanceSearch
    {
      get { return matchingDistanceSearch_; }
      set { matchingDistanceSearch_ = value; }
    }

    [SerializeField]
    bool limitNumberOfActiveJoints_ = false;
    public bool LimitNumberOfActiveJoints
    {
      get { return limitNumberOfActiveJoints_; }
      set { limitNumberOfActiveJoints_ = value; }
    }

    [SerializeField]
    int activeJointsMaxInABPair_ = 0;
    public int ActiveJointsMaxInABPair
    {
      get { return activeJointsMaxInABPair_; }
      set { activeJointsMaxInABPair_ = value; }
    }

    [SerializeField]
    bool disableCollisionsByPairs_ = false;
    public bool DisableCollisionsByPairs
    {
      get { return disableCollisionsByPairs_; }
      set { disableCollisionsByPairs_ = value; }
    }

    [SerializeField]
    bool disableAllCollisionsOfAsWithBs_ = false;
    public bool DisableAllCollisionsOfAsWithBs
    {
      get { return disableAllCollisionsOfAsWithBs_; }
      set { disableAllCollisionsOfAsWithBs_ = value; }
    }

    #endregion

    #region editParams

    #region Forces
    [SerializeField]
    ForceMaxModeEnum forcemaxMode_ = ForceMaxModeEnum.Unlimited;
    public ForceMaxModeEnum ForceMaxMode
    {
      get { return forcemaxMode_; }
      set { forcemaxMode_ = value; }
    }

    [SerializeField]
    float forceMax_ = 150000.0f;
    public float ForceMax
    {
      get {  return forceMax_; }
      set { forceMax_ = value; }
    }

    [SerializeField]
    float forceMaxRand_ = 0.0f;                 //!< Random forces in [0, forceMaxRand_] will be added to forceMax_
    public float  ForceMaxRand
    {
      get {  return forceMaxRand_; }
      set { forceMaxRand_ = value; }
    }

    [SerializeField]
    float forceRange_ = 0.1f; 
    public float  ForceRange
    {
      get {  return forceRange_; }
      set { forceRange_ = value; }
    }

    [SerializeField]
    AnimationCurve forceProfile_ = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public AnimationCurve ForceProfile
    {
      get {  return forceProfile_; }
      set { forceProfile_ = value; }
    }

    #endregion

    #region Collisions
    [SerializeField]
    bool enableCollisionIfBreak_ = true;
    public bool EnableCollisionIfBreak
    {
      get { return enableCollisionIfBreak_; }
      set { enableCollisionIfBreak_ = value; }
    }
    #endregion

    #region Break
    [SerializeField]
    bool breakIfForceMax_ = false;
    public bool BreakIfForceMax
    {
      get { return breakIfForceMax_; }
      set { breakIfForceMax_ = value; }
    }

    [SerializeField]
    bool breakAllIfLeftFewUnbroken_ = false;
    public bool BreakAllIfLeftFewUnbroken
    {
      get { return breakAllIfLeftFewUnbroken_; }
      set { breakAllIfLeftFewUnbroken_ = value; }
    }

    [SerializeField]
    int unbrokenNumberForBreakAll_ = 2;
    public int UnbrokenNumberForBreakAll
    {
      get { return unbrokenNumberForBreakAll_; }
      set { unbrokenNumberForBreakAll_ = value; }
    }

    [SerializeField]
    bool breakIfDistExcedeed_ = false;
    public bool BreakIfDistExcedeed
    {
      get { return breakIfDistExcedeed_; }
      set { breakIfDistExcedeed_ = value; }
    }

    [SerializeField]
    float distanceForBreak_ = 0.01f;
    public float DistanceForBreak
    {
      get { return distanceForBreak_; }
      set { distanceForBreak_ = value; }
    }

    [SerializeField]
    float distanceForBreakRand_ = 0.0f;         //!< Random distances in [0, distanceForBreakRand_] will be added to distanceForBreak_
    public float DistanceForBreakRand
    {
      get { return distanceForBreakRand_; }
      set { distanceForBreakRand_ = value; }
    }
    #endregion

    [SerializeField]
    bool breakIfHinge_ = false;
    public bool BreakIfHinge
    {
      get { return breakIfHinge_; }
      set { breakIfHinge_ = value; }
    }

    [SerializeField]
    bool plasticity_ = false;
    public bool Plasticity
    {
      get { return plasticity_; }
      set { plasticity_ = value; }
    }

    [SerializeField]
    float distanceForPlasticity_ = 0.005f;
    public float DistanceForPlasticity
    {
      get { return distanceForPlasticity_; }
      set { distanceForPlasticity_ = value; }
    }
    
    [SerializeField]
    float distanceForPlasticityRand_ = 0.005f;    //!< Random distances in [0, distanceForBreakRand_] will be added to distanceForBreak_
    public float DistanceForPlasticityRand
    {
      get { return distanceForPlasticityRand_; }
      set { distanceForPlasticityRand_ = value; }
    }

    [SerializeField]
    float plasticityRateAcquired_ = 0.05f;
    public float PlasticityRateAcquired
    {
      get { return plasticityRateAcquired_; }
      set { plasticityRateAcquired_ = value; }
    }

    [SerializeField]
    bool forcesFoldout_;
    public bool ForcesFoldout
    {
      get { return forcesFoldout_; }
      set { forcesFoldout_ = value; }
    }

    [SerializeField]
    bool collisionsFoldout_;
    public bool CollisionFoldout
    {
      get { return collisionsFoldout_; }
      set { collisionsFoldout_ = value; }
    }

    [SerializeField]
    bool breakFoldout_;
    public bool BreakFoldout
    {
      get { return breakFoldout_; }
      set { breakFoldout_ = value; }
    }

    [SerializeField]
    bool plasticityFoldout_;
    public bool PlasticityFoldout
    {
      get { return plasticityFoldout_; }
      set { plasticityFoldout_ = value; }
    }

    [SerializeField]
    float delayedCreationTime_;
    public float DelayedCreationTime
    {
      get { return delayedCreationTime_; }
      set { delayedCreationTime_ = value; }
    }

    [SerializeField]
    private float damping_ = 0.0f;
    public float Damping
    {
      get { return damping_; }
      set { damping_ = value; }
    }

    #endregion

    public bool IsCreateModeAtLocators
    {
      get
      {
        return  CreationMode == CreationModeEnum.AtLocatorsPositions ||
                CreationMode == CreationModeEnum.AtLocatorsBBoxCenters ||
                CreationMode == CreationModeEnum.AtLocatorsVertexes;
      }
    }

    public override CNFieldContentType FieldContentType { get { return CNFieldContentType.MultiJointNode; } }

    public override void CloneData(CommandNode original)
    {
      base.CloneData(original);

      CNJointGroups originalJG = (CNJointGroups)original;

      objectsA_  = originalJG.ObjectsA.DeepClone();
      objectsB_  = originalJG.ObjectsB.DeepClone();
      locatorsC_ = originalJG.LocatorsC.DeepClone();

      delayedCreationTime_        = originalJG.delayedCreationTime_;
      creationMode_               = originalJG.creationMode_;
      isRigidGlue_                = originalJG.isRigidGlue_;
      contactDistanceSearch_      = originalJG.contactDistanceSearch_;
      contactAreaMin_             = originalJG.contactAreaMin_;
      contactAngleMaxInDegrees_   = originalJG.contactAngleMaxInDegrees_;
      contactNumberMax_           = originalJG.contactNumberMax_;

      matchingDistanceSearch_     = originalJG.matchingDistanceSearch_;
      limitNumberOfActiveJoints_  = originalJG.limitNumberOfActiveJoints_;
      activeJointsMaxInABPair_    = originalJG.activeJointsMaxInABPair_;

      disableCollisionsByPairs_       = originalJG.disableCollisionsByPairs_;
      disableAllCollisionsOfAsWithBs_ = originalJG.disableAllCollisionsOfAsWithBs_;

      forcemaxMode_                = originalJG.forcemaxMode_;
      forceMax_                    = originalJG.forceMax_;
      forceMaxRand_                = originalJG.forceMaxRand_;
      forceProfile_                = originalJG.forceProfile_.DeepClone();
      
      enableCollisionIfBreak_    = originalJG.enableCollisionIfBreak_;
      breakIfForceMax_           = originalJG.breakIfForceMax_;
      breakAllIfLeftFewUnbroken_ = originalJG.breakAllIfLeftFewUnbroken_;
      unbrokenNumberForBreakAll_ = originalJG.unbrokenNumberForBreakAll_;
      breakIfDistExcedeed_       = originalJG.breakIfDistExcedeed_;
      distanceForBreak_          = originalJG.distanceForBreak_;
      distanceForBreakRand_      = originalJG.distanceForBreakRand_;
      breakIfHinge_              = originalJG.breakIfHinge_;

      plasticity_                = originalJG.plasticity_;
      distanceForPlasticity_     = originalJG.distanceForPlasticity_;
      plasticityRateAcquired_    = originalJG.plasticityRateAcquired_;

      delayedCreationTime_       = originalJG.delayedCreationTime_;
      damping_                   = originalJG.damping_;
    }

    public override bool UpdateNodeReferences(Dictionary<CommandNode, CommandNode> dictNodeToClonedNode)
    {
      bool updatedA = objectsA_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updatedB = objectsB_.UpdateNodeReferences(dictNodeToClonedNode);
      bool updatedC = locatorsC_.UpdateNodeReferences(dictNodeToClonedNode);

      return ( updatedA || updatedB || updatedC );
    }

  }// class CNJointGroups...

}//namespace CaronteFX...