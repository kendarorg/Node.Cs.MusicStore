// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.Linq;
using Node.Cs.MVC.Controllers;
using NodeCsMusicStore.Models;
using Node.Cs.Authorization;
using Node.Cs.Lib.Attributes;
using Node.Cs.Lib.Contexts;
using Node.Cs.Lib.Controllers;

namespace NodeCsMusicStore.Controllers
{
	[Authorize]
	public class CheckoutController : ControllerBase
	{
		MusicStoreEntities storeDB = new MusicStoreEntities();
		const string PromoCode = "FREE";

		//
		// GET: /Checkout/AddressAndPayment

		public IEnumerable<IResponse> AddressAndPayment()
		{
			yield return View(new Order());
		}

		//
		// POST: /Checkout/AddressAndPayment

		[HttpPost]
		public IEnumerable<IResponse> AddressAndPayment(FormCollection values)
		{
			var order = new Order();
			TryUpdateModel(order);


			if (string.Equals(values["PromoCode"], PromoCode,
					StringComparison.OrdinalIgnoreCase) == false)
			{
				yield return View(order);
			}
			else
			{
				var invalid = false;
				try
				{
					order.Username = User.Identity.Name;
					order.OrderDate = DateTime.Now;

					//Save Order
					storeDB.Orders.Add(order);
					storeDB.SaveChanges();

					//Process the order
					var cart = ShoppingCart.Helper.GetCart(this.HttpContext);
					cart.CreateOrder(order);
				}
				catch
				{
					//Invalid - redisplay with errors
					invalid = true;
				}

				if (invalid)
				{
					yield return View(order);
				}
				else
				{
					yield return RedirectToAction("Complete",
						new { id = order.OrderId });
				}
			}

		}

		//
		// GET: /Checkout/Complete

		public IEnumerable<IResponse> Complete(int id)
		{
			// Validate customer owns this order
			bool isValid = storeDB.Orders.Any(
					o => o.OrderId == id &&
					o.Username == User.Identity.Name);

			if (isValid)
			{
				yield return View(id);
			}
			else
			{
				yield return View("Error");
			}
		}
	}
}
