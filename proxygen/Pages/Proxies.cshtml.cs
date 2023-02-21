using Microsoft.AspNetCore.Mvc.RazorPages;
using proxygen.Models;
using System.Threading.Tasks;

namespace proxygen.Pages
{
    public class ProxiesModel : PageModel
    {
        public void OnGet()
        {

        }

        public async Task<ProxyViewModel> OnPostAsync()
        {
            var foo = new ProxyViewModel();

            foo.Cards.Add(new CardProxies { Card = new CardModel { name = "Larador, Karador's Brother" } });

            return foo;
        }
    }
}