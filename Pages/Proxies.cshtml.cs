using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proxygen.Models;

namespace proxygen.Pages
{
    public class ProxiesModel : PageModel
    {
        public List<ProxyPageModel> Proxies { get; set; }
        public PageResult OnGetAsync()
        {
            Console.WriteLine("here!");

            var foo = JsonSerializer.Deserialize<List<ProxyPageModel>>(TempData["cards"] as string);

            if(foo== null || foo.Count == 0)
            {
                RedirectToPage("Index");
            }

            Proxies = foo;

            return Page();
        }

        public void OnPostAsync()
        {

        }
    }
}