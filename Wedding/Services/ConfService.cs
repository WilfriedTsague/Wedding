namespace Wedding.Services
{
    public class ConfService
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Ajoutez l'authentification
            services.AddAuthentication("MyCookieAuth")
                .AddCookie("MyCookieAuth", options =>
                {
                    options.LoginPath = "/Account/Login"; // Chemin vers votre page de connexion
                    options.LogoutPath = "/Account/Logout"; // Chemin vers votre action de déconnexion
                });

            // Ajoutez les autres services
            services.AddControllersWithViews();
        }

    }


}
