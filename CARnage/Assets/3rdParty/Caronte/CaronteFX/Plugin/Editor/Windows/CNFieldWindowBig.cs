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

namespace CaronteFX
{
  public class CNFieldWindowBig : CNFieldWindow
  {
    float minWidth  = 450f;
    float minHeight = 450f;

    //-----------------------------------------------------------------------------------
    public override void Init( CNFieldController controller, CommandNodeEditor ownerEditor )
    {
      Instance.minSize = new Vector2(minWidth, minHeight);
      View = new CNFieldExtendedView( controller, ownerEditor ); 
      Controller = controller;
    }
  }
}

