using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CaronteFX
{
  public class CNSoftbodyEditorState : CNBodyEditorState
  {
    public int   resolution_;
    public float lengthStiffness_;
    public float volumeStiffness_;
    public bool  plasticity_;
    public float threshold_in01_;
    public float acquired_in01_;
    public float compressionLimit_in01_;
    public float expansionLimit_in_1_100_;
    public float dampingPerSecond_CM_;
  }

  public class CNSoftbodyEditor : CNBodyEditor
  {

    static readonly GUIContent sbResolutionCt_                   = new GUIContent( CarStringManager.GetString("SbResolution"),                   CarStringManager.GetString("SbResolutionTooltip"));
    static readonly GUIContent sbRelativeLengthStiffnessCt_      = new GUIContent( CarStringManager.GetString("SbRelativeLengthStiffness"),      CarStringManager.GetString("SbRelativeLengthStiffnessTooltip"));
    static readonly GUIContent sbRelativeVolumeStiffnessCt_      = new GUIContent( CarStringManager.GetString("SbRelativeVolumeStiffness"),      CarStringManager.GetString("SbRelativeVolumeStiffnessTooltip"));
    static readonly GUIContent sbPlasticityCt_                   = new GUIContent( CarStringManager.GetString("SbPlasticity"),                   CarStringManager.GetString("SbPlasticityTooltip"));
    static readonly GUIContent sbPlasticityThresholdCt_          = new GUIContent( CarStringManager.GetString("SbPlasticityThreshold"),          CarStringManager.GetString("SbPlasticityThresholdTooltip"));
    static readonly GUIContent sbPlasticityAcquiredCt_           = new GUIContent( CarStringManager.GetString("SbPlasticityAcquired"),           CarStringManager.GetString("SbPlasticityAcquiredTooltip"));
    static readonly GUIContent sbPlasticityCompressionLimitCt_   = new GUIContent( CarStringManager.GetString("SbPlasticityCompressionLimit"),   CarStringManager.GetString("SbPlasticityCompressionLimitTooltip"));
    static readonly GUIContent sbPlasticityExpansionLimitCt_     = new GUIContent( CarStringManager.GetString("SbPlasticityExpansionLimit"),     CarStringManager.GetString("SbPlasticityExpansionLimitTooltip"));

    public static Texture icon_;
    public override Texture TexIcon 
    { 
      get{ return icon_; } 
    }

    new CNSoftbodyEditorState state_;
    new CNSoftbody Data { get; set; }
    //-----------------------------------------------------------------------------------
    public CNSoftbodyEditor( CNSoftbody data, CNSoftbodyEditorState state )
      : base(data, state)
    {
      Data   = (CNSoftbody)data;
      state_ = state;
    }
    //-----------------------------------------------------------------------------------
    protected override void LoadState()
    {
      base.LoadState();

      state_.resolution_               = Data.Resolution;
      state_.lengthStiffness_          = Data.LengthStiffness;
      state_.volumeStiffness_          = Data.VolumeStiffness;
      state_.plasticity_               = Data.Plasticity;
      state_.threshold_in01_           = Data.Threshold_in01;
      state_.acquired_in01_            = Data.Acquired_in01;
      state_.compressionLimit_in01_    = Data.CompressionLimit_in01;
      state_.expansionLimit_in_1_100_  = Data.ExpansionLimit_in_1_100;
      state_.dampingPerSecond_CM_      = Data.DampingPerSecond_CM;
    }
    //-----------------------------------------------------------------------------------
    public override void ValidateState()
    {
      base.ValidateState();

      ValidateResolution();
      ValidateLengthStiffness();
      ValidateVolumeStiffness();
      ValidatePlasticity();
      ValidateThreshold();
      ValidateAcquired();
      ValidateCompressionLimit();
      ValidateExpansionLimit();
      ValidateDampingPerSecondCM();
    }
    //-----------------------------------------------------------------------------------
    protected override void ValidateVelocity()
    {
      if ( state_.velocityStart_ != Data.VelocityStart )
      {
        eManager.SetVelocity( Data );
        Debug.Log("Changed linear velocity");
        state_.velocityStart_ = Data.VelocityStart;
      }
    }
    //-----------------------------------------------------------------------------------
    protected override void ValidateOmega()
    {
      if ( state_.omegaStart_inDegSeg_ != Data.OmegaStart_inDegSeg )
      {
        eManager.SetVelocity( Data );
        Debug.Log("Changed angular velocity");
        state_.omegaStart_inDegSeg_ = Data.OmegaStart_inDegSeg;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateResolution()
    {
      if ( state_.resolution_ != Data.Resolution )
      {
        eManager.SetResolution( Data );
        Debug.Log("Changed Resolution");  

        state_.resolution_ = Data.Resolution;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateLengthStiffness()
    {
      if ( state_.lengthStiffness_ != Data.LengthStiffness )
      { 
        eManager.SetLengthStiffness( Data );
        Debug.Log("Changed Length Stiffness");

        state_.lengthStiffness_ = Data.LengthStiffness;
      }
    }
    //-----------------------------------------------------------------------------------
    private void  ValidateVolumeStiffness()
    {
      if ( state_.volumeStiffness_ != Data.VolumeStiffness )
      {
        eManager.SetVolumeStiffness( Data );
        Debug.Log("Changed Volume Stiffness");

        state_.volumeStiffness_ = Data.VolumeStiffness;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidatePlasticity()
    {
      if ( state_.plasticity_ != Data.Plasticity )
      {
        eManager.SetPlasticity( Data );
        Debug.Log("Changed Plasticity");

        state_.plasticity_ = Data.Plasticity;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateThreshold()
    {
      if (state_.threshold_in01_ != Data.Threshold_in01 )
      {
        eManager.SetThreshold( Data );
        Debug.Log("Changed Threshold");

        state_.threshold_in01_ = Data.Threshold_in01;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateAcquired()
    {
      if (state_.acquired_in01_ != Data.Acquired_in01)
      {
        eManager.SetAcquired( Data );
        Debug.Log("Changed Acquired");

        state_.acquired_in01_ = Data.Acquired_in01;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateCompressionLimit()
    {
      if (state_.compressionLimit_in01_ != Data.CompressionLimit_in01 )
      {
        eManager.SetCompressionLimit( Data );
        Debug.Log("Changed compression limit");
        
        state_.compressionLimit_in01_ = Data.CompressionLimit_in01;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateExpansionLimit()
    {
      if (state_.expansionLimit_in_1_100_ != Data.ExpansionLimit_in_1_100 )
      {
        eManager.SetExpansionLimit( Data );
        Debug.Log("Changed expansion limit");

        state_.expansionLimit_in_1_100_ = Data.ExpansionLimit_in_1_100;
      }
    }
    //-----------------------------------------------------------------------------------
    private void ValidateDampingPerSecondCM()
    {
      if (state_.dampingPerSecond_CM_ != Data.DampingPerSecond_CM )
      { 
        eManager.SetInternalDamping( Data );
        Debug.Log("Changed internal damping");

        state_.dampingPerSecond_CM_ = Data.DampingPerSecond_CM;
      }
    }
    //-----------------------------------------------------------------------------------
    public override void CreateBodies(GameObject[] arrGameObject)
    {
      CreateBodies(arrGameObject, "Caronte FX - Softbody creation", "Creating " + Data.Name + " soft bodies. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(GameObject[] arrGameObject)
    {
      DestroyBodies(arrGameObject, "CaronteFX - Softbody destruction", "Destroying " + Data.Name + "soft bodies. ");
    }
    //-----------------------------------------------------------------------------------
    public override void DestroyBodies(int[] arrInstanceId)
    {
      DestroyBodies(arrInstanceId, "Caronte FX - Softbody destruction", "Destroying " + Data.Name + " soft bodies. ");
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionCreateBody( GameObject go )
    {
      eManager.CreateBody(Data, go);
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionDestroyBody( GameObject go )
    {
      eManager.DestroyBody(Data, go);
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionDestroyBody( int instanceId )
    {
      eManager.DestroyBody(Data, instanceId);
    }
    //-----------------------------------------------------------------------------------
    protected override void ActionCheckBodyForChanges( GameObject go, bool recreateIfInvalid )
    {
	    eManager.CheckBodyForChanges(Data, go, recreateIfInvalid);
    }
    //-----------------------------------------------------------------------------------
    private void DrawDoAutocollide()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle( bdAutoCollideCt_, Data.AutoCollide );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change auto collide - " + Data.Name);
        Data.AutoCollide = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawResolution()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.IntField( sbResolutionCt_, Data.Resolution );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change resolution - " + Data.Name);
        Data.Resolution = Mathf.Clamp(value, 4, 2048);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawLengthStiffness()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( sbRelativeLengthStiffnessCt_, Data.LengthStiffness );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change length stiffness - " + Data.Name);
        Data.LengthStiffness = Mathf.Clamp(value, 0f, 1000f);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawVolumeStiffness()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( sbRelativeVolumeStiffnessCt_, Data.VolumeStiffness );
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change volume stiffness - " + Data.Name);
        Data.VolumeStiffness = Mathf.Clamp(value, 0f, 1000f);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawDampingPerSecondCM()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.FloatField( bdInternalDampingCt_, Data.DampingPerSecond_CM);
      if ( EditorGUI.EndChangeCheck() )
      {
        Undo.RecordObject(Data, "Change internal damping - " + Data.Name);
        Data.DampingPerSecond_CM = Mathf.Clamp(value, 0f, float.MaxValue);
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawPlasticityFoldout()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Foldout( Data.PlasticityFoldout, sbPlasticityCt_);
      if (EditorGUI.EndChangeCheck())
      {
        Data.PlasticityFoldout = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawPlasticity()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Toggle(Data.Plasticity);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change plasticity - " + Data.Name);
        Data.Plasticity = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawThreshold()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( sbPlasticityThresholdCt_, Data.Threshold_in01, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change threshold - " + Data.Name);
        Data.Threshold_in01 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawAcquired()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( sbPlasticityAcquiredCt_, Data.Acquired_in01, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change acquired - " + Data.Name);
        Data.Acquired_in01 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawCompressionLimit()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( sbPlasticityCompressionLimitCt_, Data.CompressionLimit_in01, 0f, 1f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change compression limit - " + Data.Name);
        Data.CompressionLimit_in01 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    private void DrawExpansionLimit()
    {
      EditorGUI.BeginChangeCheck();
      var value = EditorGUILayout.Slider( sbPlasticityExpansionLimitCt_, Data.ExpansionLimit_in_1_100, 1f, 100f);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(Data, "Change expansion limit - " + Data.Name);
        Data.ExpansionLimit_in_1_100 = value;
        EditorUtility.SetDirty(Data);
      }
    }
    //-----------------------------------------------------------------------------------
    protected override void RenderFieldsBody(bool isEditable)
    {
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      CarGUIUtils.DrawSeparator();
      CarGUIUtils.Splitter();

      scroller_ = EditorGUILayout.BeginScrollView(scroller_);
      
      EditorGUILayout.Space();
      float originalLabelWidth    = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 180f;

      EditorGUI.BeginDisabledGroup(!isEditable);
      {
        DrawDoCollide();
        DrawDoAutocollide();
        EditorGUILayout.Space();

        DrawGUIMassOptions();
        DrawIsShell();

        GUILayout.Space(simple_space);
        DrawResolution();
        GUILayout.Space(simple_space);
        DrawLengthStiffness();
        DrawVolumeStiffness();
        GUILayout.Space(simple_space);
        DrawDampingPerSecondCM();
        GUILayout.Space(simple_space);

        CarGUIUtils.Splitter();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawPlasticityFoldout();
        EditorGUI.BeginDisabledGroup(!isEditable);
        {
          DrawPlasticity();
          EditorGUILayout.EndHorizontal();

          if (Data.PlasticityFoldout)
          { 
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!Data.Plasticity);
            DrawThreshold();
            DrawAcquired();
            DrawCompressionLimit();
            DrawExpansionLimit();
            EditorGUI.EndDisabledGroup();
          }   
        }
        EditorGUI.EndDisabledGroup();

        CarGUIUtils.Splitter();
        GUILayout.Space(simple_space);

        DrawRestitution();
        DrawFrictionKinetic();
     
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(Data.FromKinetic);
        DrawFrictionStatic();
        EditorGUI.EndDisabledGroup();
        DrawFrictionStaticFromKinetic();
        GUILayout.EndHorizontal();

        GUILayout.Space(simple_space);
        DrawDampingPerSecondWorld();
        GUILayout.Space(simple_space);

        bool currentMode = EditorGUIUtility.wideMode;
        EditorGUIUtility.wideMode = true;
        DrawLinearVelocity();
        DrawAngularVelocity();
        EditorGUIUtility.wideMode = currentMode;
      }
      EditorGUI.EndDisabledGroup();

      EditorGUILayout.Space();
      CarGUIUtils.Splitter();
      EditorGUILayout.Space();

      EditorGUI.BeginDisabledGroup(!isEditable);
      {
        DrawExplosionOpacity();
        DrawExplosionResponsiveness();
      }
      EditorGUI.EndDisabledGroup();

      EditorGUIUtility.labelWidth = originalLabelWidth;

      EditorGUILayout.Space();     
      EditorGUILayout.EndScrollView();
    }
    //-----------------------------------------------------------------------------------
  }
}
