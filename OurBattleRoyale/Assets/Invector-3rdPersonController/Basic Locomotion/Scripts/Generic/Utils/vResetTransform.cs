﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.Utils
{
    [vClassHeader("Reset Transform",useHelpBox =true, helpBoxText = "Use this to Reset transformation values<b><color=red>\nPosition Zero\nRotation Zero\nScale One</color> </b>", openClose =false)]
    public class vResetTransform : vMonoBehaviour
    {
        public bool resetPositionOnStart;
        public bool resetRotationOnStart;
        public bool resetScaleOnStart;

        private void Start()
        {
            if (resetPositionOnStart) ResetPosition();
            if (resetRotationOnStart) ResetRotation();
            if (resetScaleOnStart) ResetScale();
        }

        public void ResetRotation()
        {
            transform.eulerAngles = Vector3.zero;
        }
        public void ResetPosition()
        {
            transform.position = Vector3.zero;
        }
        public void ResetScale()
        {
            transform.localScale = Vector3.one;
        }
    }

}
