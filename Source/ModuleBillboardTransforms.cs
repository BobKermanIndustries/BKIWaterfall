using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using KSP.Localization;

namespace ModuleBillboardTransforms
{
    public class ModuleBillboardTransforms : PartModule
    {
        [KSPField]
        public string transformNames = string.Empty; //comma separated list

        [KSPField]
        public string billboardBaseName = string.Empty; // prefix-based naming

        [KSPField]
        public int billboardStartIndex = 1;

        [KSPField]
        public bool billboardInEditor = true;

        [KSPField]
        public bool billboardInFlight = true;

        private List<Transform> billboardTransforms = new List<Transform>();
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight)
                return;

            CacheTransforms();
        }
        private void CacheTransforms()
        {
            billboardTransforms.Clear();

            // name list
            if (!string.IsNullOrEmpty(transformNames))
            {
                string[] names = transformNames.Split(',');
                foreach (string rawName in names)
                {
                    string name = rawName.Trim();
                    if (string.IsNullOrEmpty(name))
                        continue;

                    Transform[] found = part.FindModelTransforms(name);
                    if (found != null && found.Length > 0)
                        billboardTransforms.AddRange(found);
                }
            }
            // numbered sequence with prefix
            if (!string.IsNullOrEmpty(billboardBaseName))
            {
                int index = billboardStartIndex;
                Transform found;
                while ((found = part.FindModelTransform(billboardBaseName + index)) != null)
                {
                    billboardTransforms.Add(found);
                    index++;
                }
            }

            Debug.Log($"[ModuleBillboardMeshes] Cached {billboardTransforms.Count} billboard transform(s) on part {part.partInfo?.name}");
        }
        private Camera GetActiveCamera()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightCamera.fetch != null)
                return FlightCamera.fetch.mainCamera;

            if (HighLogic.LoadedSceneIsEditor && EditorCamera.Instance != null)
            {
                Camera editorCam = EditorCamera.Instance.GetComponent<Camera>();
                if (editorCam != null)
                    return editorCam;
            }

            // fallback
            return Camera.main;
        }

        public void LateUpdate()
        {
            if (billboardTransforms == null || billboardTransforms.Count == 0)
                return;

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!billboardInFlight)
                    return;
                if (part.vessel == null || part.vessel.packed)
                    return; // skip during timewarp
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (!billboardInEditor)
                    return;
            }
            else
            {
                return;
            }

            Camera cam = GetActiveCamera();
            if (cam == null)
                return;
            Transform camTransform = cam.transform;
            int count = billboardTransforms.Count;
            for (int i = 0; i < count; i++)
            {
                Transform tb = billboardTransforms[i];
                if (tb == null)
                    continue;

                tb.rotation = Quaternion.LookRotation(tb.position - camTransform.position, camTransform.up);
            }

        }

    }
}
