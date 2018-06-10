using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrpTask
{
    public class PlayerInventory :IPlayerInventory
    {
        private List<Item> _inventory;
        private readonly IGameEngine _gameEngine;

        public int Armour { get; set; }
        public int CarryingCapacity { get; private set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
       
        
        public PlayerInventory(IGameEngine gameEngine, int MaximumCarryingCapacity) {

            _inventory = new List<Item>();
            _gameEngine = gameEngine;
             CarryingCapacity = MaximumCarryingCapacity;

        }
        public void AddItem(Item item) {

            _inventory.Add(item);
        }

        public bool CheckIfItemExistsInInventory(Item item)
        {
            return _inventory.Any(x => x.Id == item.Id);
        }

        public int CalculateStats()
        {
            var armour = _inventory.Sum(x => x.Armour);
            return armour;
            
        }

        public int CalculateInventoryWeight()
        {
            return _inventory.Sum(x => x.Weight);
        }

        public List<Item> GetItems()
        {
            return _inventory;
        }

    }
}
