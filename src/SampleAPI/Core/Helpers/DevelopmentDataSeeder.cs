using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SampleAPI.Core.Helpers
{
    using Extensions;
    using Models;
    using System.Collections.Generic;

    public class DevelopmentDataSeeder
    {
        private readonly IDataContext dataContext;

        public DevelopmentDataSeeder(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <summary>
        /// Seed default data.
        /// </summary>
        /// <returns></returns>
        public void Seed()
        {
            if (!dataContext.Database.Exists())
                return;

            CreateUsers();
        }

        /// <summary>
        /// Creates default users.
        /// </summary>
        private void CreateUsers()
        {
            if (dataContext.Users.Any())
                return;   // DB has been seeded

            var companies = new Company[]
            {
                new Company{ Name = "Apple" },
                new Company{ Name = "Google" }
            };

            var users = new User[]
            {
                new User{ FullName = "John Smith", Email = "john@example.com", CreateDate = new DateTime(2021, 01, 05) },
                new User{ FullName = "Michael Smith", Email = "michael@example.com", CreateDate = new DateTime(2021, 04, 23) },
                new User{ FullName = "Mary Smith", Email = "mary@example.com", CreateDate = new DateTime(2020, 09, 13) },
                new User{ FullName = "Torrie Gossman", Email = "torrie@example.com", CreateDate = new DateTime(2021, 1, 10) },
                new User{ FullName = "Ellsworth Graden", Email = "ellsworth@example.com", CreateDate = new DateTime(2020, 5, 1) },
                new User{ FullName = "Sherri Ospina", Email = "sherri@example.com", CreateDate = new DateTime(2021, 2, 3) },
                new User{ FullName = "Parker Humble", Email = "parker@example.com", CreateDate = new DateTime(2021, 10, 12) },
                new User{ FullName = "Erica Bonilla", Email = "erica@example.com", CreateDate = new DateTime(2020, 5, 15) },
                new User{ FullName = "Casey Lee", Email = "casey@example.com", CreateDate = new DateTime(2021, 4, 6) },
                new User{ FullName = "Rosalind Allen", Email = "rosalind@example.com", CreateDate = new DateTime(2020, 7, 7) },
                new User{ FullName = "Zachary Marsh", Email = "zachary@example.com", CreateDate = new DateTime(2021, 8, 18) },
                new User{ FullName = "Lea Hayes", Email = "lea@example.com", CreateDate = new DateTime(2021, 12, 9) },
                new User{ FullName = "Fletcher Neal", Email = "fletcher@example.com", CreateDate = new DateTime(2020, 5, 11) },
                new User{ FullName = "Sarah Nolan", Email = "sarah@example.com", CreateDate = new DateTime(2021, 4, 22) },
                new User{ FullName = "Michelle Schroeder", Email = "michelle@example.com", CreateDate = new DateTime(2020, 10, 30) },
                new User{ FullName = "Elwood Robinson", Email = "elwood@example.com", CreateDate = new DateTime(2021, 1, 11) },
                new User{ FullName = "Cynthia Savage", Email = "cynthia@example.com", CreateDate = new DateTime(2020, 5, 1) },
                new User{ FullName = "Kirby Lester", Email = "kirby@example.com", CreateDate = new DateTime(2021, 4, 4) },
                new User{ FullName = "Neville Valdez", Email = "neville@example.com", CreateDate = new DateTime(2020, 7, 15) },
                new User{ FullName = "Estela Andrade", Email = "estela@example.com", CreateDate = new DateTime(2021, 5, 10) },
                new User{ FullName = "Thelma Valencia", Email = "thelma@example.com", CreateDate = new DateTime(2021, 7, 1) }
            };

            users[1].Company = companies[0];
            users[1].Posts.Add(new Post() { Title = "Mountain Out of a Molehill", Content = "Some content" });
            users[1].Posts.Add(new Post() { Title = "Jaws of Death", Content = "Some content" });
            users[1].Posts.Add(new Post() { Title = "A Day Late and a Dollar Short", Content = "Some content" });
            users[1].Posts.Add(new Post() { Title = "Eat My Hat", Content = "Some content" });
            users[1].Posts.Add(new Post() { Title = "A Lot on One’s Plate", Content = "Some content" });

            users[2].Company = companies[1];
            users[2].Posts.Add(new Post() { Title = "Elephant in the Room", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Drive Me Nuts", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Break The Ice", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Keep Your Shirt On", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Ride Him, Cowboy!", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "On the Ropes", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Rain on Your Parade", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "A Chip on Your Shoulder", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Goody Two-Shoes", Content = "Some content" });
            users[2].Posts.Add(new Post() { Title = "Knock Your Socks Off", Content = "Some content" });

            foreach (var u in users)
            {
                dataContext.Users.Add(u);
            }

            dataContext.SaveChanges();
        }
    }
}