using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }
        private const string PLAYER_PREFS_BINDINGS = "InputBindings";

        public event Action OnInteractAction;
        public event Action OnInteractAlternateAction;
        public event Action OnPauseAction;
        public event Action OnBindingRebind;

        public enum Binding
        {
            Move_Right,
            Move_Left,
            Jump_Up,
            Jump_Down,
            Interact,
            InteractAlternate,
            Pause,
            Gamepad_Interact,
            Gamepad_InteractAlternate,
            Gamepad_Pause
        }


        private PlayerInputAction playerInputActions;


        private void Awake()
        {
            playerInputActions = new PlayerInputAction();

            if(PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
            {
                playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
            }

            playerInputActions.Player.Enable();
            
        }

    }
}