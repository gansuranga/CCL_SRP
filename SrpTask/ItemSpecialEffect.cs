
namespace SrpTask
{
    public class ItemSpecialEffect:IItemSpecialEffect
    {
        private readonly IGameEngine _gameEngine;
        public ItemSpecialEffect(IGameEngine gameEngine) {
            _gameEngine = gameEngine;
            
        }
        
        public void PlaySpecialEffect(Item item) {

            if (item != null && item.Rare) {
                _gameEngine.PlaySpecialEffect("cool_swirly_particles");
          
            }

            if (item != null && item.Rare && item.Unique) {
                _gameEngine.PlaySpecialEffect("blue_swirly");
                return;
            }

            if (item != null && item.Heal > 500) {
                _gameEngine.PlaySpecialEffect("green_swirly");
                return;
            }
           
        }

    }
}
