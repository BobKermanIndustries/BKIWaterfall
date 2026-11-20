using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using KSP.Localization;

namespace Blueshift
{
    public class WBIAnimatedTextureActivate : PartModule
    {

        [KSPField]
        public string moduleID = string.Empty;

        public override void OnActive()
        {
            base.OnActive();
            SetActivation(true);
        }

        private void SetActivation(bool state)
        {
            Debug.Log($"[WBIAnimatedTextureActivate] SetActivation({state}) called, moduleID={moduleID}");
            foreach (var tex in part.FindModulesImplementing<WBIAnimatedTexture>())
            {
                Debug.Log($"[WBIAnimatedTextureActivate] found WBIAnimatedTexture with moduleID={tex.moduleID}");
                if (tex.moduleID == moduleID)
                {
                    Debug.Log("[WBIAnimatedTextureActivate] match - activating");
                    tex.isActivated = state;
                    if (state)
                        tex.animationThrottle = 1f;
                }
            }
        }
    }
}