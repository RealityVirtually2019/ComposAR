//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: An area that the player can teleport to
//
//=============================================================================

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class TeleportArea : TeleportMarkerBase
	{
		//Public properties
		public Bounds meshBounds { get; private set; }

		//Private data
		private MeshRenderer areaMesh;
		private int tintColorId = 0;
		private Color visibleTintColor = Color.clear;
		private Color highlightedTintColor = Color.clear;
		private Color lockedTintColor = Color.clear;
		private bool highlighted = false;

		//-------------------------------------------------
		public void Awake()
		{
		}


		//-------------------------------------------------
		public void Start()
		{
		}


		//-------------------------------------------------
		public override bool ShouldActivate( Vector3 playerPosition )
		{
			return true;
		}


		//-------------------------------------------------
		public override bool ShouldMovePlayer()
		{
			return true;
		}


		//-------------------------------------------------
		public override void Highlight( bool highlight )
		{
		}


		//-------------------------------------------------
		public override void SetAlpha( float tintAlpha, float alphaPercent )
		{
		}


		//-------------------------------------------------
		public override void UpdateVisuals()
		{
		}


		//-------------------------------------------------
		public void UpdateVisualsInEditor()
		{
		}


		//-------------------------------------------------
		private bool CalculateBounds()
		{
			return true;
		}


		//-------------------------------------------------
		private Color GetTintColor()
		{
			return highlightedTintColor;
		}
	}

}
