using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ManagementWeb; 

namespace KühlschrankAbrechnung
{


    public class WebServerManager
    {
        private IHost _webHost;

        public void StartServer()
        {
            if (_webHost != null)
                return;

            var app = WebServerFactory.CreateWebApp();

            _ = app.StartAsync();   // Starte Server non-blocking

            _webHost = app;
        }

        public async Task StopServer()
        {
            if (_webHost != null)
            {
                await _webHost.StopAsync();
                _webHost.Dispose();
                _webHost = null;
            }
        }

        public bool IsRunning => _webHost != null;
    }


}
