﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class CharacterStatsManager : Singleton<CharacterStatsManager>
    {
        [Header("Runtime References")]
        [SerializeField] private CharacterData _characterData;

        [Header("References")]
        [SerializeField] private List<CharacterStatsSlot> _characterStatSlots;


        private void Start()
        {
            StartCoroutine(ReferencePlayer());
        }

        private IEnumerator ReferencePlayer()
        {
            yield return new WaitUntil(() => GameDataManager.Instance.currentPlayer != null);          
            _characterData = GameDataManager.Instance.currentPlayer.characterData;


            UpdateStatUI(CharacterStats.MaxHealth);
            UpdateStatUI(CharacterStats.Recovery);
            UpdateStatUI(CharacterStats.Armor);
            UpdateStatUI(CharacterStats.MoveSpeed);
            UpdateStatUI(CharacterStats.JumpForce);
            UpdateStatUI(CharacterStats.Duration);
            UpdateStatUI(CharacterStats.Area);
            UpdateStatUI(CharacterStats.Cooldown);
            UpdateStatUI(CharacterStats.Magnet);
            UpdateStatUI(CharacterStats.Revival);
            UpdateStatUI(CharacterStats.Aware);
        }



        public void UpdateStatUI(CharacterStats characterStat)
        {
            if (_characterData == null)
                _characterData = GameDataManager.Instance.currentPlayer.characterData;

            for (int i = 0; i < _characterStatSlots.Count; i++)
            {
                if (_characterStatSlots[i]._characterStat == characterStat)
                {
                    switch (_characterStatSlots[i]._characterStat)
                    {
                        case CharacterStats.MaxHealth:
                            _characterStatSlots[i].SetValueUI(_characterData._currentMaxHealth, _characterData._currentMaxHealth - _characterData._defaultMaxHealth);
                            break;
                        case CharacterStats.Recovery:
                            _characterStatSlots[i].SetValueUI(_characterData._currentRecovery, _characterData._currentRecovery - _characterData._defaultRecovery);
                            break;
                        case CharacterStats.Armor:
                            _characterStatSlots[i].SetValueUI(_characterData._currentArmor, _characterData._currentArmor - _characterData._defaultArmor);
                            break;
                        case CharacterStats.MoveSpeed:
                            _characterStatSlots[i].SetValueUI(_characterData._currentMoveSpeed, _characterData._currentMoveSpeed - _characterData._defaultMoveSpeed);
                            break;
                        case CharacterStats.JumpForce:
                            _characterStatSlots[i].SetValueUI(_characterData._currentJumpForce, _characterData._currentJumpForce - _characterData._defaultJumpForce);
                            break;      
                        case CharacterStats.Duration:
                            _characterStatSlots[i].SetValueUI(_characterData._currentDuration, _characterData._currentDuration - _characterData._defaultDuration);
                            break;
                        case CharacterStats.Area:
                            _characterStatSlots[i].SetValueUI(_characterData._currentArea, _characterData._currentArea - _characterData._defaultArea);
                            break;
                        case CharacterStats.Cooldown:
                            _characterStatSlots[i].SetValueUI(_characterData._currentCooldown, _characterData._currentCooldown - _characterData._defaultCooldown);
                            break;
                        case CharacterStats.Magnet:
                            _characterStatSlots[i].SetValueUI(_characterData._currentMagnet, _characterData._currentMagnet - _characterData._defaultMagnet);
                            break;
                        case CharacterStats.Revival:
                            _characterStatSlots[i].SetValueUI(_characterData._currentRevival, _characterData._currentRevival - _characterData._defaultRevival);
                            break;
                        case CharacterStats.Aware:
                            _characterStatSlots[i].SetValueUI(_characterData._currentAware, _characterData._currentAware - _characterData._defaultAware);
                            break;
                        default:
                            break;
                    }

                    return;
                }
            }          
        }
    }
}
