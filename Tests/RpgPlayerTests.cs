using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using SrpTask;
using Xunit;

namespace Tests
{
    public class RpgPlayerTests
    {
        private readonly IGameEngine _engine;
        private readonly IPlayerInventory _playerInventory;
        private readonly ItemSpecialEffect _specialEffect;

        public int MaximumCarryingCapacity = 1000;

        public RpgPlayerTests() {
            _engine = Substitute.For<IGameEngine>();
            _playerInventory = new PlayerInventory(_engine,MaximumCarryingCapacity);
            _specialEffect = new ItemSpecialEffect(_engine);
        }

        [Fact]
        public void PickUpItem_ThatCanBePickedUp_ItIsAddedToTheInventory()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            Item item = ItemBuilder.Build;
          
            //Act
            var items =_playerInventory.GetItems();
            // Assert
            Assert.Empty(items);
            // Act
            player.PickUpItem(item);
            // Assert
            Assert.True(_playerInventory.CheckIfItemExistsInInventory(item));
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthIsIncreaseAndItemIsNotAddedToInventory()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect)
            {
                MaxHealth = 100,
                Health = 10
            };

            Item healthPotion = ItemBuilder.Build.WithHeal(100);

            // Act
            player.PickUpItem(healthPotion);
            player.CalculateHealth(healthPotion);
            //Act
            var items = _playerInventory.GetItems();
            // Assert
            Assert.Empty(items);
            // Assert
            player.Health.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthDoesNotExceedMaxHealth()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect)
            {
                MaxHealth = 50,
                Health = 10
            };

            Item healthPotion = ItemBuilder.Build.WithHeal(100);
            // Act
            player.PickUpItem(healthPotion);
            player.CalculateHealth(healthPotion);
            //Act
            var items = _playerInventory.GetItems();
            // Assert
            Assert.Empty(items);
            player.Health.Should().Be(50);
        }

        [Fact]
        public void PickUpItem_ThatIsRare_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            Item rareItem = ItemBuilder.Build.IsRare(true);
            // Act
            player.PickUpItem(rareItem);
            // Assert
            _engine.Received().PlaySpecialEffect("cool_swirly_particles");
        }

        [Fact]
        public void PickUpItem_ThatIsUnique_ItShouldNotBePickedUpIfThePlayerAlreadyHasIt()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            player.PickUpItem(ItemBuilder.Build.WithId(100));
            Item uniqueItem = ItemBuilder.Build.WithId(100).IsUnique(true);
            // Act
            var result = player.PickUpItem(uniqueItem);
            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void PickUpItem_ThatDoesMoreThan500Healing_AnExtraGreenSwirlyEffectOccurs()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            Item xPotion = ItemBuilder.Build.WithHeal(501);

            // Act
            player.PickUpItem(xPotion);
            player.CalculateHealth(xPotion);
            // Assert
            _engine.Received().PlaySpecialEffect("green_swirly");
        }

        [Fact]
        public void PickUpItem_ThatGivesArmour_ThePlayersArmourValueShouldBeIncreased()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            player.Armour.Should().Be(0);
            Item armour = ItemBuilder.Build.WithArmour(100);
       
            // Act
            player.PickUpItem(armour);
            player.CalculateStats();
            // Assert
            player.Armour.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatIsTooHeavy_TheItemIsNotPickedUp()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            var inventory = new PlayerInventory(_engine, MaximumCarryingCapacity);
            Item heavyItem = ItemBuilder.Build.WithWeight(inventory.CarryingCapacity + 1);
            // Act
            var result = player.PickUpItem(heavyItem);
            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TakeDamage_WithNoArmour_HealthIsReducedAndParticleEffectIsShown()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect)
            {
                Health = 200
            };
            // Act
            player.TakeDamage(100);
            // Assert
            player.Health.Should().Be(100);
            _specialEffect.PlaySpecialEffect(null);
            _engine.Received().PlaySpecialEffect("lots_of_gore");
         
        }

        [Fact]
        public void TakeDamage_With50Armour_DamageIsReducedBy50AndParticleEffectIsShown()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect)
            { Health = 200};
            player.PickUpItem(ItemBuilder.Build.WithArmour(50));
            player.CalculateStats();
            // Act
            player.TakeDamage(100);
            // Assert
            player.Health.Should().Be(150);
            _specialEffect.PlaySpecialEffect(null);
            _engine.Received().PlaySpecialEffect("lots_of_gore");
        }

        [Fact]
        public void TakeDamage_WithMoreArmourThanDamage_NoDamageIsDealtAndParryEffectIsPlayed()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect)
            { Health = 200};
            player.PickUpItem(ItemBuilder.Build.WithArmour(2000));
            player.CalculateStats();
            // Act
            player.TakeDamage(100);
            // Assert
            player.Health.Should().Be(200);
            _engine.Received().PlaySpecialEffect("parry");
        }

        [Fact]
        public void UseItem_StinkBomb_AllEnemiesNearThePlayerAreDamaged()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            var enemy = Substitute.For<IEnemy>();
            Item item = ItemBuilder.Build.WithName("Stink Bomb");
            _engine.GetEnemiesNear(player).Returns(new List<IEnemy> {enemy});
            // Act
            player.UseItem(item);
            // Assert
            enemy.Received().TakeDamage(100);
        }

        [Fact]
        public void PickUpItem_ThatIsRareAndUnique_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect);
            Item iSRareAndUnique = ItemBuilder.Build.IsRare(true).IsUnique(true);
            
            // Act
            player.PickUpItem(iSRareAndUnique);
            // Assert
            _engine.Received().PlaySpecialEffect("blue_swirly");
        }
        [Fact]
        public void TakeDamage_IsReducedBy25Percent_IfCarrying50PercentOfTheMaximumCapacity()
        {
            // Arrange
            var player = new RpgPlayer(_engine, _playerInventory, _specialEffect)
            {
                Health = 100,
                CarryingCapacity = 490
            };
         
            // Act
            player.TakeDamage(100);
            // Assert
            player.Health.Should().Be(25);
         
        }

    }
}