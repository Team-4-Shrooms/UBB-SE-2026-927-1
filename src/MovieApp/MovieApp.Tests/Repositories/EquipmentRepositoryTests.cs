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

            Assert.Empty(result);
        }

        [Fact]
        public void FetchAvailableEquipment_OnlyReturnsAvailable_ReturnsCorrectCount()
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
        }

        [Fact]
        public void FetchAvailableEquipment_OnlyReturnsAvailable_AllStatusesAreAvailable()
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

            bool allAvailable = result.All(item => item.Status == EquipmentStatus.Available);

            Assert.True(allAvailable);
        }

        [Fact]
        public void ListItem_StoresItemWithMatchingTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            Equipment item = BuildEquipment(seller, "Lens", EquipmentStatus.Sold);
            repository.ListItem(item);

            Assert.Equal("Lens", context.Equipment.Single().Title);
        }

        [Fact]
        public void ListItem_ForcesStatusToAvailable()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            Equipment item = BuildEquipment(seller, "Lens", EquipmentStatus.Sold);
            repository.ListItem(item);

            Assert.Equal(EquipmentStatus.Available, context.Equipment.Single().Status);
        }

        [Fact]
        public void PurchaseEquipment_HappyPath_MarksItemAsSold()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            User buyer = BuildBuyer(500m);
            context.Users.AddRange(seller, buyer);
            Equipment item = BuildEquipment(seller, "Camera", EquipmentStatus.Available);
            item.Price = 200m;
            context.Equipment.Add(item);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            repository.PurchaseEquipment(item.Id, buyer.Id, 200m, "1 Test Street");

            Assert.Equal(EquipmentStatus.Sold, context.Equipment.Single().Status);
        }

        [Fact]
        public void PurchaseEquipment_HappyPath_DeductsBuyerBalance()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            User buyer = BuildBuyer(500m);
            context.Users.AddRange(seller, buyer);
            Equipment item = BuildEquipment(seller, "Camera", EquipmentStatus.Available);
            item.Price = 200m;
            context.Equipment.Add(item);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            repository.PurchaseEquipment(item.Id, buyer.Id, 200m, "1 Test Street");

            Assert.Equal(300m, context.Users.Single(user => user.Id == buyer.Id).Balance);
        }

        [Fact]
        public void PurchaseEquipment_HappyPath_LogsNegativeTransactionAmount()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            User buyer = BuildBuyer(500m);
            context.Users.AddRange(seller, buyer);
            Equipment item = BuildEquipment(seller, "Camera", EquipmentStatus.Available);
            item.Price = 200m;
            context.Equipment.Add(item);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            repository.PurchaseEquipment(item.Id, buyer.Id, 200m, "1 Test Street");

            Assert.Equal(-200m, context.Transactions.Single().Amount);
        }

        [Fact]
        public void PurchaseEquipment_HappyPath_StoresShippingAddress()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            User buyer = BuildBuyer(500m);
            context.Users.AddRange(seller, buyer);
            Equipment item = BuildEquipment(seller, "Camera", EquipmentStatus.Available);
            item.Price = 200m;
            context.Equipment.Add(item);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            repository.PurchaseEquipment(item.Id, buyer.Id, 200m, "1 Test Street");

            Assert.Equal("1 Test Street", context.Transactions.Single().ShippingAddress);
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

        private static User BuildBuyer(decimal balance)
        {
            return new User
            {
                Username = "buyer",
                Email = "buyer@example.com",
                PasswordHash = "hash",
                Balance = balance
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
