#region

using SDG.Unturned;
using UnityEngine;

#endregion

namespace LegallyDistinctMissile.Commands
{
    public class LdmComponent : MonoBehaviour
    {
        private Player m_Player;
        private PlayerMovement m_PlayerMovement;
        public bool IsGodModeActive { get; private set; }
        public bool IsVanishModeActive { get; private set; }

        protected virtual void Awake()
        {
            m_Player = gameObject.GetComponent<Player>();
            m_PlayerMovement = gameObject.GetComponent<PlayerMovement>();
        }

        protected virtual void OnDestroy()
        {
            DesactivateGodMode();
            DesactivateVanishMode();
            m_PlayerMovement = null;
            m_Player = null;
        }

        public virtual void ActivateGodMode()
        {
            if (IsGodModeActive)
                return;

            m_Player.life.askHeal(100, true, true);
            m_Player.life.askDisinfect(100);
            m_Player.life.askDrink(100);
            m_Player.life.askEat(100);

            m_Player.life.onBleedingUpdated += OnBleeding;
            m_Player.life.onBrokenUpdated += OnBroken;
            m_Player.life.onFoodUpdated += OnFood;
            m_Player.life.onHealthUpdated += OnHealth;
            m_Player.life.onOxygenUpdated += OnOxygen;
            m_Player.life.onVirusUpdated += OnVirus;
            m_Player.life.onWaterUpdated += OnWater;

            IsGodModeActive = true;
        }

        public virtual void ActivateVanishMode()
        {
            if (IsVanishModeActive)
                return;

            m_PlayerMovement.canAddSimulationResultsToUpdates = false;
            IsVanishModeActive = true;
        }

        public virtual void DesactivateGodMode()
        {
            if (!IsGodModeActive)
                return;

            m_Player.life.onBleedingUpdated -= OnBleeding;
            m_Player.life.onBrokenUpdated -= OnBroken;
            m_Player.life.onFoodUpdated -= OnFood;
            m_Player.life.onHealthUpdated -= OnHealth;
            m_Player.life.onOxygenUpdated -= OnOxygen;
            m_Player.life.onVirusUpdated -= OnVirus;
            m_Player.life.onWaterUpdated -= OnWater;

            IsGodModeActive = false;
        }

        public virtual void DesactivateVanishMode()
        {
            if (!IsVanishModeActive)
                return;

            m_PlayerMovement.canAddSimulationResultsToUpdates = true;
            m_PlayerMovement.updates.Add(new PlayerStateUpdate(m_PlayerMovement.real, m_Player.look.angle,
                m_Player.look.rot));
            IsVanishModeActive = false;
        }

        #region UnturnedEvents

        private void OnBleeding(bool newBleeding)
        {
            if (!newBleeding)
                return;

            m_Player.life.serverSetBleeding(false);
        }

        private void OnBroken(bool newBroken)
        {
            if (!newBroken)
                return;

            m_Player.life.serverSetLegsBroken(false);
        }

        private void OnFood(byte newFood)
        {
            if (newFood > 90)
                return;

            m_Player.life.serverModifyFood(100);
        }

        private void OnHealth(byte newHealth)
        {
            if (newHealth > 90)
                return;

            m_Player.life.askHeal(100, false, false);
        }

        private void OnOxygen(byte newOxygen)
        {
            if (newOxygen > 10)
                return;

            m_Player.life.simulatedModifyOxygen(100);
        }

        private void OnVirus(byte newVirus)
        {
            if (newVirus > 10)
                return;

            m_Player.life.serverModifyVirus(100);
        }

        private void OnWater(byte newWater)
        {
            if (newWater > 90)
                return;

            m_Player.life.serverModifyWater(100);
        }

        #endregion
    }
}