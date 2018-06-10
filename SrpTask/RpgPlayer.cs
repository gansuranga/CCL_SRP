using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpTask
{
    public class RpgPlayer
    {
        private readonly IPlayerInventory _playerInventory;
        private readonly IGameEngine _gameEngine;
        private readonly IItemSpecialEffect _specialEffect;

        public const int MaximumCarryingCapacity = 1000;
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Armour { get;  set; }

        /// <summary>
        /// How much the player can carry in kilograms
        /// </summary>
        public int CarryingCapacity { get;  set; }

        public RpgPlayer(IGameEngine gameEngine, IPlayerInventory playerInventory,IItemSpecialEffect itemSpecialEffect)
        {
            _gameEngine = gameEngine;
            _playerInventory = playerInventory;
            CarryingCapacity = MaximumCarryingCapacity;
            _specialEffect = itemSpecialEffect;
        }
   
        public void CalculateStats()
        {
            Armour = _playerInventory.CalculateStats();
        }

        public void UseItem(Item item)
        {
            if (item.Name == "Stink Bomb")
            {
                var enemies = _gameEngine.GetEnemiesNear(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(100);
                }
            }
        }

        public bool PickUpItem(Item item)
        {
            var weight = _playerInventory.CalculateInventoryWeight();
            if (weight + item.Weight > CarryingCapacity || item.Unique && _playerInventory.CheckIfItemExistsInInventory(item))
                return false;

            _specialEffect.PlaySpecialEffect(item);
       
            if (item.Heal <= 0)
            {
                _playerInventory.AddItem(item);
            }
            return true;
        }

        public void CalculateHealth(Item item)
        {
            // Don't pick up items that give health, just consume them.
            if (item.Heal > 0)
            {
                Health += item.Heal;

                if (Health > MaxHealth)
                    Health = MaxHealth;
                _specialEffect.PlaySpecialEffect(item);
     
            }
        }


        public void TakeDamage(int damage)
        {
            if (damage < Armour)
            {
                _gameEngine.PlaySpecialEffect("parry");
                return;
            }
            var damageToDeal = damage - Armour;
            if (MaximumCarryingCapacity/2 > CarryingCapacity)
                Health -= (damageToDeal-(Convert.ToInt32(damageToDeal*0.25)));
            else
                Health -= damageToDeal;
            _gameEngine.PlaySpecialEffect("lots_of_gore");
            
        }
    }
}