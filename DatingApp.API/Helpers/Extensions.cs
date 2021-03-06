using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions // general purpose extensions class the we need some custom functionlationality extension methods.
    // we dont need to create new instance for that we will make it static , so we dont need to create its instance in order to use it.
    // All these m,ethods are cusomt extension of something that we do not have from C# which we are trying to extendd and use for our own
    {
        public static void AddApplicationError(this HttpResponse response, string message){
            response.Headers.Add("Application-Error",message); // here we are adding the error message to the header 
            // in client side even when we send a handled error message,  it still gets confused and displays trash messages in console.
            // its misleading so inorder for the browser to show correctly we are adding these two
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }

        public static int CalculateAge(this DateTime theDateTime){
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today) age--;
            return age;
        }

        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages){
            var paginationHeader = new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPages);
            // these two below lines will send headers in camelcase as oopposed to title case , camelCase insetead of CamelCase
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}