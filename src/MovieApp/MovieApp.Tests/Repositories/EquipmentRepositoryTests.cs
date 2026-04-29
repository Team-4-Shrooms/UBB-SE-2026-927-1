using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;

namespace MovieApp.Tests.Repositories
{
    public sealed class EquipmentRepositoryTests
    {
        [Fact]
        public void FetchAvailableEquipment_NoItemsExist_ReturnsEmptyList()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EquipmentRepository repository = new EquipmentRepository(context);

            List<Equipment> availableEquipment = repository.FetchAvailableEquipment();

            Assert.Empty(availableEquipment);
        }

        [Fact]
        public void FetchAvailableEquipment_TwoAvailableOneUnavailable_ReturnsTwoItems()
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
            List<Equipment> availableEquipment = repository.FetchAvailableEquipment();

            Assert.Equal(2, availableEquipment.Count);
        }

        [Fact]
        public void FetchAvailableEquipment_MixedStatuses_AllReturnedItemsAreAvailable()
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
            List<Equipment> availableEquipment = repository.FetchAvailableEquipment();

            bool allAreAvailable = availableEquipment.All(item => item.Status == EquipmentStatus.Available);

            Assert.True(allAreAvailable);
        }

        [Fact]
        public async Task GetByIdAsync_EquipmentExists_ReturnsEquipmentWithMatchingTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            Equipment equipment = BuildEquipment(seller, "Camera", EquipmentStatus.Available);
            context.Equipment.Add(equipment);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            Equipment? foundEquipment = await repository.GetByIdAsync(equipment.Id);

            Assert.Equal("Camera", foundEquipment!.Title);
        }

        [Fact]
        public async Task GetByIdAsync_EquipmentDoesNotExist_ReturnsNull()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            EquipmentRepository repository = new EquipmentRepository(context);

            Equipment? foundEquipment = await repository.GetByIdAsync(999);

            Assert.Null(foundEquipment);
        }

        [Fact]
        public async Task AddAsync_ValidEquipment_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            Equipment equipment = BuildEquipment(seller, "Lens", EquipmentStatus.Available);
            await repository.AddAsync(equipment);
            await repository.SaveChangesAsync();

            Assert.Single(context.Equipment);
        }

        [Fact]
        public async Task AddAsync_ValidEquipment_StoresCorrectTitle()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User seller = BuildSeller();
            context.Users.Add(seller);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            Equipment equipment = BuildEquipment(seller, "Lens", EquipmentStatus.Available);
            await repository.AddAsync(equipment);
            await repository.SaveChangesAsync();

            Assert.Equal("Lens", context.Equipment.Single().Title);
        }

        [Fact]
        public async Task AddTransactionAsync_ValidTransaction_CreatesOneRecord()
        {
            using AppDbContext context = TestDbContextFactory.Create();
            User user = BuildSeller();
            context.Users.Add(user);
            context.SaveChanges();

            EquipmentRepository repository = new EquipmentRepository(context);
            await repository.AddTransactionAsync(new Transaction
            {
                Buyer = user,
                Amount = -100m,
                Type = "EquipmentPurchase",
                Status = "Completed",
                Timestamp = DateTime.UtcNow
            });
            await repository.SaveChangesAsync();

            Assert.Single(context.Transactions);
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
