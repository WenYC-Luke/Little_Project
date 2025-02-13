
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Farmer_Project.Service.ServiceImpl
{
    public class AppServiceInitializerImpl : AppServiceInitializer
    {
        private readonly InitCreateDefaultAdmin _initAdmin;

        public AppServiceInitializerImpl(InitCreateDefaultAdmin initAdmin) { 
            _initAdmin = initAdmin;
        }

        public void initialize()
        {
            _initAdmin.CreateDefaultAdmin();
        }
    }
}
