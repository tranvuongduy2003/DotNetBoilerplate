using Bogus;
using Domain.Entities;
using Domain.Enums.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Extensions;

namespace Infrastructure.Persistence.Data;

public class ApplicationDbContextSeed
{
    private const int MAX_USERS_QUANTITY = 10;

    private readonly ApplicationDbContext _context;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public ApplicationDbContextSeed(ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Seed()
    {
        SeedRoles().Wait();
        SeedUsers().Wait();
    }

    private async Task SeedRoles()
    {
        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new Role(EUserRole.ADMIN.GetDisplayName()));
            await _roleManager.CreateAsync(new Role(EUserRole.CUSTOMER.GetDisplayName()));
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedUsers()
    {
        if (!_userManager.Users.Any())
        {
            var admin = new User
            {
                Email = "admin@gmail.com",
                UserName = "admin",
                PhoneNumber = "0829440357",
                FullName = "Admin",
                Dob = new Faker().Person.DateOfBirth,
                Gender = EGender.MALE,
                Bio = new Faker().Lorem.Paragraph(),
                Status = EUserStatus.ACTIVE
            };
            var result = await _userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync("admin@gmail.com");
                if (user != null)
                    await _userManager.AddToRoleAsync(user, EUserRole.ADMIN.GetDisplayName());
            }


            var userFaker = new Faker<User>()
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.UserName, f => f.Person.UserName)
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"))
                .RuleFor(u => u.FullName, f => f.Person.FullName)
                .RuleFor(u => u.Dob, f => f.Person.DateOfBirth)
                .RuleFor(u => u.Gender, f => f.PickRandom<EGender>())
                .RuleFor(u => u.Bio, f => f.Lorem.Paragraph())
                .RuleFor(u => u.Status, _ => EUserStatus.ACTIVE);

            for (var userIndex = 0; userIndex < MAX_USERS_QUANTITY * 2; userIndex++)
            {
                var customer = userFaker.Generate();
                var customerResult = await _userManager.CreateAsync(customer, "User@123");
                if (!customerResult.Succeeded || customer.Email == null) continue;
                var user = await _userManager.FindByEmailAsync(customer.Email);
                if (user != null)
                    await _userManager.AddToRolesAsync(user, new List<string>
                        { EUserRole.CUSTOMER.GetDisplayName() });
            }

            await _context.SaveChangesAsync();
        }
    }
}