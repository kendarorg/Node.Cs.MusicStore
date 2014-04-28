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
using Node.Cs.MVC.Controllers;
using NodeCsMusicStore.Models;
using Node.Cs.Authorization;
using Node.Cs.Lib.Attributes;
using Node.Cs.Lib.Controllers;
using Node.Cs.Lib.Utils;
using Node.Cs.Lib;

namespace NodeCsMusicStore.Controllers
{
	public class AccountController : ControllerBase
	{

		private void MigrateShoppingCart(string UserName)
		{
			// Associate shopping cart items with logged-in user
			var cart = ShoppingCart.Helper.GetCart(this.HttpContext);

			cart.MigrateCart(UserName);
			Session[ShoppingCart.CartSessionKey] = UserName;
		}

		//
		// GET: /Account/LogOn

		public IEnumerable<IResponse> LogOn()
		{
			yield return View(new LogOnModel());
		}

		//
		// POST: /Account/LogOn

		[HttpPost]
		public IEnumerable<IResponse> LogOn(LogOnModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var authProvider = GlobalVars.AuthenticationDataProvider;
				if (authProvider.IsUserAuthorized(model.UserName, model.Password))
				{
					MigrateShoppingCart(model.UserName);

					Authorize.FormSetAuthCookie(HttpContext, model.UserName, model.RememberMe);
					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
							&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
					{
						yield return Redirect(returnUrl);
					}
					else
					{
						yield return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			yield return View(model);
		}

		//
		// GET: /Account/LogOff

		public IEnumerable<IResponse> LogOff()
		{
			Authorize.FormSignOut(HttpContext);

			yield return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/Register

		public IEnumerable<IResponse> Register()
		{
			yield return View(new RegisterModel());
		}

		//
		// POST: /Account/Register

		[HttpPost]
		public IEnumerable<IResponse> Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				var authProvider = GlobalVars.AuthenticationDataProvider;

				// Attempt to register the user
				AuthenticationCreateStatus createStatus;
				authProvider.CreateUser(model.UserName, model.Password, model.Email, "question", "answer", true, null, out createStatus);

				if (createStatus == AuthenticationCreateStatus.Success)
				{
					MigrateShoppingCart(model.UserName);

					Authorize.FormSetAuthCookie(HttpContext, model.UserName, false /* createPersistentCookie */);
					yield return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", ErrorCodeToString(createStatus));
				}
			}

			// If we got this far, something failed, redisplay form
			yield return View(model);
		}

		//
		// GET: /Account/ChangePassword

		[Authorize]
		public IEnumerable<IResponse> ChangePassword()
		{
			yield return View(new ChangePasswordModel());
		}

		//
		// POST: /Account/ChangePassword

		[Authorize]
		[HttpPost]
		public IEnumerable<IResponse> ChangePassword(ChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{

				// ChangePassword will throw an exception rather
				// than return false in certain failure scenarios.
				bool changePasswordSucceeded;
				try
				{
					var authProvider = GlobalVars.AuthenticationDataProvider;
					var currentUser = authProvider.GetUser(User.Identity.Name, true /* userIsOnline */);
					changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
				}
				catch (Exception)
				{
					changePasswordSucceeded = false;
				}

				if (changePasswordSucceeded)
				{
					yield return RedirectToAction("ChangePasswordSuccess");
				}
				else
				{
					ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
				}
			}

			// If we got this far, something failed, redisplay form
			yield return View(model);
		}

		//
		// GET: /Account/ChangePasswordSuccess

		public IEnumerable<IResponse> ChangePasswordSuccess()
		{
			yield return View();
		}

		#region Status Codes
		private string ErrorCodeToString(AuthenticationCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for
			// a full list of status codes.
			switch (createStatus)
			{
				case AuthenticationCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name.";

				case AuthenticationCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

				case AuthenticationCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case AuthenticationCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case AuthenticationCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case AuthenticationCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case AuthenticationCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case AuthenticationCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case AuthenticationCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
		#endregion
	}
}
