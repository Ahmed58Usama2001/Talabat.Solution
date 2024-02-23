﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if(userManager.Users.Count()==0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Ahmed Usama",
                    Email = "ahmedusamasaad@gmail.com",
                    UserName = "Ahmed85Usama",
                    PhoneNumber = "01024430384",
                };

                await userManager.CreateAsync(user,"Osama_58200165");
            }
        }
    }
}
