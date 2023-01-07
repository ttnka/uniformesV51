using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Shared
{
    public class NavMenuBase : ComponentBase 
    {
        public int ElNivel = 1;
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UserRepo { get; set; } = default!;
        [Inject]
        public AuthenticationStateProvider GetAuthenticationStateAsync { get; set; } = default!;

        protected async override Task OnInitializedAsync()
        {
            try
            {
                var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
                var user = authstate.User;
                if (!user.Identity!.IsAuthenticated)
                { ElNivel = 1; }
                else
                {
                    var elUser = (await UserRepo.Get(x => x.OldEmail == user.Identity.Name))
                    .FirstOrDefault() ?? new();
                    if (elUser == null || string.IsNullOrEmpty(elUser.UserId) ||
                        elUser.UserId.Length < 10)
                    {
                        ElNivel = 1;
                    }
                    else
                    {
                        ElNivel = elUser.Nivel;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        
        public MyFunc MyFunc { get; set; } = new MyFunc();
    }
}
