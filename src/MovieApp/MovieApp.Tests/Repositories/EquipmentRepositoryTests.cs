using MovieApp.Logic.Data;
using MovieApp.Logic.Models;
using MovieApp.Logic.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class EquipmentRepositoryTests
    {
        [Fact]
        public void FetchAvailableEquipment_NoItems_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EquipmentRepository repository = new EquipmentRepository(context);

            List<Equipment> result = repository.FetchAvailableEquipment();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void FetchAvailableEquipment_OnlyReturnsAvailable()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            context.Equipment.AddRange(
                BuildEquipment(seller, "Camera", EquipmentStatus.Available),
                BuildEquipment(seller, "Mic", EquipmentStatus.Sold),
                BuildEquipment(seller, "Tripod", EquipmentStatus.Available));
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            List<Equipment> result = repository.FetchAvailableEquipment();

            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.Equal(EquipmentStatus.Available, item.Status));
        }

        [Fact]
        public void ListItem_AddsItemAsAvailable()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            Equipment item = BuildEquipment(seller, "Lens", EquipmentStatus.Sold);
            repository.ListItem(item);

            Equipment stored = context.Equipment.Single();
            Assert.Equal("Lens", stored.Title);
            Assert.Equal(EquipmentStatus.Available, stored.Status);
        }

        [Fact]
        public void PurchaseEquipment_HappyPath_MarksSoldAndLogsTransaction()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            User buyer = new User
            {
                Username = "buyer",
                Email = "buyer@example.com",
                PasswordHash = "hash",
                Balance = 500m
            };
            context.Users.AddRange(seller, buyer);
            Equipment item = BuildEquipment(seller, "Camera", EquipmentStatus.Available);
            item.Price = 200m;
            context.Equipment.Add(item);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            repository.PurchaseEquipment(item.Id, buyer.Id, 200m, "1 Test Street");

            Assert.Equal(EquipmentStatus.Sold, context.Equipment.Single().Status);
            Assert.Equal(300m, context.Users.Single(user => user.Id == buyer.Id).Balance);
            Transaction transaction = Assert.Single(context.Transactions);
            Assert.Equal(-200m, transaction.Amount);
            Assert.Equal("1 Test Street", transaction.ShippingAddress);
        }

        [Fact]
        public void PurchaseEquipment_InvalidBuyerId_Throws()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EquipmentRepository repository = new EquipmentRepository(context);

            Assert.Throws<InvalidOperationException>(
                () => repository.PurchaseEquipment(1, 0, 10m, "address"));
        }

        private static User BuildSeller()
        {
            return new User
            {
                Username = "seller",
                Email = "seller@example.com",
                PasswordHash = "hash",
                Balance = 0m
            };
        }

        private static Equipment BuildEquipment(User seller, string title, EquipmentStatus status)
        {
            return new Equipment
            {
                Title = title,
                Category = "Cameras",
                Description = "desc",
                Condition = "New",
                Price = 100m,
                ImageUrl = "https://example.com/image.jpg",
                Status = status,
                Seller = seller
            };
        }
    }
}
