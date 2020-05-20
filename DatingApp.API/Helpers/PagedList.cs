using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    // this class casn be used for paging we will do somet type of extension here.
    public class PagedList<T> : List<T> // can be used for generic class
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            // example if there are 13 users , if page size is 5 ,ceiling will return 3 
            TotalPages = (int)Math.Ceiling(count/ (double)pageSize);
            this.AddRange(items);
        }

        // retunrnign a new instance of a paged list with some attributes defined.
            // what we are doing in the class and method is 
            // when we call this method, we will return a new paged list with passed info
            // in constructor we are assining the values
            // on client side it can udnerstand what to display and how to request next
            // count async to return no of elements. 
            // then we get the items from the source of items that are passed to this
            // but we ill use paging info to decide what to send back
            // if we have 13 users , we had a page size 5 and requestsing the second page of users , 
                // we have to skip then select next 5 from 6th.
                // if page 1 we will return first five users. in the end sending back that list.
            // if we get page no 3 with size 5 it will skip(3-1) * 5 then return next remaining using take per size
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}