using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text;
using System;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Visualizes the eye poses for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    public class EyeStatus : MonoBehaviour
    {
        enum EyeType { Left, Right }
        public UnityEvent OnLeftOpen;
        public UnityEvent OnLeftClosed;
        public UnityEvent OnRightOpen;
        public UnityEvent OnRightClosed;

        public bool isLeftOpen { get; private set; } = true;
        public bool isRightOpen { get; private set; } = true;

        ARFace m_Face => GetComponent<ARFace>();

        private EyePoseVisualizer eyePos => GetComponent<EyePoseVisualizer>();
        private List<int> leftEyeVertices = new List<int>() { 490, 491, 492, 493, 494, 495, 496, 497, 498, 500, 501, 503, 504, 505, 506, 583, 594, 595, 602, 603, 753, 754, 756, 757, 787, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 846, 847, 854, 861, 862, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071, 1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 1157, 1158, 1159, 1160, 1161, 1162, 1163, 1164, 1165, 1166, 1167, 1168, 1169, 1172, 1173, 1174, 1175, 1176, 1177, 1178, 1179, 1180 };
        private List<int> rightEyeVertices = new List<int>() { 40, 41, 42, 43, 44, 45, 47, 48, 49, 133, 134, 138, 144, 145, 146, 153, 154, 160, 317, 318, 319, 321, 322, 356, 358, 378, 379, 380, 381, 382, 383, 384, 385, 386, 416, 426, 433, 434, 435, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1102, 1103, 1104, 1105, 1106, 1107, 1108, 1137, 1138, 1139, 1140, 1141, 1142, 1143, 1144, 1145, 1146, 1186, 1192, 1193, 1194, 1195, 1196, 1197, 1198, 1199, 1200, 1201, 1202, 1203, 1204 };

        private bool areEyeVerticesInited = false;
        private bool didFaceUpdate = false;

        private const float thresSumDistanceSqr = 0.0155f;

        private void Awake()
        {
            OnLeftOpen.AddListener(() => { Shoot(EyeType.Left); });
            OnRightOpen.AddListener(() => { Shoot(EyeType.Right); });

        }
        void OnEnable()
        {
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null && faceManager.subsystem != null && faceManager.descriptor.supportsEyeTracking)
            {
                m_Face.updated += OnUpdated;
                StartCoroutine(InitEyesVerticesCoroutine());
            }
            else
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            didFaceUpdate = false;
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            didFaceUpdate = true;

            if (areEyeVerticesInited)
            {
                UpdateEyeStatus(EyeType.Left);
                UpdateEyeStatus(EyeType.Right);
            }
        }

        private void Shoot(EyeType eyeType)
		{
            GameObject eyeGameObject = (eyeType == EyeType.Left) ? eyePos.Left : eyePos.Right;
            eyeGameObject.GetComponent<Gun>().Shoot();
        }

        private void UpdateEyeStatus(EyeType eyeType)
        {
            List<int> indices = (eyeType == EyeType.Left) ? leftEyeVertices : rightEyeVertices;
            bool isOpen = (eyeType == EyeType.Left) ? isLeftOpen : isRightOpen;
            UnityEvent onOpen = (eyeType == EyeType.Left) ? OnLeftOpen : OnRightOpen;
            UnityEvent onClosed = (eyeType == EyeType.Left) ? OnLeftClosed : OnRightClosed;

            float sumDistanceSqr = 0;
            Vector3 center = Vector3.zero;
            for (int i = 0; i < indices.Count; ++i)
			{
                center += m_Face.vertices[indices[i]];
            }
            center /= indices.Count;

            for (int i = 0; i < indices.Count; ++i)
            {
                float distanceSqr = (m_Face.vertices[indices[i]] - center).sqrMagnitude;
                sumDistanceSqr += distanceSqr;
            }

            /*
            if (eyeType == EyeType.Left)
			{
                GameObject.Find("Eye Tracking Info (1)").GetComponent<Text>().text = "Left: " + sumDistanceSqr;
            }
            else
			{
                GameObject.Find("Eye Tracking Info (2)").GetComponent<Text>().text = "Right: " + sumDistanceSqr;
            }
            */

            if (sumDistanceSqr < thresSumDistanceSqr)
            {
                if (isOpen)
                {
                    onClosed?.Invoke();
                }
                if (eyeType == EyeType.Left)
                {
                    isLeftOpen = false;
                }
                else
                {
                    isRightOpen = false;
                }
            }
            else
            {
                if (!isOpen)
                {
                    onOpen?.Invoke();
                }
                if (eyeType == EyeType.Left)
                {
                    isLeftOpen = true;
                }
                else
                {
                    isRightOpen = true;
                }
            }
        }

        private IEnumerator InitEyesVerticesCoroutine()
        {
            yield return new WaitUntil(() => didFaceUpdate);
            yield return new WaitForSeconds(1);
            areEyeVerticesInited = true;
        }

    }
}