using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PJH.Core
{
    public static class MUtil
    {
        public static Ray GetScreenPointToRay(Vector3 pos)
        {
            return Camera.main.ScreenPointToRay(pos);
        }


        public static Vector3 GetWorldToScreenPoint(Vector3 pos)
        {
            return Camera.main.WorldToScreenPoint(pos);
        }


        public static bool GetMousePositionRaycast(Vector3 aimPos, LayerMask whatIsMask, out RaycastHit hit)
        {
            Vector3 mousePos = aimPos;
            mousePos.z = Camera.main.nearClipPlane;

            Ray ray = GetScreenPointToRay(mousePos);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsMask);
        }

        public static Vector3 GetMouseRayPoint(Vector3 aimPos)
        {
            if (GetMousePositionRaycast(aimPos, Define.MLayerMask.WhatIsGround, out RaycastHit hit))
            {
                return hit.point;
            }

            return default;
        }

        public static Vector3 GetMouseRayPoint(Vector3 aimPos, LayerMask layerMask)
        {
            if (GetMousePositionRaycast(aimPos, layerMask, out RaycastHit hit))
            {
                return hit.point;
            }

            return default;
        }

        public static bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

        public static void SetCursorLockMode(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;

            Cursor.visible = (lockMode != CursorLockMode.Locked);
        }

        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }

            return component;
        }

        public static int GetEnumFlagIndex<T>(T e) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return Array.IndexOf(values, e);
        }

        public static Type GetTypeFromAssemblies(string TypeName)
        {
            var type = Type.GetType($" {TypeName},Assembly-CSharp");
            if (type != null)
                return type;

            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = System.Reflection.Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }
    }
}