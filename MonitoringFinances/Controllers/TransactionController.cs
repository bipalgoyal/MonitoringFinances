﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitoringFinances.Data;
using MonitoringFinances.Models;
using MonitoringFinances.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitoringFinances.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TransactionController
            (ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            //Get current user
            ApplicationUser currentUser = (ApplicationUser) await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            IEnumerable<Transaction> transactionsForCurUser = _db.Transaction.Include(u => u.Category).Where(u => u.Category.ApplicationUser.Id == currentUser.Id);
            foreach (Transaction transaction in transactionsForCurUser)
            {
                transaction.Category.CategoryType = _db.CategoryType.Where(u => u.Id == transaction.Category.CategoryTypeId).FirstOrDefault();
                transaction.Category.ApplicationUser = _db.ApplicationUser.Where(u => u.Id == currentUser.Id).FirstOrDefault();
            };
            return View(transactionsForCurUser);
        }
    }
}
