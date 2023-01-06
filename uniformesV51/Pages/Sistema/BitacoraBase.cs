using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Radzen.Blazor;
using System;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Pages.Sistema
{
    public class BitacoraBase : ComponentBase
    {
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UserRepo { get; set; } = default!;
        public List<Z110_Usuarios> LosUsers { get; set; } = new List<Z110_Usuarios>();
        [Inject]
        public Repo<Z190_Bitacora, ApplicationDbContext> BitaRepo { get; set; } = default!;
        public List<Z190_Bitacora> LasBitas { get; set; } = new List<Z190_Bitacora>();
        public RadzenDataGrid<Z190_Bitacora>? BitaGrid { get; set; } =
            new RadzenDataGrid<Z190_Bitacora>();
        public Z190_Bitacora SearchBita { get; set; } = new();
        public MyFunc MyFunc { get; set; } = new();
        string ErrorMsn = "";
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
                var user = authstate.User;
                if (!user.Identity!.IsAuthenticated)
                    NM.NavigateTo("Identity/Account/Login?ReturnUrl=/bitacora", true);
                ElUser = (await UserRepo.Get(x => x.OldEmail == user.Identity.Name))
                    .FirstOrDefault() ?? new();
                if (ElUser == null || string.IsNullOrEmpty(ElUser.UserId))
                    NM.NavigateTo("/Bloqueado", true);
                var bitaTemp = MyFunc.MakeBitacora(ElUser.UserId, ElUser.OrgId,
                            "Bitacora, Se consulto listado de bitacora!", false);
                await BitaRepo.Insert(bitaTemp);
                await Arranque();
            }
            catch (Exception)
            {
                NM.NavigateTo($"Error {ErrorMsn}");
            }
        }
        protected async Task Arranque()
        {   
            await LeerUsers();
            await LeerBitacoras();
        }
        public async Task LeerBitacoras()
        {
            ErrorMsn = "";
            IEnumerable<Z190_Bitacora> resultado = new List<Z190_Bitacora>();
            try
            {
                if (SearchBita.UserId == "Filtro")
                {
                    resultado = await BitaRepo.Get(x => x.OrgId == ElUser.OrgId &
                    x.Sistema == SearchBita.Sistema & x.Desc.Contains(SearchBita.Desc));
                }
                else
                {
                    resultado = await BitaRepo.Get(x => x.OrgId == ElUser.OrgId);
                }
                if (resultado != null)
                {
                    resultado = resultado.OrderByDescending(x => x.Fecha);
                    LasBitas = resultado.ToList<Z190_Bitacora>();
                }
                else
                {
                    ErrorMsn = "No hay registros de bitacora";
                }   
            }
            catch (Exception ex)
            {
                ErrorMsn = ex.Message;
                LasBitas = new();
            }
        }
        public async Task LeerUsers()
        {
            ErrorMsn = "";
            try
            {
                var resultado = await UserRepo.GetAll();
                if (resultado != null)
                {
                    LosUsers = resultado.ToList<Z110_Usuarios>();
                }
                else
                {
                    ErrorMsn = "No hay registros de usuarios";
                }
            }
            catch (Exception ex)
            {
                ErrorMsn = ex.Message;
                LosUsers = new();
            }
        }
        public Z110_Usuarios ElUser { get; set; } = new();
        [Inject]
        public NavigationManager NM { get; set; } = default!;

        [Inject] 
        public AuthenticationStateProvider GetAuthenticationStateAsync { get; set; } = default!;    

        

    }
}
