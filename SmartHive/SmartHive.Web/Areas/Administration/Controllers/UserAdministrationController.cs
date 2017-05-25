﻿using SmartHive.Authentication.Providers;
using SmartHive.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PagedList;
using SmartHive.Web.Areas.Administration.Models;

namespace SmartHive.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserAdministrationController : Controller
    {
        private readonly IUserService userService;
        private readonly IAuthenticationProvider authProvider;

        public UserAdministrationController(
            IUserService userService,
            IAuthenticationProvider authProvider)
        {
            if(userService == null)
            {
                throw new ArgumentNullException(nameof(userService));
            }    

            if(authProvider == null)
            {
                throw new ArgumentNullException(nameof(authProvider));
            }

            this.userService = userService;
            this.authProvider = authProvider;
        }

        // GET: UserAdmin
        public ActionResult Index(int page = 1, int count = 15)
        {
            var users = this.userService.GetUsers();

            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var isAdmin = this.authProvider.IsInRole(user.Id, "Administrator");
                var viewModel = new UserViewModel(user, isAdmin);
                model.Add(viewModel);
            }

            return this.View(model.ToPagedList(page, count));
        }

        public ActionResult RemoveAdmin(string userId, int page)
        {
            this.authProvider.RemoveFromRole(userId, "Administrator");

            return this.RedirectToAction("Index", new { page = page });
        }

        public ActionResult AddAdmin(string userId, int page)
        {
            this.authProvider.AddToRole(userId, "Administrator");

            return this.RedirectToAction("Index", new { page = page });
        }
    }
}