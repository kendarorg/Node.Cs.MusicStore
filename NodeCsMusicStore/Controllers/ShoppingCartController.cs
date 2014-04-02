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


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NodeCsMusicStore.Models;
using NodeCsMusicStore.ViewModels;
using Node.Cs.Lib.Attributes;
using Node.Cs.Lib.Controllers;

namespace NodeCsMusicStore.Controllers
{
	public class ShoppingCartController : ControllerBase
	{
		MusicStoreEntities storeDB = new MusicStoreEntities();

		//
		// GET: /ShoppingCart/

		public IEnumerable<IResponse> Index()
		{
			var cart = ShoppingCart.Helper.GetCart(this.HttpContext);

			// Set up our ViewModel
			var viewModel = new ShoppingCartViewModel
			{
				CartItems = cart.GetCartItems(),
				CartTotal = cart.GetTotal()
			};

			// Return the view
			yield return View(viewModel);
		}

		//
		// GET: /Store/AddToCart/5

		public IEnumerable<IResponse> AddToCart(int id)
		{

			// Retrieve the album from the database
			var addedAlbum = storeDB.Albums
					.Single(album => album.AlbumId == id);

			// Add it to the shopping cart
			var cart = ShoppingCart.Helper.GetCart(this.HttpContext);

			cart.AddToCart(addedAlbum);

			// Go back to the main store page for more shopping
			yield return RedirectToAction("Index");
		}

		//
		// AJAX: /ShoppingCart/RemoveFromCart/5

		[HttpPost]
		public IEnumerable<IResponse> RemoveFromCart(int id)
		{
			// Remove the item from the cart
			var cart = ShoppingCart.Helper.GetCart(this.HttpContext);

			// Get the name of the album to display confirmation
			string albumName = storeDB.Carts
					.Single(item => item.RecordId == id).Album.Title;

			// Remove from cart
			int itemCount = cart.RemoveFromCart(id);

			// Display the confirmation message
			var results = new ShoppingCartRemoveViewModel
			{
				Message = HttpUtility.HtmlEncode(albumName) +
							" has been removed from your shopping cart.",
				CartTotal = cart.GetTotal(),
				CartCount = cart.GetCount(),
				ItemCount = itemCount,
				DeleteId = id
			};

			yield return JsonResponse(results);
		}

		//
		// GET: /ShoppingCart/CartSummary

		[ChildActionOnly]
		public IEnumerable<IResponse> CartSummary()
		{
			var cart = ShoppingCart.Helper.GetCart(this.HttpContext);

			ViewData["CartCount"] = cart.GetCount();

			yield return PartialView("CartSummary");
		}
	}
}