using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView
{
	/// <summary> <para>Base class for params to beused with a <see cref="GridAdapter{TParams, TCellVH}"/></para>
	/// </summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class GridParams : BaseParams
	{
		/// <summary>The prefab to use for each cell</summary>
		[Header("Grid")]
		public RectTransform cellPrefab;

		/// <summary>The max. number of cells in a row (for vertical ScrollView) or column (for horizontal ScrollView)</summary>
		[Tooltip("The max. number of cells in a row group (for vertical ScrollView) or column group (for horizontal ScrollView)")]
		public int numCellsPerGroup;

		/// <summary>The alignment of cells inside their parent LayoutGroup (Vertical or Horizontal, depending on ScrollView's orientation)</summary>
		public TextAnchor alignmentOfCellsInGroup = TextAnchor.UpperLeft;

		/// <summary>The padding of cells as a whole inside their parent LayoutGroup</summary>
		public RectOffset groupPadding;

		/// <summary>Wether to force the cells to expand in width inside their parent LayoutGroup</summary>
		public bool cellWidthForceExpandInGroup;

		/// <summary>Wether to force the cells to expand in height inside their parent LayoutGroup</summary>
		public bool cellHeightForceExpandInGroup;

		// Both of these should be at least 1
		int NumCellsPerGroupHorizontally { get { return scrollRect.horizontal ? 1 : numCellsPerGroup; } }
		int NumCellsPerGroupVertically { get { return scrollRect.horizontal ? numCellsPerGroup : 1; } }

		/// <summary>Cached prefab, auto-generated at runtime, first time <see cref="GetGroupPrefab(int)"/> is called</summary>
		HorizontalOrVerticalLayoutGroup _TheOnlyGroupPrefab;


		/// <inheritdoc/>
		public override void InitIfNeeded(ISRIA sria)
		{
			base.InitIfNeeded(sria);
			_DefaultItemSize = scrollRect.horizontal ? cellPrefab.rect.width : cellPrefab.rect.height;

			// Hotfix 12.10.2017 14:45: There's a bug in Unity on some versions: creating a new GameObject at runtime and adding it a RectTransform cannot be done in Awake() or OnEnabled().
			// See: https://issuetracker.unity3d.com/issues/error-when-creating-a-recttransform-component-in-an-awake-call-of-an-instantiated-monobehaviour
			// The bug was initially found in a project where the initial count is 0 (when Start() is called), then the scrollview is disabled, set a non-zero count, then enabled back,
			// and in OnEnable() the user called ResetItems(), which triggered the lazy-instantiation of the group prefab - since it's created in the first GetGroupPrefab() call.
			// Solved it by creating the prefab here, because InitIfNeeded(ISRIA) is called at Init time (in MonoBehaviour.Start())
			CreateCellGroupPrefab();
		}

		/// <summary>Returns the prefab to use as LayoutGroup for the group with index <paramref name="forGroupAtThisIndex"/></summary>
		public virtual HorizontalOrVerticalLayoutGroup GetGroupPrefab(int forGroupAtThisIndex)
		{
			if (_TheOnlyGroupPrefab == null)
				throw new UnityException("GridParams.InitIfNeeded() was not called by SRIA. Did you forget to call base.Start() in <YourAdapter>.Start()?");

			return _TheOnlyGroupPrefab;
		}

		/// <summary>Approximates the group width using the provided padding, spacing, cell prefab's width, and the <see cref="NumCellsPerGroupHorizontally"/> </summary>
		public virtual float GetGroupWidth()
		{ return groupPadding.left + (cellPrefab.rect.width + contentSpacing) * NumCellsPerGroupHorizontally - contentSpacing + groupPadding.right; }

		/// <summary>Similar to <see cref="GetGroupWidth"/></summary>
		public virtual float GetGroupHeight()
		{ return groupPadding.top + (cellPrefab.rect.height + contentSpacing) * NumCellsPerGroupVertically - contentSpacing + groupPadding.bottom; }

		//public virtual int GetCellIndex(int groupIndex) { }
		public virtual int GetGroupIndex(int cellIndex) { return cellIndex / numCellsPerGroup; }

		public virtual int GetNumberOfRequiredGroups(int numberOfCells) { return numberOfCells == 0 ? 0 : GetGroupIndex(numberOfCells - 1) + 1; }

		protected void CreateCellGroupPrefab()
		{
			var go = new GameObject(scrollRect.name + "_CellGroupPrefab", typeof(RectTransform));

			// Additional reminder of the "add recttransform in awake" bug explained in InitIfNeeded()
			if (!(go.transform is RectTransform))
				Debug.LogException(new UnityException("SRIA: Don't call SRIA.Init() outside MonoBehaviour.Start()!"));

			go.SetActive(false);
			go.transform.SetParent(scrollRect.transform, false);
			if (scrollRect.horizontal)
				_TheOnlyGroupPrefab = go.AddComponent<VerticalLayoutGroup>(); // groups are columns in a horizontal scrollview
			else
				_TheOnlyGroupPrefab = go.AddComponent<HorizontalLayoutGroup>(); // groups are rows in a vertical scrollview
			_TheOnlyGroupPrefab.spacing = contentSpacing;
			_TheOnlyGroupPrefab.childForceExpandWidth = cellWidthForceExpandInGroup;
			_TheOnlyGroupPrefab.childForceExpandHeight = cellHeightForceExpandInGroup;
			_TheOnlyGroupPrefab.childAlignment = alignmentOfCellsInGroup;
			_TheOnlyGroupPrefab.padding = groupPadding;
		}
	}
}