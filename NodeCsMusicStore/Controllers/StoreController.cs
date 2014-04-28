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
using Node.Cs.Lib.Attributes;
using Node.Cs.Lib.Controllers;

namespace NodeCsMusicStore.Controllers
{
	public class StoreController : ControllerBase
	{
		MusicStoreEntities storeDB = new MusicStoreEntities();

		//
		// GET: /Store/

		public IEnumerable<IResponse> Index()
		{
			var genres = storeDB.Genres.ToList();

			yield return View(genres);
		}

		//
		// GET: /Store/Browse?genre=Disco

		public IEnumerable<IResponse> Browse(string genre)
		{
			// Retrieve Genre and its Associated Albums from database
			var genreModel = storeDB.Genres.Include("Albums")
				.Single(g => g.Name == genre);

			yield return View(genreModel);
		}

		//
		// GET: /Store/Details/5

		public IEnumerable<IResponse> Details(int id)
		{
			var album = storeDB.Albums.Find(id);

			yield return View(album);
		}

		//
		// GET: /Store/GenreMenu

		[ChildActionOnly]
		public IEnumerable<IResponse> GenreMenu()
		{
			var genres = storeDB.Genres.ToList();

			yield return PartialView(genres);
		}

	}
}