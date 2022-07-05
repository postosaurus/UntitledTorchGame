using System;
using System.Collections.Generic;
using UnityEngine;

namespace Posty.Utils {

    public static class UtilsClass {
    
        private static Camera mainCamera;

        public static Vector3 GetMouseWorldPosition() {
            if (mainCamera == null) mainCamera = Camera.main;

            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            return mouseWorldPosition;
        }

        public static Vector3 GetRandomDirection() {
            return new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
            ).normalized;
        }
        public static Vector3 GetVectorFromAngle(float angle) {
            float angleRad = angle * (Mathf.PI/180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVectorFloat(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }
    }


}