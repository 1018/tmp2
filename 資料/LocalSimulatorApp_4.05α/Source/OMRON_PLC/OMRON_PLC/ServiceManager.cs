using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMRON.Compolet.FinsGateway;
using OMRON.FinsGateway.Service;

namespace OMRON_PLC
{
    public class ServiceManager
    {
        private FgwScm fgwscm = new FgwScm();
        private string serviceName;
        private Service service;

        public ServiceManager(string serviceName)
        {
            this.serviceName = serviceName;       
        }

        public void Start()
        {           

            //FinsGatewayサービスマネージャ起動
            if (!fgwscm.IsAlive)
            {
                fgwscm.StartManager();
                PlcCommunication.logManager.Write("FinsGatewayサービスマネージャ起動");
            }

            this.service = fgwscm.Services.FirstOrDefault((s) => (s.Name == serviceName));
            if (this.service == null)
            {
                PlcCommunication.logManager.Write("無効なサービス名です。" + serviceName );
                fgwscm.StopManager();
                PlcCommunication.logManager.Write("FinsGatewayサービスマネージャ停止");
                return; 
            }

            try
            {
                if (this.service.Status == Service.ServiceStatus.Stopped)
                {
                    this.service.Start();
                    if (this.service.WaitForStatus(Service.ServiceStatus.Running, 100))
                    {
                        PlcCommunication.logManager.Write(serviceName + "サービス起動");
                        return;
                    }
                }
            }
            catch
            {
                fgwscm.StopManager();
                PlcCommunication.logManager.Write("FinsGatewayサービスマネージャ停止");                
                return; 
            }

            return;

        }

        public void End()
        {
            try
            {
                if (this.service.WaitForStatus(Service.ServiceStatus.Stopped, 100))
                {
                    PlcCommunication.logManager.Write(serviceName + "サービス停止");
                }
            }
            catch { }

            try
            {
                if (fgwscm.IsAlive)
                {
                    fgwscm.StopManager();
                    PlcCommunication.logManager.Write("FinsGatewayサービスマネージャ停止");
                }
            }
            catch { }

        }        

        public static IEnumerable<string> GetServiceNames()
        {
            FgwScm temp = new FgwScm();
            return temp.ServiceNames;
        }
        
    }
}
