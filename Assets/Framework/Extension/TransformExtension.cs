using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Framework.Extension
{
    public static class TransformExtension
    {
        public static void Show(this Transform transform)
        {
            transform.gameObject.SetActive(true);
        }

        public static void Hide(this Transform transform)
        {
            transform.gameObject.SetActive(false);
        }

        public static void AddChild(this Transform parent, Transform child)
        {
            child.SetParent(parent);
        }

        #region SetLocalPosition

        public static void SetLocalPosX(this Transform transform, float x)
        {
            var localPos = transform.localPosition;
            localPos.x = x;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosY(this Transform transform, float y)
        {
            var localPos = transform.localPosition;
            localPos.y = y;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosZ(this Transform transform, float z)
        {
            var localPos = transform.localPosition;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosXY(this Transform transform, float x, float y)
        {
            var localPos = transform.localPosition;
            localPos.x = x;
            localPos.y = y;
            transform.localPosition = localPos;
        }
        public static void SetLocalPosXZ(this Transform transform, float x, float z)
        {
            var localPos = transform.localPosition;
            localPos.x = x;
            localPos.z = z;
            transform.localPosition = localPos;
        }
        public static void SetLocalPosYZ(this Transform transform, float y, float z)
        {
            var localPos = transform.localPosition;
            localPos.y = y;
            localPos.z = z;
            transform.localPosition = localPos;
        }
        #endregion

        public static void SetPosX(this Transform transform, float x)
        {
            var Pos = transform.position;
            Pos.x = x;
            transform.position = Pos;
        }

        public static void SetPosY(this Transform transform, float y)
        {
            var Pos = transform.position;
            Pos.y = y;
            transform.position = Pos;
        }

        public static void SetPosZ(this Transform transform, float z)
        {
            var Pos = transform.position;
            Pos.z = z;
            transform.position = Pos;
        }

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.one;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }

        public static void ResetPivot2D(this Transform transform)
        {
            transform.up = Vector3.up;
            transform.right = Vector3.right;
        }

        public static void ResetPivot3D(this Transform transform)
        {
            transform.up = Vector3.up;
            transform.right = Vector3.right;
            transform.forward = Vector3.forward;
        }

    }
}