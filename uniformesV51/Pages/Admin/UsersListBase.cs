using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using uniformesV51.Model;

namespace uniformesV51.Pages.Admin
{
    public class UsersListBase : ComponentBase 
    {
        [CascadingParameter(Name = "ElUserAll")]
        public Z110_Usuarios ElUser { get; set; } = new();
        [CascadingParameter(Name = "LasOrgsAll")]
        public List<Z100_Org> LasOrgs { get; set; } = new List<Z100_Org>();
        public List<KeyValuePair<int, string>> LosNiveles { get; set; } =
            new List<KeyValuePair<int, string>>();
        public List<KeyValuePair<int, string>> LosNivelesTitulos { get; set; } =
            new List<KeyValuePair<int, string>>();
        public RadzenDataGrid<Z110_Usuarios>? UsersGrid { get; set; } =
            new RadzenDataGrid<Z110_Usuarios>();
        [Parameter]
        public EventCallback LeerUsersC { get; set; }
        [Parameter]
        public EventCallback<Z190_Bitacora> BitacoraC { get; set; }
        [Parameter]
        public List<Z110_Usuarios> LosUsers { get; set; } = new List<Z110_Usuarios>();
        protected async Task LeerUsers()
        {
            await LeerUsersC.InvokeAsync();
            Editando = false;
        }
        protected void LeerNiveles()
        {
            string[] NomNiveles = UserNivel.Titulos.Split(",");

            switch (ElUser.OldEmail)
            {
                case Constantes.UserNameMailPublico:
                    LosNiveles.Add(new KeyValuePair<int, string>(1, NomNiveles[0].ToString()));
                    break;

                default:
                    for (int i = 0; i < NomNiveles.Length; i++)
                    {
                        if (ElUser.Nivel > i)
                            LosNiveles.Add(new KeyValuePair<int, string>
                                (i + 1, NomNiveles[i]));
                    }
                    break;
            }
        }

        protected MyFunc MyFunc { get; set; } = new();
        protected bool Editando = false;
    }
}
