﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TailwindBlazorElectron.Data;
using TailwindBlazorElectron.Model;

namespace TailwindBlazorElectron.Services
{
	public class LibraryService
	{
		private readonly ApplicationDbContext _dbContext;

		public LibraryService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
			SeedLibrarian();
		}

		private void SeedLibrarian()
		{
			var email = "gorajkora@gmail.com";
			var account = _dbContext.Accounts.FirstOrDefault(a => a.Email == email);
			if (account is null)
			{
				SignUp(email, "123", new User()
				{
					FirstName = "David",
					LastName = "Ivkovic",
					DateOfBirth = new DateTime(2000, 6, 17),
					Sex = Sex.Male,
					Status = Status.Adult,
					Role = Role.Librarian
				},
				new Address()
				{
					ZipCode = 21000,
					City = "Novi Sad",
					Street = "Strazilovska",
					HouseNumber = "30"
				});
			}
		}

		public User SignIn(string email, string password)
		{
			var user = _dbContext.Accounts
								 .Include(a => a.User)
								 	.ThenInclude(u => u.Address)
								 .Include(a => a.User)
									.ThenInclude(u => u.Notifications)
								.Include(a => a.User)
									.ThenInclude(u => u.Subscription)
								 .FirstOrDefault(a => a.Email == email && a.Password == password)?.User;
			return user;
		}

		public bool SignUp(string email, string password, User user, Address address, SubscriptionModel subscription = null)
		{
			var existingAccount = _dbContext.Accounts.SingleOrDefault(a => a.Email == email);

			if (existingAccount is not null)
			{
				return false;
			}

			Account account = new()
			{
				Email = email,
				Password = password,
				User = user,
				IsActive = true
			};

			user.Move(address);
			if (subscription is not null)
			{
				user.Subscribe(subscription);
			}
			user.Account = account;

			_dbContext.Add(account);
			return _dbContext.SaveChanges() != 0;
		}

		public User GetUser(Guid id)
		{
			return _dbContext.Users.Find(id);
		}

		public Subscription SubscribeReader(User user, SubscriptionModel model)
		{
			var subscription = user.Subscribe(model);
			_dbContext.SaveChanges();

			return subscription;
		}

		public Reservation RequestEditionReservation(User user, Edition edition)
		{
			var reservation = edition.Reserve();

			if (reservation is null)
			{
				return null;
			}

			user.AddReservation(reservation);

			_dbContext.Update(user);
			_dbContext.Add(reservation);
			_dbContext.SaveChanges();

			return reservation;
		}

		public void AllowReservation(Reservation reservation)
		{
			reservation.Allow();
			_dbContext.SaveChanges();
		}

		public Book GetBookByIdTitle(string IdTitle)
		{
			var book = _dbContext.Books.Include(b => b.Author)
									   .Include(b => b.Genres)
									   .Include(b => b.Editions)
										   .ThenInclude(e => e.Authors)
									   .FirstOrDefault(b => b.IdTitle == IdTitle);

			return book;
		}

		public List<SubscriptionModel> GetSubscriptionModels()
		{
			return new List<SubscriptionModel>
			{
				new() { DurationInMonths = 1, PriceInUsd = 0.99 },
				new() { DurationInMonths = 6, PriceInUsd = 3.99 },
				new() { DurationInMonths = 12, PriceInUsd = 5.99 },
				new() { DurationInMonths = 24, PriceInUsd = 9.99 },
			};
		}

		public List<Reservation> GetAllReservations()
		{
			return _dbContext.Reservations
							 .Include(r => r.User)
								 .ThenInclude(u => u.Account)
							 .Include(r => r.Edition)
								 .ThenInclude(e => e.Book)
									 .ThenInclude(b => b.Author)
							 .Include(r => r.Edition)
								 .ThenInclude(e => e.Authors)
							 .ToList();
		}
	}
}
