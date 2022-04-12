using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pigger.Utils
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cam;
        [SerializeField] private int targetWidth;
        [SerializeField] private float pixelToUnits;

        private void Start()
        {
            int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
            cam.m_Lens.OrthographicSize = height / pixelToUnits / 2;
        }

    }
}

