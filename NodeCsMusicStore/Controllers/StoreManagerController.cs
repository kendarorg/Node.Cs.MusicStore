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
using System.Data;
using System.Data.Entity;
using System.Linq;
using Node.Cs.MVC.Controllers;
using NodeCsMusicStore.Models;
using Node.Cs.Authorization;
using Node.Cs.Lib.Attributes;
using Node.Cs.Lib.Controllers;
using Node.Cs.Lib.Utils;
using Node.Cs.Razor.Helpers;

namespace NodeCsMusicStore.Controllers
{
	[Authorize(Roles = "Administrator")]
	public class StoreManagerController : ControllerBase, IDisposable
	{
		private MusicStoreEntities db = new MusicStoreEntities();

		//
		// GET: /StoreManager/

		public IEnumerable<IResponse> Index()
		{
			var albums = db.Albums.Include(a => a.Genre).Include(a => a.Artist);
			yield return View(albums.ToList());
		}

		//
		// GET: /StoreManager/Details/5

		public IEnumerable<IResponse> Details(int id)
		{
			Album album = db.Albums.Find(id);
			yield return View(album);
		}

		//
		// GET: /StoreManager/Create

		public IEnumerable<IResponse> Create()
		{
			ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name");
			ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name");
			yield return View(new Album());
		}

		//
		// POST: /StoreManager/Create

		[HttpPost]
		public IEnumerable<IResponse> Create(Album album)
		{
			if (ModelState.IsValid)
			{
				db.Albums.Add(album);
				db.SaveChanges();
				yield return RedirectToAction("Index");
			}

			ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
			ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
			yield return View(album);
		}

		//
		// GET: /StoreManager/Edit/5

		public IEnumerable<IResponse> Edit(int id)
		{
			Album album = db.Albums.Find(id);
			ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
			ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
			yield return View(album);
		}

		//
		// POST: /StoreManager/Edit/5

		[HttpPost]
		public IEnumerable<IResponse> Edit(Album album)
		{
			if (ModelState.IsValid)
			{
				db.Entry(album).State = EntityState.Modified;
				db.SaveChanges();
				yield return RedirectToAction("Index");
			}
			ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "Name", album.GenreId);
			ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);
			yield return View(album);
		}

		//
		// GET: /StoreManager/Delete/5

		public IEnumerable<IResponse> Delete(int id)
		{
			Album album = db.Albums.Find(id);
			yield return View(album);
		}

		//
		// POST: /StoreManager/Delete/5

		[HttpPost, ActionName("Delete")]
		public IEnumerable<IResponse> DeleteConfirmed(int id)
		{
			Album album = db.Albums.Find(id);
			db.Albums.Remove(album);
			db.SaveChanges();
			yield return RedirectToAction("Index");
		}

		public void Dispose()
		{
			db.Dispose();
		}
	}
}