using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions // general purpose extensions class the we need some custom functionlationality extension methods.
    // we dont need to create new instance for that we will make it static , so we dont need to create its instance in order to use it.
    {
        public static void AddApplicationError(this HttpResponse response, string message){
            response.Headers.Add("Application-Error",message); // here we are adding the error message to the header 
            // in client side even when we send a handled error message,  it still gets confused and displays trash messages in console.
            // its misleading so inorder for the browser to show correctly we are adding these two
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
}