﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TailwindBlazorElectron.Model
{
	public class User
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public Sex Sex { get; set; }
		public Role Role { get; set; }
		public Status Status { get; set; }
		public Account Account { get; set; }
		public Subscription Subscription { get; set; }
		public Address Address { get; private set; }
		public ICollection<Notification> Notifications { get; private set; }
		public ICollection<Reservation> Reservations { get; private set; }
		public string FullName => $"{FirstName} {LastName}";

		public Subscription Subscribe(SubscriptionModel subscription)
		{
			return Subscription = new()
			{
				StartDate = DateTime.Now,
				EndTime = DateTime.Now.AddMonths(subscription.DurationInMonths),
				Price = subscription.PriceInUsd,
				UserId = Id
			};
		}

		public void Move(Address adress)
		{
			Address = adress;
		}

		public void Notify(Notification notification)
		{
			Notifications.Add(notification);
		}

		public void AddReservation(Reservation reservation)
		{
			Reservations.Add(reservation);
		}
	}
}
