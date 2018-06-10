using System;
using System.Collections.Generic;
using System.Text;

namespace SrpTask
{
    public interface IPlayerInventory
    {
        int CalculateStats();

        bool CheckIfItemExistsInInventory(Item item);

        void AddItem(Item item);

        List<Item> GetItems();

        int CalculateInventoryWeight();

    }
}
