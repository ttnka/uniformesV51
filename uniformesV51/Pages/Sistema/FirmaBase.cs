using Microsoft.AspNetCore.Components;
using uniformesV51.Model;

namespace uniformesV51.Pages.Sistema
{
    public class FirmaBase : ComponentBase
    {
        [Parameter]
        public string UrlRegreso { get; set; } = "";
        [Inject]
        public NavigationManager NM { get; set; } = default!;
        public AddUser ElNuevo { get; set; } = new AddUser();
        [Inject]
        public IAddUser AddUserRepo { get; set; } = default!;
        public async Task FirmaInn()
        {
            ElNuevo.FirmaIn = true;
            var respuesta = await AddUserRepo.FirmaIn(ElNuevo);

            UrlRegreso = respuesta.Exito ? UrlRegreso : "/";
            
            NM.NavigateTo(UrlRegreso, true);
        }
    }
}
