using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

namespace Sataura
{
    public class GameManager : Singleton<GameManager> 
    {
        [SerializeField] private int limitFps = 144;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        public List<InventoryData> inGameInventories;

        public CinemachineVirtualCamera CinemachineVirtualCamera { get => cinemachineVirtualCamera; }


        private void Awake()
        {
            Application.targetFrameRate = limitFps;
        }

    
    }
}