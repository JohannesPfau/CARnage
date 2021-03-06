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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CaronteSharp;

namespace CaronteFX
{
  public class CRBodyData
  {
    public uint              idBody_   = uint.MaxValue;
    public BodyType          bodyType_ = BodyType.None;
    public List<CommandNode> listNode_ = new List<CommandNode>();
  }

  [CanEditMultipleObjects]
  [CustomEditor(typeof(Caronte_Fx_Body))]
  public class Caronte_Fx_Body_Editor : Editor
  {
    SerializedProperty serializedColliderType_;
    SerializedProperty serializedColliderMesh_;
    SerializedProperty serializedColliderGameObject_;
    SerializedProperty serializedColliderColor_;
    SerializedProperty serializedColliderRenderMode_;
    SerializedProperty serializedTileMesh_;
    SerializedProperty serializedBalltreeAsset_;
    SerializedProperty serializedRenderCollider_;
    SerializedProperty serializedSmoothRopeRenderMesh_;

    List<CRBodyData>  listBodyData_ = new List<CRBodyData>();

    CarManagerEditor window_;
    CarManager       manager_;
    CNHierarchy     hierarchy_;

    Vector2         scrollVecDefinition_;
    Vector2         scrollVecReferenced_;

    Caronte_Fx_Body cfxBody_;

    void OnEnable()
    {
      serializedColliderType_         = serializedObject.FindProperty("colliderType_");
      serializedColliderMesh_         = serializedObject.FindProperty("colliderMesh_");
      serializedColliderGameObject_   = serializedObject.FindProperty("colliderGameObject_");
      serializedColliderColor_        = serializedObject.FindProperty("colliderColor_");
      serializedColliderRenderMode_   = serializedObject.FindProperty("colliderRenderMode_");
      serializedTileMesh_             = serializedObject.FindProperty("tileMesh_");
      serializedBalltreeAsset_        = serializedObject.FindProperty("btAsset_");
      serializedRenderCollider_       = serializedObject.FindProperty("renderCollider_");
      serializedSmoothRopeRenderMesh_ = serializedObject.FindProperty("smoothRopeRenderMesh_");

      cfxBody_ = (Caronte_Fx_Body)target;
    }

    void OnDisable()
    {
      if (window_ != null)
      {
        window_.WantRepaint -= Repaint;
      }
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      bool isEditing = false;

      if (CarManagerEditor.IsOpen)
      {
        window_ = CarManagerEditor.Instance;
        window_.WantRepaint -= Repaint;
        window_.WantRepaint += Repaint;
      }
      
      if (CarManager.IsInitedStatic)
      {
        manager_   = CarManager.Instance;
        hierarchy_ = manager_.Hierarchy;
        manager_.GetBodiesData( listBodyData_ );

        isEditing = manager_.Player.IsEditing;
      }
      else
      {
        listBodyData_.Clear();
      }

      CRBodyData bdData = null;
      uint idBody = uint.MaxValue;

      int nBodyData = listBodyData_.Count;

      BodyType bodyType;
      string   bodyTypeText;
      string   bodyIdText;

      if (nBodyData == 0)
      {
        bodyType     = BodyType.None;
        bodyTypeText = "-";
        bodyIdText   = "-";
      }
      else
      {
        bdData = listBodyData_[0];

        bodyType     = bdData.bodyType_;
        bodyTypeText = GetBodyTypeString(bdData.bodyType_);
        idBody       = bdData.idBody_;

        for (int i = 1; i < nBodyData; i++)
        {
          bdData = listBodyData_[i];

          if (bdData.bodyType_ != bodyType)
          {
            bodyType     = BodyType.None;
            bodyTypeText = "-";
            bodyIdText   = "-"; 
            break;
          }
        }

        if (idBody == uint.MaxValue || nBodyData > 1)
        {
          bodyIdText = "-";
        }
        else
        {
          bodyIdText = idBody.ToString();
        }
      }

      HashSet<CommandNode> setBodyDefinition = new HashSet<CommandNode>();
      HashSet<CommandNode> setBodyReference  = new HashSet<CommandNode>();

      for (int i = 0; i < nBodyData; i++)
      {
        bdData = listBodyData_[i];
        List<CommandNode> bdDataNodes = bdData.listNode_;
        int nDataNodes = bdDataNodes.Count;
        for (int j = 0; j < nDataNodes; j++)
        {
          CommandNode node = bdDataNodes[j];
          if (j == 0)
          {
            setBodyDefinition.Add(node);
          }
          else
          {
            setBodyReference.Add(node);
          }
        }
      }

      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Body type: ", bodyTypeText );
      EditorGUILayout.LabelField("Body id:",    bodyIdText );

      EditorGUILayout.Space();

      if (bodyType == BodyType.None)
      {
        DrawFullBodySection();
      }
      else if (bodyType == BodyType.Ropebody)
      {
        DrawRopeColliderSection(isEditing);
      }
      else if ( IsNotBodyMesh(bodyType) )
      {
        DrawBodyColliderSection();
      }
     
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Body Definition: ");

      scrollVecDefinition_ = GUILayout.BeginScrollView(scrollVecDefinition_, GUILayout.ExpandHeight(false));

      DrawNodeGUI( setBodyDefinition );
      GUILayout.EndScrollView();

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Referenced in: ");

      scrollVecReferenced_ = GUILayout.BeginScrollView(scrollVecReferenced_, GUILayout.ExpandHeight(false));
      DrawNodeGUI( setBodyReference );
      GUILayout.EndScrollView();
      CarGUIUtils.Splitter();

      if (!CarManagerEditor.IsOpen)
      {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("Open CaronteFx Editor", GUILayout.Height(30f)))
        {
          window_ = (CarManagerEditor)CarManagerEditor.Init();
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
      }

      serializedObject.ApplyModifiedProperties();
  }

  private void DrawFullBodySection()
  {
    DrawCustomColliderFields();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedTileMesh_, new GUIContent("Render tile Mesh") );
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedSmoothRopeRenderMesh_, new GUIContent("Smooth render mesh"));
    EditorGUILayout.EndHorizontal();
    }

  private bool IsNotBodyMesh(BodyType bodyType)
  {
     return ( bodyType != BodyType.BodyMeshStatic &&
              bodyType != BodyType.BodyMeshAnimatedByTransform && 
              bodyType != BodyType.BodyMeshAnimatedByArrPos );
  }

  private void DrawBodyColliderSection()
  {
    DrawCustomColliderFields();
  }

  private void DrawCustomColliderFields()
  {
    Caronte_Fx_Body component = (Caronte_Fx_Body)target;
    SkinnedMeshRenderer smr = component.GetComponent<SkinnedMeshRenderer>();

    if (smr == null)
    {
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PropertyField(serializedColliderType_, new GUIContent("Collider"));
      EditorGUILayout.EndHorizontal();

      Color curColor = GUI.contentColor;
      if (serializedColliderType_.enumValueIndex == 2)
      {
        GUI.contentColor = Color.green;
      }
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PropertyField(serializedColliderMesh_, new GUIContent("Custom Mesh"));
      GUI.contentColor = curColor;
      EditorGUILayout.EndHorizontal();
        
      if (serializedColliderType_.enumValueIndex == 3)
      {
        GUI.contentColor = Color.green;
      }
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PropertyField(serializedColliderGameObject_, new GUIContent("Custom GameObject"));
      GUI.contentColor = curColor;
      EditorGUILayout.EndHorizontal();

      if (serializedColliderType_.enumValueIndex == 4)
      {
        GUI.contentColor = Color.green;
      }
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PropertyField(serializedBalltreeAsset_, new GUIContent("Balltree Asset"));
      GUI.contentColor = curColor;
      EditorGUILayout.EndHorizontal();
    
      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUILayout.PropertyField(serializedRenderCollider_, new GUIContent("Render collider"));

      EditorGUI.BeginDisabledGroup(cfxBody_.IsUsingBalltree());
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PropertyField(serializedColliderColor_, new GUIContent("Color"));
      EditorGUILayout.EndHorizontal();
      EditorGUI.EndDisabledGroup();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PropertyField(serializedColliderRenderMode_, new GUIContent("Render mode"));
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.Space();
    }

   }

  private void DrawRopeColliderSection(bool isEditing)
  {
    EditorGUI.BeginDisabledGroup(!isEditing);
    EditorGUILayout.Space();
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedTileMesh_, new GUIContent("Render tile Mesh") );
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedSmoothRopeRenderMesh_, new GUIContent("Smooth render mesh"));
    EditorGUILayout.EndHorizontal();
    EditorGUI.EndDisabledGroup();

    EditorGUILayout.Space();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedRenderCollider_, new GUIContent("Render collider"));
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedColliderColor_, new GUIContent("Color") );
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(serializedColliderRenderMode_, new GUIContent("Render mode") );
    EditorGUILayout.EndHorizontal();


  }

  private void DrawNodeGUI(HashSet<CommandNode> setNode )
  {
    if (setNode.Count == 0)
    {
      GUILayout.Label("-");
    }
    foreach( CommandNode node in setNode)
    {
      GUILayout.BeginHorizontal();
      Rect boxRect = GUILayoutUtility.GetRect(new GUIContent(""), EditorStyles.textField, GUILayout.ExpandWidth(true));
      Rect labelRect = new Rect(boxRect.xMin + 5, boxRect.yMin, boxRect.width * 0.75f - 5, boxRect.height);
      GUI.Box(labelRect, "");
      GUI.Label(labelRect, node.Name);
      Rect buttonRect = new Rect(labelRect.xMax + 10, boxRect.yMin, boxRect.width * 0.25f - 15, boxRect.height);
        
      if (GUI.Button(buttonRect, "Select", EditorStyles.miniButton))
      {
        window_ = (CarManagerEditor) CarManagerEditor.Init();
        hierarchy_.FocusAndSelect(node);
        window_.Repaint();
      }

      GUILayout.EndHorizontal();
    }
  }

  string GetBodyTypeString(BodyType bodyType)
  {
    switch(bodyType)
    {
      case BodyType.Rigidbody:
        return "Rigid Body";

      case BodyType.Softbody:
        return "Soft Body";
 
      case BodyType.BodyMeshStatic:
        return "Static Body Mesh";

      case BodyType.BodyMeshAnimatedByArrPos:
        return "Animated Body Mesh";

      case BodyType.BodyMeshAnimatedByTransform:
        return "Animated Body Mesh";

      case BodyType.Clothbody:
        return "Cloth Body";

      case BodyType.Ropebody:
        return "Rope Body";

      default:
        return "None";
    }
  }

  }
}
