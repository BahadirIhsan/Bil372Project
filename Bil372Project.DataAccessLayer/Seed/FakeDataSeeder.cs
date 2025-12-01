using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.DataAccessLayer.Seed
{
    public static class FakeDataSeeder
    {
        // DbContext ismini kendi projeninkine göre değiştir (ör: Bil372ProjectContext / AppDbContext)
        public static async Task SeedAsync(AppDbContext context, int userCount = 1000)
        {
            // Zaten data varsa tekrar seed etme (istersen bu satırı silebilirsin)

            var rnd = new Random();

            string[] firstNames = { "Ahmet", "Ayşe", "Mehmet", "Elif", "Bahadır", "Zeynep", "Ali", "Ece", "Mert", "Selin", "Murat", "İhsan", "Burak", "Emre" };
            string[] lastNames  = { "Yılmaz", "Demir", "Herdem", "Kaya", "Çelik", "Şahin", "Yıldız", "Acar", "Öztürk", "Aslan" };
            string[] cities     = { "Ankara", "İstanbul", "İzmir", "Bursa", "Antalya", "Eskişehir" };
            string[] countries  = { "Türkiye" };
            string[] activityLevels = { "Sedentary", "Lightly Active", "Moderately Active", "Very Active" };
            string[] genders    = { "Male", "Female" };

            string[] breakfastOptions =
            {
                "smoothie with protein powder",
                "greek yogurt with granola and fruit",
                "oatmeal with berries and nuts",
                "smoothie with protein powder",
                "fruit and yogurt parfait",
                "whole grain toast with avocado and egg",
                "vegetable omelette with whole grain bread",
                "chia pudding with almond milk and fruit",
                "cottage cheese with pineapple and walnuts",
                "whole grain pancakes with maple syrup and berries"
            };
            string[] lunchOptions =
            {
                "grilled chicken salad with mixed greens",
                "quinoa salad with chickpeas and vegetables",
                "turkey sandwich",
                "chicken and vegetable stir-fry",
                "lentil soup with whole grain bread",
                "tuna salad with whole grain crackers",
                "vegetable wrap with hummus",
                "baked salmon with steamed broccoli",
                "chickpea and vegetable curry with brown rice",
                "grilled vegetable and mozzarella panini"
            };
            string[] dinnerOptions =
            {
                "lentil and vegetable curry",
                "turkey chili with brown rice",
                "pasta with marinara sauce and veggies",
                "Beef and broccoli stir-fry",
                "baked cod with quinoa and asparagus",
                "vegetable lasagna",
                "chicken fajitas with bell peppers and onions",
                "stuffed bell peppers with ground turkey and rice",
                "shrimp and vegetable stir-fry with noodles",
                "eggplant parmesan with a side salad"
            };
            string[] snackOptions =
            {
                "apple with almond butter",
                "fruit and nut mix",
                "protein bar",
                "greek yogurt with fruit",
                "carrot sticks with hummus",
                "rice cakes with peanut butter",
                "cottage cheese with sliced peaches",
                "hard-boiled eggs",
                "smoothie with spinach and banana",
                "whole grain crackers with cheese"
            };

            var usersToAdd = new List<AppUser>();
            var userMeasuresToAdd = new List<UserMeasure>();
            var dietPlansToAdd = new List<UserDietPlan>();

            for (int i = 1; i <= userCount; i++)
            {
                // ---- AppUser ----
                var firstName = firstNames[rnd.Next(firstNames.Length)];
                var lastName  = lastNames[rnd.Next(lastNames.Length)];

                var user = new AppUser
                {
                    FullName  = $"{firstName} {lastName}",
                    Email     = $"user{i}@fitoll.fake", // UNIQUE garanti
                    Password  = "123456",               // şimdilik düz; ileride hash
                    IsAdmin   = false,
                    PhoneNumber = $"+9055{rnd.Next(10000000, 99999999)}",
                    BirthDate = RandomBirthDate(rnd),   // 18–60 yaş arası
                    City      = cities[rnd.Next(cities.Length)],
                    Country   = countries[rnd.Next(countries.Length)],
                    Bio       = "Bu kullanıcı fake seed verisidir.",
                    UpdatedAt = DateTime.UtcNow
                };

                // ---- UserMeasure (kendi entity'ine göre uyarlayacaksın) ----
                var gender = genders[rnd.Next(genders.Length)];
                int age = rnd.Next(18, 61);
                double height = rnd.Next(150, 195);     // cm
                double weight = rnd.Next(50, 110);      // kg

                var measure = new UserMeasure
                {
                    User = user,                        // FK otomatik dolacak
                    Age = age,
                    Gender = gender,
                    HeightCm = height,
                    WeightKg = weight,
                    ActivityLevel = activityLevels[rnd.Next(activityLevels.Length)],
                    // diğer zorunlu alanların varsa burada doldur:
                    // GoalType = "...",
                    UpdatedAt = RandomDateInRange(rnd) // *** tam istediğin formatta ***
                };

                // ---- UserDietPlan ----
                var diet = new UserDietPlan
                {
                    UserMeasure = measure, // FK için navigation kullan
                    Breakfast   = breakfastOptions[rnd.Next(breakfastOptions.Length)],
                    Lunch       = lunchOptions[rnd.Next(lunchOptions.Length)],
                    Dinner      = dinnerOptions[rnd.Next(dinnerOptions.Length)],
                    Snack       = snackOptions[rnd.Next(snackOptions.Length)],
                    GeneratedAt = DateTime.UtcNow.AddDays(-rnd.Next(0, 30)),
                    ModelVersion = "fake-seed-v1"
                };

                usersToAdd.Add(user);
                userMeasuresToAdd.Add(measure);
                dietPlansToAdd.Add(diet);
            }

            // Tek seferde add + save (performans için iyi)
            await context.Users.AddRangeAsync(usersToAdd);
            await context.UserMeasures.AddRangeAsync(userMeasuresToAdd);
            await context.UserDietPlans.AddRangeAsync(dietPlansToAdd);

            await context.SaveChangesAsync();
        }

        private static DateTime RandomBirthDate(Random rnd)
        {
            int age = rnd.Next(18, 61); // 18–60
            var today = DateTime.UtcNow.Date;
            return today.AddYears(-age).AddDays(-rnd.Next(0, 365));
        }
        
        private static DateTime RandomDateInRange(Random rnd)
        {
            var start = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc);
            var end   = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc);

            var range = (end - start).TotalSeconds;

            // Rastgele saniye ekle
            var randomSeconds = rnd.NextDouble() * range;

            // Mikro saniyeler de üretmek için ek komponent
            var randomTicks = rnd.Next(0, 10_000_000); // 1 tick = 100ns

            return start.AddSeconds(randomSeconds).AddTicks(randomTicks);
        }

    }
}
