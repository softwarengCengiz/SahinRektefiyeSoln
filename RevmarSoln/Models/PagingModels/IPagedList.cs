﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.PagingModels
{
	public interface IPagedList
	{
		int PageCount { get; }
		int TotalItemCount { get; }
		int PageIndex { get; }
		int PageNumber { get; }
		int PageSize { get; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		bool IsFirstPage { get; }
		bool IsLastPage { get; }
		int ItemStart { get; }
		int ItemEnd { get; }
	}

	public interface IPagedList<out T> : IPagedList, IEnumerable<T>
	{
	}
}