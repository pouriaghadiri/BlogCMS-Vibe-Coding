using BlogCMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogCMS.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting database seeding...");

            // Seed Roles
            if (!await roleManager.Roles.AnyAsync())
            {
                logger.LogInformation("Seeding roles...");
                var roles = new[]
                {
                    new IdentityRole("Admin"),
                    new IdentityRole("Editor"),
                    new IdentityRole("Author"),
                    new IdentityRole("User")
                };

                foreach (var role in roles)
                {
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Created role: {role.Name}");
                    }
                    else
                    {
                        logger.LogError($"Failed to create role {role.Name}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            else
            {
                logger.LogInformation("Roles already exist, skipping role seeding.");
            }

            // Seed Admin User
            if (!await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Seeding users...");
                var adminUser = new User
                {
                    UserName = "admin@blogcms.com",
                    Email = "admin@blogcms.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    logger.LogInformation("Created admin user");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Added Admin role to admin user");
                }
                else
                {
                    logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // Seed Editor User
                var editorUser = new User
                {
                    UserName = "editor@blogcms.com",
                    Email = "editor@blogcms.com",
                    EmailConfirmed = true,
                    FirstName = "Editor",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                result = await userManager.CreateAsync(editorUser, "Editor123!");
                if (result.Succeeded)
                {
                    logger.LogInformation("Created editor user");
                    await userManager.AddToRoleAsync(editorUser, "Editor");
                    logger.LogInformation("Added Editor role to editor user");
                }
                else
                {
                    logger.LogError($"Failed to create editor user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // Seed Author User
                var authorUser = new User
                {
                    UserName = "author@blogcms.com",
                    Email = "author@blogcms.com",
                    EmailConfirmed = true,
                    FirstName = "Author",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                result = await userManager.CreateAsync(authorUser, "Author123!");
                if (result.Succeeded)
                {
                    logger.LogInformation("Created author user");
                    await userManager.AddToRoleAsync(authorUser, "Author");
                    logger.LogInformation("Added Author role to author user");
                }
                else
                {
                    logger.LogError($"Failed to create author user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation("Users already exist, skipping user seeding.");
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                logger.LogInformation("Seeding categories...");
                var categories = new[]
                {
                    new Category
                    {
                        Name = "Technology",
                        Slug = "technology",
                        Description = "Technology related articles and news"
                    },
                    new Category
                    {
                        Name = "Programming",
                        Slug = "programming",
                        Description = "Programming tutorials and guides"
                    },
                    new Category
                    {
                        Name = "Web Development",
                        Slug = "web-development",
                        Description = "Web development articles and tutorials"
                    },
                    new Category
                    {
                        Name = "Mobile Development",
                        Slug = "mobile-development",
                        Description = "Mobile app development articles and tutorials"
                    }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
                logger.LogInformation("Categories seeded successfully");
            }
            else
            {
                logger.LogInformation("Categories already exist, skipping category seeding.");
            }

            // Seed Tags
            if (!await context.Tags.AnyAsync())
            {
                logger.LogInformation("Seeding tags...");
                var tags = new[]
                {
                    new Tag { Name = "C#", Slug = "csharp", Description = "C# programming language" },
                    new Tag { Name = "ASP.NET Core", Slug = "aspnet-core", Description = "ASP.NET Core framework" },
                    new Tag { Name = "JavaScript", Slug = "javascript", Description = "JavaScript programming language" },
                    new Tag { Name = "React", Slug = "react", Description = "React JavaScript library" },
                    new Tag { Name = "TypeScript", Slug = "typescript", Description = "TypeScript programming language" },
                    new Tag { Name = "SQL", Slug = "sql", Description = "SQL database language" },
                    new Tag { Name = "Entity Framework", Slug = "entity-framework", Description = "Entity Framework ORM" }
                };

                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
                logger.LogInformation("Tags seeded successfully");
            }
            else
            {
                logger.LogInformation("Tags already exist, skipping tag seeding.");
            }

            // Seed Posts
            if (!await context.Posts.AnyAsync())
            {
                logger.LogInformation("Seeding posts...");
                var adminUser = await userManager.FindByEmailAsync("admin@blogcms.com");
                var technologyCategory = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "technology");
                var programmingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "programming");
                var csharpTag = await context.Tags.FirstOrDefaultAsync(t => t.Slug == "csharp");
                var aspnetTag = await context.Tags.FirstOrDefaultAsync(t => t.Slug == "aspnet-core");

                if (adminUser != null && technologyCategory != null && programmingCategory != null && csharpTag != null && aspnetTag != null)
                {
                    var posts = new[]
                    {
                        new Post
                        {
                            Title = "Getting Started with ASP.NET Core",
                            Slug = "getting-started-with-aspnet-core",
                            Content = "ASP.NET Core is a cross-platform, high-performance framework for building modern, cloud-based, internet-connected applications. In this article, we'll explore the basics of ASP.NET Core and how to get started with your first application.",
                            Summary = "Learn the basics of ASP.NET Core and how to create your first application.",
                            Status = PostStatus.Published,
                            PublishedAt = DateTime.UtcNow.AddDays(-5),
                            AuthorId = adminUser.Id,
                            CategoryId = programmingCategory.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-6),
                            PostTags = new List<PostTag>
                            {
                                new PostTag { Tag = csharpTag },
                                new PostTag { Tag = aspnetTag }
                            }
                        },
                        new Post
                        {
                            Title = "Understanding C# 9.0 Features",
                            Slug = "understanding-csharp-9-features",
                            Content = "C# 9.0 introduces several new features that make the language more powerful and easier to use. In this article, we'll explore these features and how they can improve your code.",
                            Summary = "Explore the new features introduced in C# 9.0 and how they can enhance your development experience.",
                            Status = PostStatus.Published,
                            PublishedAt = DateTime.UtcNow.AddDays(-3),
                            AuthorId = adminUser.Id,
                            CategoryId = technologyCategory.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-4),
                            PostTags = new List<PostTag>
                            {
                                new PostTag { Tag = csharpTag }
                            }
                        }
                    };

                    await context.Posts.AddRangeAsync(posts);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Posts seeded successfully");
                }
                else
                {
                    logger.LogError("Failed to seed posts: Required entities not found");
                    if (adminUser == null) logger.LogError("Admin user not found");
                    if (technologyCategory == null) logger.LogError("Technology category not found");
                    if (programmingCategory == null) logger.LogError("Programming category not found");
                    if (csharpTag == null) logger.LogError("C# tag not found");
                    if (aspnetTag == null) logger.LogError("ASP.NET Core tag not found");
                }
            }
            else
            {
                logger.LogInformation("Posts already exist, skipping post seeding.");
            }

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
} 