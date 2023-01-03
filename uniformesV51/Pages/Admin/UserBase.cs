using Microsoft.AspNetCore.Components;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Pages.Admin
{
    public class UserBase : ComponentBase 
    {
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UsersRepo { get; set; } = default!;
        public List<Z110_Usuarios> LosUsers { get; set; } = new List<Z110_Usuarios>();
        [Parameter]
        public EventCallback LeerUsersC { get; set; }
        [CascadingParameter (Name ="BitacoraAll")]
        public EventCallback<Z190_Bitacora> BitacoraC { get; set; }
        protected string ErrorMsn = string.Empty;
        public bool Editando = false;
        protected override async Task OnParametersSetAsync()
        {
            await LeerUsersC.InvokeAsync();

            var bitaTemp = MyFunc.MakeBitacora(ElUser.UserId, ElUser.OrgId, 
                "Consulto listado de usuarios", false);
            await BitacoraC.InvokeAsync(bitaTemp);

        }
        protected async Task LeerUsers()
        {
            await LeerUsersC.InvokeAsync();
            Editando= false;
        }
        [CascadingParameter(Name = "ElUserAll")]
        public Z110_Usuarios ElUser { get; set; } = new();
        public MyFunc MyFunc { get; set; } = new();
        
    }
}
