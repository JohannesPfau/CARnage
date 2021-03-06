using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaronteFX
{
  public class CNBodyEditorState : CommandNodeEditorState
  {
    public bool    isSolidVsShell_;
    public float   mass_;
    public float   density_;
    public float   restitution_in01_;
    public float   frictionKinetic_in01_;
    public float   frictionStatic_in01_;
    public bool    fromKinetic_;
    public float   dampingPerSecond_WORLD_;
    public Vector3 velocityStart_;
    public Vector3 omegaStart_inDegSeg_;
    public float   explosionOpacity_;
    public float   explosionResponsiveness_;
  }

  public abstract class CNBodyEditor : CNMonoFieldEditor
  {
    static readonly GUIContent bdObjectsFieldCt_                 = new GUIContent( CarStringManager.GetString("BdObjectsBodyField"),             CarStringManager.GetString("BdObjectsBodyFieldTooltip"));
    static readonly GUIContent bdCollideToggleCt_                = new GUIContent( CarStringManager.GetString("BdCollideToggle"),                CarStringManager.GetString("BdCollideToggleTooltip") );
    static readonly GUIContent bdSetMassByOptionCt_              = new GUIContent( CarStringManager.GetString("BdSetMassByOption"),              CarStringManager.GetString("BdSetMassByOptionTooltip") );
    static readonly GUIContent bdMassValueCt_                    = new GUIContent( CarStringManager.GetString("BdMass"),                         CarStringManager.GetString("BdMassTooltip") );
    static readonly GUIContent bdDensityValueCt_                 = new GUIContent( CarStringManager.GetString("BdDensity"),                      CarStringManager.GetString("BdDensityTooltip") );
    static readonly GUIContent bdRestitutionValueCt_             = new GUIContent( CarStringManager.GetString("BdRestitution"),                  CarStringManager.GetString("BdRestitutionTooltip") );
    static readonly GUIContent bdKineticFrictionValueCt_         = new GUIContent( CarStringManager.GetString("BdKineticFriction"),              CarStringManager.GetString("BdKineticFrictionTooltip") );
    static readonly GUIContent bdStaticFrictionValueCt_          = new GUIContent( CarStringManager.GetString("BdStaticFriction"),               CarStringManager.GetString("BdStaticFrictionTooltip"));
    static readonly GUIContent bdFromKineticCt_                  = new GUIContent( CarStringManager.GetString("BdFromKinetic") );
    static readonly GUIContent bdExternalDampingValueCt_         = new GUIContent( CarStringManager.GetString("BdExternalDamping"),              CarStringManager.GetString("BdExternalDampingTooltip"));
    static readonly GUIContent bdInitialLinearVelocityValueCt_   = new GUIContent( CarStringManager.GetString("BdInitialLinearVelocty"),         CarStringManager.GetString("BdInitialLinearVeloctyTooltip"));
    static readonly GUIContent bdInitialAngularVelocityValueCt_  = new GUIContent( CarStringManager.GetString("BdInitialLinearAngularVelocity"), CarStringManager.GetString("BdInitialLinearAngularVelocityTooltip"));
    static readonly GUIContent bdExplosionOpacityValueCt_        = new GUIContent( CarStringManager.GetString("BdExplosionOpacity"),             CarStringManager.GetString("BdExplosionOpacityTooltip"));
    static readonly GUIContent bdExplosionResponsivenessValueCt_ = new GUIContent( CarStringManager.GetString("BdExplosionResponsiveness"),      CarStringManager.GetString("BdExplosionResponsivenessTooltip") );

    protected static readonly GUIContent bdAutoCollideCt_               = new GUIContent(CarStringManager.GetString("BdAutocollide"), CarStringManager.GetString("BdAutocollideTooltip"));
    protected static readonly GUIContent bdDisableCollisionsAtJointsCt_ = new GUIContent(CarStringManager.GetString("BdDisableCollisionsAtJoints"), CarStringManager.GetString("BdDisableCollisionsAtJointsTooltip"));
    protected static readonly GUIContent bdInternalDampingCt_           = new GUIContent(CarStringManager.GetString("BdInternalDamping"), CarStringManager.GetString("BdInternalDampingTooltip"));

    protected GUIContent[] massOptions_ = new GUIContent[] { new GUIContent("Mass per body"), new GUIContent("Density") };

    protected int massOptionIdx_    = -1;
    protected int maxCreationStep_  = 100;


    protected new CNBodyEditorState state_;
    protected new CNBody Data { get; set; }

    public CNBodyEditor( CNBody data, CNBodyEditorState state )
      : base( data, state )
    {
      Data   = (CNBody)data;
      state_ = state;
    }
    //-----------------------------------------------------------------------------------
    public override void Init()
    {
      base.Init();

      FieldController.SetCalculatesDiff(true);
      FieldController.SetFieldContentType(CNFieldContentType.Geometry);
      FieldController.IsBodyField = true;    
    }
    //-----------------------------------------------------------------------------------
    protected void SetMassOption()
    {
      if (Data.Density == -1)
      {
        massOptionIdx_ = 0;
      }
      else if (Data.Mass == -1)
      {
        massOptionIdx_ = 1;
      }
    }
    //-----------------------------------------------------------------------------------
    protected override void LoadState()
    {
      base.LoadState();

      state_.isSolidVsShell_              = Data.IsSolidVsShell;
      state_.mass_                        = Data.Mass;
      state_.density_                     = Data.Density;
      state_.restitution_in01_            = Data.Restitution_in01;
      state_.frictionKinetic_in01_        = Data.FrictionKinetic_in01;
      state_.frictionStatic_in01_         = Data.FrictionStatic_in01;
      state_.fromKinetic_                 = Data.FromKinetic;
      state_.dampingPerSecond_WORLD_      = Data.DampingPerSecond_WORLD;
      state_.velocityStart_               = Data.VelocityStart;
      state_.omegaStart_inDegSeg_         = Data.OmegaStart_inDegSeg;
      state_.explosionOpacity_            = Data.ExplosionOpacity;
      state_.explosionResponsiveness_     = Data.ExplosionResponsiveness;
    }
    //-----------------------------------------------------------------------------------
    public override void ValidateState()
    {
      base.ValidateState();

      ValidateSolidVsShell();
      ValidateMass();
      ValidateDensity();
      ValidateRestitution();
      ValidateFriction();
      ValidateDampingPerSecondWorld();
      ValidateVelocity();
      ValidateOmega();
      ValidateExplosionOpacity();
      ValidateExplosionResponsiveness();
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateSolidVsShell()
    {
      if ( state_.isSolidVsShell_ != Data.IsSolidVsShell )
      {
        DestroyBodies();
        CreateBodies();

        state_.isSolidVsShell_ = Data.IsSolidVsShell;
      }
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateMass()
    {
      if ( state_.mass_ != Data.Mass )
      {
        if (Data.Mass != -1)
        {
          eManager.SetMass( Data );
          Debug.Log("Changed Mass");
        }

        state_.mass_ = Data.Mass;
      }
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateDensity()
    {
      if ( state_.density_ != Data.Density )
      {
        if (Data.Density != -1)
        {
          eManager.SetDensity( Data );
          Debug.Log("Changed Density");
        }

        state_.density_ = Data.Density;
      }
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateRestitution()
    {
      if ( state_.restitution_in01_ != Data.Restitution_in01 )
      {
        eManager.SetRestitution( Data );
        Debug.Log("Changed Restitution");

        state_.restitution_in01_ = Data.Restitution_in01;
      }
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateFriction()
    {
      bool invalidKinetic = state_.frictionKinetic_in01_ != Data.FrictionKinetic_in01;
      bool invalidStatic = (state_.frictionStatic_in01_ != Data.FrictionStatic_in01) || (state_.fromKinetic_ != Data.FromKinetic);

      if ( invalidKinetic || invalidStatic )
      {
        if (invalidKinetic)
        {
          eManager.SetFrictionKinetic(Data);
          Debug.Log("Changed Friction Kinetic");
          state_.frictionKinetic_in01_ = Data.FrictionKinetic_in01;
        }

        if (Data.FromKinetic)
        {
          eManager.SetFrictionStaticFromKinetic( Data );
          Debug.Log("Changed Friction Static");
        }
        else
        {
          eManager.SetFrictionStatic( Data );
          Debug.Log("Changed Friction Static");
        }

        state_.frictionStatic_in01_ = Data.FrictionStatic_in01;
        state_.fromKinetic_ = Data.FromKinetic;
      }
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateDampingPerSecondWorld()
    {
      if ( state_.dampingPerSecond_WORLD_ != Data.DampingPerSecond_WORLD )
      {
        eManager.SetDampingPerSecondWorld( Data );
        Debug.Log("Changed Damping Per Second World");
        state_.dampingPerSecond_WORLD_ = Data.DampingPerSecond_WORLD;
      }
    }
    //-----------------------------------------------------------------------------------
    protected abstract void ValidateVelocity();
    //-----------------------------------------------------------------------------------
    protected abstract void ValidateOmega();
    //-----------------------------------------------------------------------------------
    protected void ValidateExplosionOpacity()
    {
      if ( state_.explosionOpacity_ != Data.ExplosionOpacity )
      {
        eManager.SetExplosionOpacity( Data );
        state_.explosionOpacity_ = Data.ExplosionOpacity;
      }
    }
    //-----------------------------------------------------------------------------------
    protected void ValidateExplosionResponsiveness()
    {
      if ( state_.explosionResponsiveness_ != Data.ExplosionResponsiveness )
      {
        eManager.SetExplosionResponsiveness( Data );
        state_.explosionResponsiveness_ = Data.ExplosionResponsiveness;
      }
    }
    //-----------------------------------------------------------------------------------
    public override void SceneSelection()
    {
      FieldController.SceneSelectionTopMost();
    }
    //-----------------------------------------------------------------------------------
    public override void FreeResources()
    {
      DestroyBodies();
      FieldController.DestroyField();
    }
    //----------------------------------------------------------------------------------
    public void CheckUpdate(out int[] idsUnityAdded, out int[] idsUnityRemoved)
    {
      bool updateNeeded = FieldController.IsUpdateNeeded(out idsUnityAdded, out idsUnityRemoved);
      if (updateNeeded)
      {
        if (Data != null)
        {
          Data.NeedsUpdate = true;
          EditorUtility.SetDirty(Data);
        }
      }
    }
    //-----------------------------------------------------------------------------------
    public void SetActivityIfDisabled()
    {
      if ( !IsEnabledInHierarchy )
      {
        eManager.SetActivity( Data );
      }
    }
    //-----------------------------------------------------------------------------------
    public override void SetVisibilityState()
    {
      base.SetVisibilityState();
      eManager.SetVisibility( Data );
    }
    //-----------------------------------------------------------------------------------
    public override void SetExcludedState()
    {
      base.SetExcludedState();
      if (IsExcludedInHierarchy)
      {
        DestroyBodies();
      }
      else
      {
        CreateBodies();
      }
    }
    //-----------------------------------------------------------------------------------
    public void EditCollisionFlags()
    {
      eManager.EditBodyCollisionFlags( Data );
    }
    //-----------------------------------------------------------------------------------
    protected void CreateBodies(GameObject[] arrGameObject, string windowTitle, string progressString)
    {
      if (arrGameObject.Length > 0 && !IsExcludedInHierarchy)
      {
        ValidateState();

        int nBody = arrGameObject.Length;
        StringBuilder strBuilder = new StringBuilder();


        int maxStep = Mathf.Min(nBody / 4, maxCreationStep_);
        int progStep = Mathf.Max(maxStep, 1);

        for (int i = 0; i < nBody; ++i)
        {
          if (i % progStep == 0)
          {
            strBuilder.AppendFormat("{0} {1} of {2}.", progressString, (i + 1), nBody);
            float progress = (float)i / (float)nBody;
            EditorUtility.DisplayProgressBar(windowTitle, strBuilder.ToString(), progress);
            strBuilder.Length = 0;
          }

          GameObject go = arrGameObject[i];
          ActionCreateBody(go);
        }
        EditorUtility.ClearProgressBar();

        LoadState();
      }

      if (Data != null)
      {
        Data.NeedsUpdate = false;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DestroyBodies(GameObject[] arrGameObject, string windowTitle, string progressString)
    {
      if (arrGameObject.Length > 0)
      {
        int nBody = arrGameObject.Length;
        StringBuilder strBuilder = new StringBuilder();

        int maxStep = Mathf.Min(nBody / 4, maxCreationStep_);
        int progStep = Mathf.Max(maxStep, 1);

        for (int i = 0; i < nBody; i++)
        {
          if (i % progStep == 0)
          {
            strBuilder.AppendFormat("{0} {1} of {2}.", progressString, (i + 1), nBody);
            float progress = (float)i / (float)nBody;
            EditorUtility.DisplayProgressBar(windowTitle, strBuilder.ToString(), progress);
            strBuilder.Length = 0;
          }

          GameObject go = arrGameObject[i];
          ActionDestroyBody(go);
        }

        EditorUtility.ClearProgressBar();

        LoadState();
      }

      if (Data != null)
      {
        Data.NeedsUpdate = false;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DestroyBodies(int[] arrInstanceId, string windowTitle, string progressString)
    {
      if (arrInstanceId.Length> 0)
      {
        int nBody = arrInstanceId.Length;
        StringBuilder strBuilder = new StringBuilder();

        int maxStep = Mathf.Min(nBody / 4, maxCreationStep_);
        int progStep = Mathf.Max(maxStep, 1);

        for (int i = 0; i < nBody; i++)
        {
          if (i % progStep == 0)
          {
            strBuilder.AppendFormat("{0} {1} of {2}.", progressString, (i + 1), nBody);
            float progress = (float)i / (float)nBody;
            EditorUtility.DisplayProgressBar(windowTitle, strBuilder.ToString(), progress);
            strBuilder.Length = 0;
          }

          int instanceId = arrInstanceId[i];
          ActionDestroyBody(instanceId);
        }
        EditorUtility.ClearProgressBar();

        LoadState();
      }


      if (Data != null)
      {
        Data.NeedsUpdate = false;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    public void RecreateBodies()
    {
      DestroyBodies();
      CreateBodies();
    }
    //-----------------------------------------------------------------------------------
    public virtual void CreateBodies()
    {
      if ( !IsExcludedInHierarchy )
      {
        GameObject[] gameObjects = FieldController.GetUnityGameObjects();
        CreateBodies(gameObjects);
        LoadState();
      }

      if (Data != null)
      {
        Data.NeedsUpdate = false;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    public void DestroyBodies()
    {
      GameObject[] gameObjects = FieldController.GetUnityGameObjects();
      DestroyBodies( gameObjects );

      if (Data != null)
      {
        Data.NeedsUpdate = false;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    public void CheckBodiesForChanges(bool recreateIfInvalid)
    {   
      GameObject[] arrGameObject = FieldController.GetUnityGameObjects();

      foreach(GameObject go in arrGameObject)
      {
        if (go != null)
        {
          ActionCheckBodyForChanges(go, recreateIfInvalid);
        }
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawGUIMassOptions()
    {
      int prevMassOptionIdx = massOptionIdx_;
      EditorGUI.BeginChangeCheck();
      massOptionIdx_ = EditorGUILayout.Popup( bdSetMassByOptionCt_, massOptionIdx_, massOptions_ );
      if ( prevMassOptionIdx != massOptionIdx_ && EditorGUI.EndChangeCheck() )
      {
        if (massOptionIdx_ == 0)
        {
          Undo.RecordObject( Data, "Change to set by mass ");
          Data.Mass = Mathf.Clamp(Data.Density, 0f, float.MaxValue);
          Data.Density = -1;
          EditorUtility.SetDirty( Data );
        }
        else if ( massOptionIdx_ == 1)
        {
          Undo.RecordObject( Data, "Change to set by density");
          Data.Density = Mathf.Clamp(Data.Mass, 0f, float.MaxValue);
          Data.Mass = -1;
          EditorUtility.SetDirty( Data );
        }
      }

      if (massOptionIdx_ == 0)
      {
        EditorGUI.BeginChangeCheck();
        var value = EditorGUILayout.FloatField(bdMassValueCt_, Data.Mass );
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject( Data, "Change mass");
          Data.Mass = Mathf.Clamp(value, 0f, float.MaxValue);
          EditorUtility.SetDirty( Data );
        }
      }
      else if (massOptionIdx_ == 1)
      {
        EditorGUI.BeginChangeCheck();
        var value = EditorGUILayout.FloatField(bdDensityValueCt_, Data.Density );
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject( Data, "Change density");
          Data.Density = Mathf.Clamp(value, 0f, float.MaxValue);
          EditorUtility.SetDirty( Data );
        }
      }
    } 
    //-----------------------------------------------------------------------------------
    protected void DrawIsShell()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( new GUIContent("Shell mode"), !Data.IsSolidVsShell);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change shell mode - " + Data.Name);
        Data.IsSolidVsShell = !value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDoCollide()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( bdCollideToggleCt_, Data.DoCollide );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change collide - " + Data.Name);
        Data.DoCollide = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawRestitution()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( bdRestitutionValueCt_, Data.Restitution_in01, 0f, 1f );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change restitution - " + Data.Name);
        Data.Restitution_in01 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawFrictionKinetic()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( bdKineticFrictionValueCt_, Data.FrictionKinetic_in01, 0f, 1f );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change kinetic friction - " + Data.Name);
        Data.FrictionKinetic_in01 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawFrictionStatic()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( bdStaticFrictionValueCt_, Data.FrictionStatic_in01, 0f, 1f );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change static friction - " + Data.Name);
        Data.FrictionStatic_in01 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawFrictionStaticFromKinetic()
    {
      EditorGUI.BeginChangeCheck();
      bool value = Data.FromKinetic;
      value = EditorGUILayout.ToggleLeft( bdFromKineticCt_, Data.FromKinetic, GUILayout.Width(100f) );
      if ( EditorGUI.EndChangeCheck() )
      {   
        Undo.RecordObject( Data, "Change from kinetic - " + Data.Name);
        Data.FromKinetic = value;
        EditorUtility.SetDirty( Data );
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawDampingPerSecondWorld()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( bdExternalDampingValueCt_, Data.DampingPerSecond_WORLD );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change external damping - " + Data.Name);
        Data.DampingPerSecond_WORLD = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawExplosionOpacity()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( bdExplosionOpacityValueCt_, Data.ExplosionOpacity, 0f, 1f );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Explosion opacity - " + Data.Name);
        Data.ExplosionOpacity = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawExplosionResponsiveness()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( bdExplosionResponsivenessValueCt_, Data.ExplosionResponsiveness, 0f, 1f );
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Explosion responsiveness - " + Data.Name);
        Data.ExplosionResponsiveness = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawLinearVelocity()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Vector3Field(bdInitialLinearVelocityValueCt_, Data.VelocityStart);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change linear velocity - " + Data.Name);
        Data.VelocityStart = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected void DrawAngularVelocity()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Vector3Field(bdInitialAngularVelocityValueCt_, Data.OmegaStart_inDegSeg);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change angular velocity - " + Data.Name);
        Data.OmegaStart_inDegSeg = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    public override void RenderGUI ( Rect area, bool isEditable )
    {
      GUILayout.BeginArea( area );

      RenderTitle(isEditable);

      EditorGUI.BeginDisabledGroup(!isEditable);
      RenderFieldObjects( bdObjectsFieldCt_, FieldController, true, true, CNFieldWindow.Type.normal );
      EditorGUI.EndDisabledGroup();

      SetMassOption();
      RenderFieldsBody(isEditable);  
      GUILayout.EndArea();
    }
    //-----------------------------------------------------------------------------------
    protected abstract void RenderFieldsBody(bool isEditable);
    //-----------------------------------------------------------------------------------
    public abstract void CreateBodies( GameObject[] arrGameObject );
    //-----------------------------------------------------------------------------------
    public abstract void DestroyBodies( GameObject[] arrGameObject );
    //-----------------------------------------------------------------------------------
    public abstract void DestroyBodies( int[] arrInstanceId );
    //-----------------------------------------------------------------------------------
    protected abstract void ActionCreateBody( GameObject go );
    //-----------------------------------------------------------------------------------
    protected abstract void ActionDestroyBody( GameObject go );
    //-----------------------------------------------------------------------------------
    public void ActionRecreateBody(GameObject go)
    {
      ActionDestroyBody(go);
      ActionCreateBody(go);
    }
    //-----------------------------------------------------------------------------------
    protected abstract void ActionDestroyBody( int instanceId );
    //-----------------------------------------------------------------------------------
    protected abstract void ActionCheckBodyForChanges( GameObject go, bool recreateIfInvalid );


  }
}

